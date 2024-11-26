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
        Ready,
    }

    /// <summary>
    /// �ڑ�ID���L�[�ɃL�����N�^�̃I�u�W�F�N�g���Ǘ�
    /// </summary>
    private Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    /// <summary>
    /// �Q�[�����
    /// </summary>
    private GameState gameState = GameState.None;

    [Header("�e��Object���A�^�b�`")]

    /// <summary>
    /// ���[�����f���i�[�p
    /// </summary>
    [SerializeField] private RoomModel roomModel;

    /// <summary>
    /// ��������L�����N�^�[�v���n�u
    /// </summary>
    [SerializeField] private GameObject characterPrefab;

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

    [Space (40)]
    [Header("===== UI�֘A =====")]

    [Space(10)]
    [Header("---- Text ----")]

    /// <summary>
    /// ���[�U�[ID
    /// </summary>
    [SerializeField] private Text idText;

    /// <summary>
    /// ������ԕ\���e�L�X�g
    /// </summary>
    [SerializeField] private Text[] readyStateTexts;

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

    /// <summary>
    /// ���������{�^��
    /// </summary>
    [SerializeField] private Button readyButton;

    /// <summary>
    /// �����L�����Z���{�^��
    /// </summary>
    [SerializeField] private Button nonReadyButton;

    //=====================================
    // ���\�b�h

    // ��������
    void Start()
    {
        // �e�ʒm���͂����ۂɍs�����������f���ɓo�^����
        roomModel.OnJoinedUser += OnJoinedUser;     // ����
        roomModel.OnReadyUser += OnReadyUser;       // ��������
        roomModel.OnNonReadyUser += OnNonReadyUser; // �����L�����Z��
        roomModel.OnExitedUser += OnExitedUser;     // �ގ�
        roomModel.OnMovedUser += OnMovedUser;       // �ړ�

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

        var moveData = new MoveData()
        {
            ConnectionId = roomModel.ConnectionId,
            Position = characterList[roomModel.ConnectionId].transform.position,
            Rotation = characterList[roomModel.ConnectionId].transform.eulerAngles
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

    // ������������
    public async void OnReady()
    {
        await roomModel.ReadyAsync();

        gameState = GameState.Ready;
        ChangeUI(gameState);
    }

    // �����L�����Z������
    public async void OnCancel()
    {
        await roomModel.NonReadyAsync();

        gameState = GameState.Join;
        ChangeUI(gameState);
    }

    // �ؒf����
    public async void OnDisconnect()
    {
        CancelInvoke();

        // �ޏo
        await roomModel.ExitAsync();
        // �ؒf
        await roomModel.DisconnectionAsync();
        // �v���C���[�I�u�W�F�N�g�̍폜
        foreach(Transform child in parentObj.transform)
        {
            Destroy(child.gameObject);
        }

        gameState = GameState.None;
        ChangeUI(gameState);
        Debug.Log("�ޏo");
    }

    // --------------------------------------------------------------
    // ���f���o�^�p�֐�

    // �����ʒm���̏���
    private void OnJoinedUser(JoinedUser user)
    {
        Debug.Log(user.JoinOrder + "P");

        // �v���C���[�̐���
        GameObject characterObj = Instantiate(characterPrefab, respownList[user.JoinOrder-1].position, Quaternion.Euler(0,180,0));

        characterObj.transform.parent = parentObj.transform;    // �e�̐ݒ�
        characterList[user.ConnectionId] = characterObj;        // �t�B�[���h�ŕۑ�

        if(user.ConnectionId == roomModel.ConnectionId)
        {
            characterList[roomModel.ConnectionId].gameObject.AddComponent<PlayerManager>();
            gameState = GameState.Join;
            ChangeUI(gameState);
            InvokeRepeating("SendMoveData", 0, internetSpeed);
        }
    }


    // ���������ʒm���̏���
    private void OnReadyUser(JoinedUser user)
    {
        readyStateTexts[user.JoinOrder - 1].text = "���������I";
    }

    // �����L�����Z���ʒm���̏���
    private void OnNonReadyUser(JoinedUser user)
    {
        readyStateTexts[user.JoinOrder - 1].text = "������...";
    }

    // �ޏo�ʒm���̏���
    private void OnExitedUser(Guid connectionId)
    {
        // �ʒu���̍X�V
        if (!characterList.ContainsKey(connectionId)) return;

        Destroy(characterList[connectionId]);   // �I�u�W�F�N�g�̔j��
        characterList.Remove(connectionId);     // ���X�g����폜
    }

    // �v���C���[���ړ������Ƃ��̏���
    private void OnMovedUser(MoveData moveData)
    {
        // �ʒu���̍X�V
        if (!characterList.ContainsKey(moveData.ConnectionId)) return;

        characterList[moveData.ConnectionId].gameObject.transform.DOMove(moveData.Position, internetSpeed).SetEase(Ease.Linear);
        characterList[moveData.ConnectionId].gameObject.transform.DORotate(moveData.Rotation, internetSpeed).SetEase(Ease.Linear);
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

                // Text
                for (int i = 0;readyStateTexts.Length > i; i++)
                {
                    readyStateTexts[i].gameObject.SetActive(false);
                }

                // Button
                joinButton.gameObject.SetActive(true);
                exitButton.gameObject.SetActive(false);
                readyButton.gameObject.SetActive(false);
                nonReadyButton.gameObject.SetActive(false);
                break;

            // ������� ---------------------------------------------
            case GameState.Join:
                // InputField
                idInput.enabled = false;

                // Text
                for (int i = 0; readyStateTexts.Length > i; i++)
                {
                    readyStateTexts[i].gameObject.SetActive(true);
                }

                // Button
                joinButton.gameObject.SetActive(false);
                exitButton.gameObject.SetActive(true);
                readyButton.gameObject.SetActive(true);
                nonReadyButton.gameObject.SetActive(false);
                break;

            // ����������� -----------------------------------------
            case GameState.Ready:
                // InputField
                idInput.enabled = false;

                // Text
                for (int i = 0; readyStateTexts.Length > i; i++)
                {
                    readyStateTexts[i].gameObject.SetActive(true);
                }

                // Button
                joinButton.gameObject.SetActive(false);
                exitButton.gameObject.SetActive(true);
                readyButton.gameObject.SetActive(false);
                nonReadyButton.gameObject.SetActive(true);
                break;
        }
    }
}
