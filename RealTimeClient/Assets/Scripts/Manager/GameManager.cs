//---------------------------------------------------------------
// �Q�[���}�l�[�W���[ [ GameManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/11/18
// Update:2024/12/05
//---------------------------------------------------------------
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using DG.Tweening;
using RealTimeServer.Model.Entity;
using DavidJalbert;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //=====================================
    // �t�B�[���h

    /// <summary>
    /// �Q�[����Ԏ��
    /// </summary>
    private enum GameState
    {
        None = 0,
        Join,
        InGame
    }

    /// <summary>
    /// �ڑ�ID���L�[�ɃL�����N�^�̃I�u�W�F�N�g���Ǘ�
    /// </summary>
    private Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    /// <summary>
    /// �Q�[�����
    /// </summary>
    private GameState gameState = GameState.None;

    /// <summary>
    /// �␳pos
    /// </summary>
    private Vector3 posCorrection = new Vector3(0.0f,-0.9f,0.0f);

    /// <summary>
    /// �v���C���[�R���g���[���[
    /// </summary>
    private GameObject playerController;

    /// <summary>
    /// ����R���g���[���[
    /// </summary>
    private GameObject inputController;

    /// <summary>
    /// �z�C�[���̊p�x�擾�p
    /// </summary>
    private Transform wheelAngle;

    [Header("�e��Object���A�^�b�`")]

    /// <summary>
    /// ���[�����f���i�[�p
    /// </summary>
    [SerializeField] private RoomModel roomModel;

    /// <summary>
    /// ��������v���C���[�̃L�����N�^�[�v���n�u
    /// </summary>
    [SerializeField] private GameObject playerPrefab;

    /// <summary>
    /// �������鑼�v���C���[�̃L�����N�^�[�v���n�u
    /// </summary>
    [SerializeField] private GameObject otherPrefab;

    /// <summary>
    /// ���͏����v���n�u
    /// </summary>
    [SerializeField] private GameObject inputPrefab;

    /// <summary>
    /// �v���C���[���i�[����e�I�u�W�F�N�g
    /// </summary>
    [SerializeField] private GameObject parentObj;

    /// <summary>
    /// �v���C���[���Ƃ̃��X�n�_
    /// </summary>
    [SerializeField] private Transform[] respownList;

    /// <summary>
    /// �ʐM���x
    /// </summary>
    [SerializeField] private float internetSpeed = 0.1f;

    /// <summary>
    /// ��������
    /// </summary>
    [SerializeField] private float timeLimit = 3.0f;

    [Space (40)]
    [Header("===== UI�֘A =====")]

    [Space(10)]
    [Header("---- Text ----")]

    /// <summary>
    /// ���[�U�[ID
    /// </summary>
    [SerializeField] private Text idText;

    /// <summary>
    /// �^�C�}�[
    /// </summary>
    [SerializeField] private Text timerText;

    /// <summary>
    /// ���ҕ\��
    /// </summary>
    [SerializeField] private Text winnerText;

    [Space(10)]
    [Header("---- InputField ----")]

    /// <summary>
    /// ID���͗�
    /// </summary>
    [SerializeField] private InputField idInput;

    [Space(10)]
    [Header("---- Button ----")]

    /// <summary>
    /// �����{�^��
    /// </summary>
    [SerializeField] private Button joinButton;

    /// <summary>
    /// �ގ��{�^��
    /// </summary>
    [SerializeField] private Button exitButton;

    [Space(10)]
    [Header("---- Panel ----")]

    /// <summary>
    /// ���U���g�p�l��
    /// </summary>
    [SerializeField] private GameObject resultPanel;

    //=====================================
    // ���\�b�h

    /// <summary>
    /// ��������
    /// </summary>
    void Start()
    {
        // �e�ʒm���͂����ۂɍs�����������f���ɓo�^����
        roomModel.OnJoinedUser += OnJoinedUser;         // ����
        roomModel.OnExitedUser += OnExitedUser;         // �ގ�
        roomModel.OnMovedUser += OnMovedUser;           // �ړ�
        roomModel.OnInGameUser += OnInGameUser;         // �C���Q�[��
        roomModel.OnStartGameUser += OnStartGameUser;   // �Q�[���X�^�[�g
        roomModel.OnEndGameUser += OnEndGameUser;       // �Q�[���I��

        ChangeUI(gameState);
    }

    /// <summary>
    /// �X�V����
    /// </summary>
    void Update()
    {
        
    }

    /// <summary>
    /// �ړ��f�[�^���M����
    /// </summary>
    private async void SendMoveData()
    {
        if (gameState == GameState.None) return;

        // �ړ��f�[�^�̍쐬
        var moveData = new MoveData()
        {
            ConnectionId = roomModel.ConnectionId,
            Position = playerController.transform.position + posCorrection,
            Rotation = playerController.transform.eulerAngles,
            WheelAngle = wheelAngle.eulerAngles.y
        };

        await roomModel.MoveAsync(moveData);
    }

    /// <summary>
    /// �ڑ�����
    /// </summary>
    public async void OnConnect()
    {
        int userId = int.Parse(idText.text);    // ���͂������[�U�[ID�̕ϊ�

        // �ڑ�
        await roomModel.ConnectAsync();
        // ���� (���[�����ƃ��[�U�[ID��n���ē����B�ŏI�I�ɂ̓��[�J���f�[�^�̃��[�U�[ID���g�p)
        await roomModel.JoinAsync("sampleRoom", userId);

        Debug.Log("����");
    }

    /// <summary>
    /// �ؒf����
    /// </summary>
    public async void OnDisconnect()
    {
        CancelInvoke();

        // �ޏo
        await roomModel.ExitAsync();

        // �v���C���[�I�u�W�F�N�g�̍폜
        foreach (Transform child in parentObj.transform)
        {
            Destroy(child.gameObject);
        }

        gameState = GameState.None;
        ChangeUI(gameState);
        Debug.Log("�ޏo");
    }

    /// <summary>
    /// �Q�[���X�^�[�g�ʒm����
    /// </summary>
    public async void OnStart()
    {
        await roomModel.GameStartAsync();
    }

    // --------------------------------------------------------------
    // ���f���o�^�p�֐�

    /// <summary>
    /// �����ʒm��M���̏���
    /// </summary>
    /// <param name="user"></param>
    private void OnJoinedUser(JoinedUser user)
    {
        Debug.Log(user.JoinOrder + "P");

        GameObject characterObj;    // ���������I�u�W�F

        // ���������v���C���[��
        if (user.ConnectionId == roomModel.ConnectionId)
        {
            // ���@�E����p�I�u�W�F�̐���
            characterObj = Instantiate(playerPrefab, respownList[user.JoinOrder - 1].position, Quaternion.Euler(0, 180, 0));
            inputController = Instantiate(inputPrefab, Vector3.zero, Quaternion.identity);

            // �J�[�R���g���[���[�̎擾�E���f
            playerController = characterObj.transform.GetChild(0).gameObject;
            wheelAngle = characterObj.transform.Find("Visuals/WheelFrontLeft").transform;

            // UI�ύX
            gameState = GameState.Join;
            ChangeUI(gameState);
            Debug.Log("���@��������");

            // �ʒu���M�J�n
            InvokeRepeating("SendMoveData", 0.5f, internetSpeed);
        }
        else
        {
            // ���v���C���[�̐���
            characterObj = Instantiate(otherPrefab, respownList[user.JoinOrder - 1].position, Quaternion.Euler(0, 180, 0));
        }

        characterObj.transform.parent = parentObj.transform;    // �e�̐ݒ�
        characterList[user.ConnectionId] = characterObj;        // �t�B�[���h�ŕۑ�
    }

    /// <summary>
    /// �ޏo�ʒm��M���̏���
    /// </summary>
    /// <param name="user"></param>
    private void OnExitedUser(JoinedUser user)
    {
        // �ʒu���̍X�V
        if (!characterList.ContainsKey(user.ConnectionId)) return;  // �ޏo�҃I�u�W�F�̑��݃`�F�b�N

        Destroy(characterList[user.ConnectionId]);   // �I�u�W�F�N�g�̔j��
        characterList.Remove(user.ConnectionId);     // ���X�g����폜
    }

    /// <summary>
    /// �v���C���[���ړ������Ƃ��̏���
    /// </summary>
    /// <param name="moveData"></param>
    private void OnMovedUser(MoveData moveData)
    {
        // �ʒu���̍X�V
        if (!characterList.ContainsKey(moveData.ConnectionId)) return;

        // �{�̈ʒu�̍X�V
        characterList[moveData.ConnectionId].transform.DOMove(moveData.Position, internetSpeed).SetEase(Ease.Linear);
        characterList[moveData.ConnectionId].transform.DORotate(moveData.Rotation, internetSpeed).SetEase(Ease.Linear);

        // �^�C���p�̍X�V
        characterList[moveData.ConnectionId].transform.Find("wheels/wheel front right").transform.DORotate(new Vector3(0,moveData.WheelAngle,0),internetSpeed).SetEase(Ease.Linear);
        characterList[moveData.ConnectionId].transform.Find("wheels/wheel front left").transform.DORotate(new Vector3(0, moveData.WheelAngle, 0), internetSpeed).SetEase(Ease.Linear);
    }

    /// <summary>
    /// �C���Q�[���ʒm��M����
    /// </summary>
    private void OnInGameUser()
    {
        // �J�E���g�_�E���J�n
        Debug.Log("�J�E���g�_�E��");
        StartCoroutine("StartCount");
    }

    /// <summary>
    /// �Q�[���X�^�[�g�ʒm��M����
    /// </summary>
    private void OnStartGameUser()
    {
        // �e�L�X�g�ύX
        timerText.text = "�J�n!";
        StartCoroutine("HiddenText");
        // ����\�ɂ���
        inputController.GetComponent<TinyCarStandardInput>().carController = playerController.GetComponent<TinyCarController>();
    }

    /// <summary>
    /// �Q�[���I���ʒm��M����
    /// </summary>
    private void OnEndGameUser(int plNo, string name)
    {
        // ����s�\�ɂ���
        inputController.GetComponent<TinyCarStandardInput>().carController = null;

        //++ �I��SE�Đ�

        // �I���\��
        timerText.text = "�I��!";

        // 1�b��Ƀ��U���g�\��
        winnerText.text = name;
        StartCoroutine("DisplayResult");
    }

    /// <summary>
    /// �\��UI�̕ύX
    /// </summary>
    /// <param name="gameState"></param>
    private void ChangeUI(GameState gameState)
    {
        switch (gameState)
        {
            // �ގ���� ---------------------------------------------
            case GameState.None:
                // InputField
                idInput.enabled = true;

                // Button
                joinButton.gameObject.SetActive(true);
                exitButton.gameObject.SetActive(false);
                break;

            // ������� ---------------------------------------------
            case GameState.Join:
                // InputField
                idInput.enabled = false;

                // Button
                joinButton.gameObject.SetActive(false);
                exitButton.gameObject.SetActive(true);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// �Q�[���J�E���g
    /// </summary>
    /// <returns></returns>
    IEnumerator StartCount()
    {
        for (int i = 3; i > 0; i--)
        {
            timerText.text = i.ToString();

            // 1�b�҂��ăR���[�`�����f
            yield return new WaitForSeconds(1.0f);

            if (i == 1)
            {
                OnStart();
            }
        }
    }

    /// <summary>
    /// �^�C�}�[�e�L�X�g��\������
    /// </summary>
    /// <returns></returns>
    IEnumerator HiddenText()
    {
        // 1�b�҂��ăR���[�`�����f
        yield return new WaitForSeconds(0.8f);

        timerText.text = "";
    }

    /// <summary>
    /// ���U���g�\��
    /// </summary>
    /// <returns></returns>
    IEnumerator DisplayResult()
    {
        // 1�b�҂��ăR���[�`�����f
        yield return new WaitForSeconds(1.0f);

        timerText.text = "";    // �I����\��
        resultPanel.gameObject.SetActive(true); // ���U���g�p�l���\��
    }

    /// <summary>
    /// �^�C�g���{�^��������
    /// </summary>
    public async void OnTitleButton()
    {
        CancelInvoke();

        // �ޏo
        await roomModel.ExitAsync();

        // �v���C���[�I�u�W�F�N�g�̍폜
        foreach (Transform child in parentObj.transform)
        {
            Destroy(child.gameObject);
        }

        gameState = GameState.None;
        ChangeUI(gameState);
        Debug.Log("�ޏo");

        // �^�C�g���ɖ߂�
        SceneManager.LoadScene("01_TitleScene");
    }
}
