//---------------------------------------------------------------
// ���[����񃂃f�� [ RoomModel.cs ]
// Author:Kenta Nakamoto
// Data:2024/11/18
// Update:2024/12/05
//---------------------------------------------------------------
using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomModel : BaseModel,IRoomHubReceiver
{
    //-------------------------------------------------------
    // �t�B�[���h

    private GrpcChannel channel;    // �ڑ����Ɏg�p
    private IRoomHub roomHub;
    private int userId;

    /// <summary>
    /// �ڑ�ID
    /// </summary>
    public Guid ConnectionId { get; set; }

    /// <summary>
    /// �Q�����[����
    /// </summary>
    public string RoomName { get; set; }

    /// <summary>
    /// �Q���� (PLNo)
    /// </summary>
    public int JoinOrder { get; set; }

    /// <summary>
    /// ���[�U�[��
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// �}�b�`���O�����ʒm
    /// </summary>
    public Action<string> OnMatchingUser { get; set; }

    /// <summary>
    /// ���[�U�[�ڑ��ʒm
    /// </summary>
    public Action<JoinedUser> OnJoinedUser {  get; set; }

    /// <summary>
    /// ���[�U�[�ޏo�ʒm
    /// </summary>
    public Action<JoinedUser> OnExitedUser { get; set; }

    /// <summary>
    /// ���[�U�[�ړ��ʒm
    /// </summary>
    public Action<MoveData> OnMovedUser { get; set; }

    /// <summary>
    /// �C���Q�[���ʒm
    /// </summary>
    public Action OnInGameUser { get; set; }

    /// <summary>
    /// �Q�[���J�n�ʒm
    /// </summary>
    public Action OnStartGameUser { get; set; }

    /// <summary>
    /// �Q�[���I���ʒm
    /// </summary>
    public Action<Dictionary<int,string>> OnEndGameUser { get; set; }

    /// <summary>
    /// ���[�U�[���j�ʒm
    /// </summary>
    public Action<string,string,Guid> OnCrushingUser { get; set; }

    //-------------------------------------------------------
    // ���\�b�h
    void Start()
    {
        // roomModel���j������Ȃ��悤�ɐݒ�
        DontDestroyOnLoad(this.gameObject);
    }

    // �ڑ�����
    public async UniTask ConnectAsync()
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true };   // �n���h���[�̐ݒ�
        channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // �T�[�o�[�Ƃ̃`�����l����ݒ�
        roomHub = await StreamingHubClient.ConnectAsync<IRoomHub, IRoomHubReceiver>(channel, this); // �T�[�o�[�Ƃ̐ڑ�
    }

    // �ؒf����
    public async UniTask DisconnectionAsync()
    {
        if (roomHub != null)
        {
            await roomHub.ExitAsync();
            await roomHub.DisposeAsync();
        }
        if(channel != null) await channel.ShutdownAsync();
        roomHub = null; channel = null;
    }

    // �j������
    async void OnDestroy()
    {
        // �ؒf
        await DisconnectionAsync();
    }

    // ���r�[�ڑ�����
    public async UniTask JoinLobbyAsync(int userId)
    {
        this.userId = userId;   // ���[�U�[ID�̕ۑ�
        await roomHub.JoinLobbyAsync(userId);
    }

    // ��������
    public async UniTask JoinAsync()
    {
        JoinedUser[] users =await roomHub.JoinAsync(RoomName, userId);
        foreach(var user in users)
        {
            if (user.UserData.Id == userId)
            {
                this.ConnectionId = user.ConnectionId;  // �ڑ�ID�̕ۑ�
                this.JoinOrder = user.JoinOrder;        // �Q����(PLNo)�̕ۑ�
                this.UserName = user.UserData.Name;     // ���[�U�[���̕ۑ�
            }
            OnJoinedUser(user); // Action��Model���g���N���X�ɒʒm
        }
    }

    // �ޏo����
    public async UniTask ExitAsync()
    {
        await roomHub.ExitAsync();
    }

    // �ړ�����
    public async UniTask MoveAsync(MoveData moveData)
    {
        await roomHub.MoveAsync(moveData);
    }

    // �Q�[���J�n�ʒm����
    public async UniTask GameStartAsync()
    {
        await roomHub.GameStartAsync();
    }

    // �Q�[���I���ʒm����
    public async UniTask GameEndAsync()
    {
        await roomHub.GameEndAsync();
    }

    // ���j�ʒm����
    public async UniTask CrushingPlayerAsync(string attackName, string cruchName, Guid crushID)
    {
        await roomHub.CrushingPlayerAsync(attackName, cruchName, crushID);
    }

    //==================================================================
    // IRoomHubReceiver�C���^�[�t�F�[�X�̎���

    // �}�b�`���O�����ʒm
    public void OnMatching(string roomName)
    {
        OnMatchingUser(roomName);
    }

    // �����ʒm
    public void OnJoin(JoinedUser user)
    {
        if (OnJoinedUser == null) return;
        OnJoinedUser(user);
    }

    // �ޏo�ʒm
    public void OnExit(JoinedUser user)
    {
        if (OnExitedUser == null) return;
        OnExitedUser(user);
    }

    // �ړ��ʒm
    public void OnMove(MoveData moveData)
    {
        if (OnMovedUser == null) return;
        OnMovedUser(moveData);
    }

    // �C���Q�[���ʒm
    public void OnInGame()
    {
        if (OnInGameUser == null) return;
        OnInGameUser();
    }

    // �Q�[���J�n�ʒm
    public void OnStartGame()
    {
        if (OnStartGameUser == null) return;
        OnStartGameUser();
    }

    // �Q�[���I���ʒm
    public void OnEndGame(Dictionary<int,string> result)
    {
        if (OnEndGameUser == null) return;
        OnEndGameUser(result);
    }

    // ���j�ʒm����
    public void OnCrushing(string attackName, string cruchName, Guid crushID)
    {
        if (OnCrushingUser == null) return;
        OnCrushingUser(attackName, cruchName, crushID);
    }
}
