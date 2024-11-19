//---------------------------------------------------------------
// �Q�[���}�l�[�W���[ [ GameManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/11/18
// Update:2024/11/18
//---------------------------------------------------------------
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour
{
    //-------------------------------------------------------
    // �t�B�[���h

    /// <summary>
    /// ���[�����f���i�[�p
    /// </summary>
    [SerializeField] private RoomModel roomModel;

    /// <summary>
    /// ���[�U�[ID
    /// </summary>
    [SerializeField] private Text idText;

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
    /// �ڑ�ID���L�[�ɃL�����N�^�̃I�u�W�F�N�g���Ǘ�
    /// </summary>
    private Dictionary<Guid,GameObject> characterList = new Dictionary<Guid,GameObject>();

    /// <summary>
    /// �Q�[���t���O
    /// </summary>
    private bool isGame = false;

    //-------------------------------------------------------
    // ���\�b�h

    // ��������
    void Start()
    {
        // ���[�U�[�����������Ƃ���OnJoinUser���\�b�h�����s����悤�A���f���ɓo�^����B
        roomModel.OnJoinedUser += OnJoinedUser;
        // ���[�U�[���ޏo�����Ƃ���OnExitUser���\�b�h�����s����悤�A���f���ɓo�^����B
        roomModel.OnExitedUser += OnExitedUser;
        // ���[�U�[���ޏo�����Ƃ���OnMoveUser���\�b�h�����s����悤�A���f���ɓo�^����B
        roomModel.OnMovedUser += OnMovedUser;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private async void FixedUpdate()
    {
        if (!isGame) return; 

        var moveData = new MoveData() { ConnectionId = roomModel.ConnectionId,
                                        Position = characterList[roomModel.ConnectionId].transform.position,
                                        Rotation = characterList[roomModel.ConnectionId].transform.eulerAngles  };

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
        // �ޏo
        await roomModel.ExitAsync();
        // �ؒf
        await roomModel.DisconnectionAsync();
        // �v���C���[�I�u�W�F�N�g�̍폜
        foreach(Transform child in parentObj.transform)
        {
            Destroy(child.gameObject);
        }

        isGame = false;
        Debug.Log("�ޏo");
    }

    // ���������Ƃ��̏���
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
            isGame = true;
        }
    }

    // �ޏo�����Ƃ��̏���
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

        characterList[moveData.ConnectionId].gameObject.transform.position = moveData.Position;
        characterList[moveData.ConnectionId].gameObject.transform.eulerAngles = moveData.Rotation; 
    }
}
