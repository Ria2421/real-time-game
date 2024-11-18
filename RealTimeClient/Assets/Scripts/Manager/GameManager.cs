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
    /// �ڑ�ID���L�[�ɃL�����N�^�̃I�u�W�F�N�g���Ǘ�
    /// </summary>
    private Dictionary<Guid,GameObject> characterList = new Dictionary<Guid,GameObject>();

    //-------------------------------------------------------
    // ���\�b�h

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // �ڑ�����
    public async void OnConnect()
    {
        int userId = int.Parse(idText.text);    // ���͂������[�U�[ID�̕ϊ�

        // ���[�U�[�����������Ƃ���OnJoinUser���\�b�h�����s����悤�A���f���ɓo�^����B
        roomModel.OnJoinedUser += OnJoinedUser;
        // �ڑ�
        await roomModel.ConnectAsync();
        // ���� (���[�����ƃ��[�U�[ID��n���ē����B�ŏI�I�ɂ̓��[�J���f�[�^�̃��[�U�[ID���g�p)
        await roomModel.JoinAsync("sampleRoom", userId);
    }

    // ���������Ƃ��̏���
    private void OnJoinedUser(JoinedUser user)
    {
        GameObject characterObj = Instantiate(characterPrefab);    // �C���X�^���X�̐���
        characterObj.transform.position = Vector3.zero;
        characterList[user.ConnectionId] = characterObj;    // �t�B�[���h�ŕۑ�
    }
}
