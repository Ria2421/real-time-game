//---------------------------------------------------------------
// ���[�U�[���f�� [ UserModel.cs ]
// Author:Kenta Nakamoto
// Data:2024/11/12
// Update:2025/01/21
//---------------------------------------------------------------
using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using MagicOnion.Client;
using Newtonsoft.Json;
using RealTimeServer.Model.Entity;
using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UserModel : BaseModel
{
    //-------------------------------------------------------
    // �t�B�[���h

    public enum Status
    {
        True = 0,   // ����
        False,      // ���s
        SameName,   // ���O���
    }

    /// <summary>
    /// ���[�U�[ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// �g�[�N��ID
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// �S�[�X�g�f�[�^
    /// </summary>
    public string GhostData { get; set; } = "";

    /// <summary>
    /// get�v���p�e�B���Ăяo�������񎞂ɃC���X�^���X��������static�ŕێ�
    /// </summary>
    private static UserModel instance;

    /// <summary>
    /// NetworkManager�v���p�e�B
    /// </summary>
    public static UserModel Instance
    {
        get
        {
            if (instance == null)
            {
                // GameObject�𐶐����AUserModel��ǉ�
                GameObject gameObject = new GameObject("UserModel");
                instance = gameObject.AddComponent<UserModel>();

                // �V�[���J�ڂŔj������Ȃ��悤�ɐݒ�
                DontDestroyOnLoad(gameObject);
            }

            return instance;
        }
    }

    //-------------------------------------------------------
    // ���\�b�h

    /// <summary>
    /// ���[�U�[�f�[�^�ۑ�����
    /// </summary>
    private void SaveUserData()
    {
        // �Z�[�u�f�[�^�N���X�̐���
        SaveData saveData = new SaveData();
        saveData.UserID = this.UserId;
        saveData.Token = this.Token;

        // �f�[�^��JSON�V���A���C�Y
        string json = JsonConvert.SerializeObject(saveData);

        // �w�肵����΃p�X��"saveData.json"��ۑ�
        var writer = new StreamWriter(Application.persistentDataPath + "/saveData.json");
        writer.Write(json); // �����o��
        writer.Flush();     // �o�b�t�@�Ɏc���Ă���l��S�ď����o��
        writer.Close();     // �t�@�C����
    }

    /// <summary>
    /// ���[�U�[�f�[�^�ǂݍ��ݏ���
    /// </summary>
    /// <returns></returns>
    public bool LoadUserData()
    {
        if (!File.Exists(Application.persistentDataPath + "/saveData.json"))
        {   // �w��̃p�X�̃t�@�C�������݂��Ȃ��������A�������^�[��
            return false;
        }

        //  ���[�J���t�@�C�����烆�[�U�[�f�[�^�̓Ǎ�����
        var reader = new StreamReader(Application.persistentDataPath + "/saveData.json");
        string json = reader.ReadToEnd();
        reader.Close();

        // �Z�[�u�f�[�^JSON���f�V���A���C�Y���Ď擾
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);
        this.UserId = saveData.UserID;
        this.Token = saveData.Token;

        // �ǂݍ��݌��ʂ����^�[��
        return true;
    }

    /// <summary>
    /// ���[�U�[�f�[�^�o�^����
    /// </summary>
    /// <param name="name">���[�U�[��</param>
    /// <returns> [true]���� , [false]���s </returns>
    public async UniTask<Status> RegistUserAsync(string name)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // �n���h���[�̐ݒ�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // �T�[�o�[�Ƃ̃`�����l����ݒ�
        var client = MagicOnionClient.Create<IUserService>(channel);    // �T�[�o�[�Ƃ̐ڑ�

        try
        {
            this.Token = Guid.NewGuid().ToString();                     // �g�[�N������
            this.UserId = await client.RegistUserAsync(name, Token);    // �֐��Ăяo��
            SaveUserData();                                             // ���[�J���ɕۑ�
            return Status.True;
        }catch(RpcException e)
        {
            Debug.Log(e);
            if (e.Status.Detail == "SameName")
            {   // ���O���
                return Status.SameName;
            }
            else
            {   // �ʐM���s
                return Status.False;
            }
        }
    }

    /// <summary>
    /// ���[�U�[��ID�w��Ō���
    /// [return : ���[�U�[���]
    /// </summary>
    /// <param name="id">���[�U�[ID</param>
    /// <returns></returns>
    public async UniTask<User> SearchUserID(int id)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // �n���h���[�̐ݒ�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // �T�[�o�[�Ƃ̃`�����l����ݒ�
        var client = MagicOnionClient.Create<IUserService>(channel);    // �T�[�o�[�Ƃ̐ڑ�

        try
        {
            var userData = await client.SearchUserID(id);    // �֐��Ăяo��
            return userData;
        }
        catch(RpcException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    /// <summary>
    /// �w��ID�̃��[�U�[���X�V
    /// [return : �^�U]
    /// </summary>
    /// <param name="id">  ���[�U�[ID</param>
    /// <param name="name">���[�U�[��</param>
    /// <returns></returns>
    public async UniTask<Status> UpdateUserName(int id, string name)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // �n���h���[�̐ݒ�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // �T�[�o�[�Ƃ̃`�����l����ݒ�
        var client = MagicOnionClient.Create<IUserService>(channel);    // �T�[�o�[�Ƃ̐ڑ�

        try
        {
            var userData = await client.UpdateUserName(id, name);
            return Status.True;
        }
        catch (RpcException e)
        {
            if (e.Status.Detail == "SameName")
            {   // ���O���
                return Status.SameName;
            }
            else
            {   // �ʐM���s
                return Status.False;
            }
        }
    }
}
