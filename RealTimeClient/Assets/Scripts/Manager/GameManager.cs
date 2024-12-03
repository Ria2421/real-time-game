//---------------------------------------------------------------
// �Q�[���}�l�[�W���[ [ GameManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/11/18
// Update:2024/11/26
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

    //=====================================
    // ���\�b�h

    // ��������
    void Start()
    {
        // �e�ʒm���͂����ۂɍs�����������f���ɓo�^����
        roomModel.OnJoinedUser += OnJoinedUser;         // ����
        roomModel.OnExitedUser += OnExitedUser;         // �ގ�
        roomModel.OnMovedUser += OnMovedUser;           // �ړ�
        roomModel.OnInGameUser += OnInGameUser;         // �C���Q�[��
        roomModel.OnStartGameUser += OnStartGameUser;   // �Q�[���X�^�[�g

        ChangeUI(gameState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // �ړ��f�[�^���M����
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

    // �ڑ�����
    public async void OnConnect()
    {
        int userId = int.Parse(idText.text);    // ���͂������[�U�[ID�̕ϊ�

        // �ڑ�
        await roomModel.ConnectAsync();
        // ���� (���[�����ƃ��[�U�[ID��n���ē����B�ŏI�I�ɂ̓��[�J���f�[�^�̃��[�U�[ID���g�p)
        await roomModel.JoinAsync("sampleRoom", userId);

        Debug.Log("����");
    }

    // �ؒf����
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

    // �Q�[���X�^�[�g�ʒm����
    public async void OnStart()
    {
        await roomModel.StartAsync();
    }

    // --------------------------------------------------------------
    // ���f���o�^�p�֐�

    // �����ʒm���̏���
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

    // �ޏo�ʒm���̏���
    private void OnExitedUser(JoinedUser user)
    {
        // �ʒu���̍X�V
        if (!characterList.ContainsKey(user.ConnectionId)) return;  // �ޏo�҃I�u�W�F�̑��݃`�F�b�N

        Destroy(characterList[user.ConnectionId]);   // �I�u�W�F�N�g�̔j��
        characterList.Remove(user.ConnectionId);     // ���X�g����폜
    }

    // �v���C���[���ړ������Ƃ��̏���
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
    /// �C���Q�[���ʒm����
    /// </summary>
    private void OnInGameUser()
    {
        // �J�E���g�_�E���J�n
        Debug.Log("�J�E���g�_�E��");
        StartCoroutine("StartCount");
    }

    /// <summary>
    /// �Q�[���X�^�[�g�ʒm����
    /// </summary>
    private void OnStartGameUser()
    {
        // �e�L�X�g�ύX
        timerText.text = "Start!";
        StartCoroutine("HiddenText");
        // ����\�ɂ���
        inputController.GetComponent<TinyCarStandardInput>().carController = playerController.GetComponent<TinyCarController>();
    }

    // �\��UI�̕ύX
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
}
