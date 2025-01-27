//---------------------------------------------------------------
// ���[����񃂃f�� [ RoomModel.cs ]
// Author:Kenta Nakamoto
// Data:2024/11/18
// Update:2025/01/27
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

    /// <summary>
    /// ���[�U�[ID
    /// </summary>
    public int UserId { get; set; }

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
    public Action<string,int> OnMatchingUser { get; set; }

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
    public Action<List<ResultData>> OnEndGameUser { get; set; }

    /// <summary>
    /// ���[�U�[���j�ʒm
    /// </summary>
    public Action<string,string,Guid,int> OnCrushingUser { get; set; }

    /// <summary>
    /// �c�^�C���ʒm
    /// </summary>
    public Action<int> OnTimeCountUser { get; set; }

    /// <summary>
    /// �^�C���A�b�v�ʒm
    /// </summary>
    public Action OnTimeUpUser {  get; set; }

    /// <summary>
    ///  ���˒ʒm
    /// </summary>
    public Action<int> OnShotUser { get; set; }

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
        // �ؒf����
        await DisconnectionAsync();
    }

    // ���r�[�ڑ�����
    public async UniTask JoinLobbyAsync(int userId)
    {
        this.UserId = userId;   // ���[�U�[ID�̕ۑ�
        await roomHub.JoinLobbyAsync(userId);
    }

    // ��������
    public async UniTask JoinAsync()
    {
        JoinedUser[] users =await roomHub.JoinAsync(RoomName, UserId);
        foreach(var user in users)
        {
            if (user.UserData.Id == UserId)
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
    public async UniTask CrushingPlayerAsync(string attackName, string cruchName, Guid crushID, int deadNo)
    {
        await roomHub.CrushingPlayerAsync(attackName, cruchName, crushID, deadNo);
    }

    // �c�^�C���ʒm����
    public async UniTask TimeCountAsync(int time)
    {
        await roomHub.TimeCountAsync(time);
    }

    //==================================================================
    // IRoomHubReceiver�C���^�[�t�F�[�X�̎���

    // �}�b�`���O�����ʒm
    public void OnMatching(string roomName, int stageID)
    {
        OnMatchingUser(roomName,stageID);
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
    public void OnEndGame(List<ResultData> result)
    {
        if (OnEndGameUser == null) return;
        OnEndGameUser(result);
    }

    // ���j�ʒm����
    public void OnCrushing(string attackName, string cruchName, Guid crushID, int deadNo)
    {
        if (OnCrushingUser == null) return;
        OnCrushingUser(attackName, cruchName, crushID, deadNo);
    }

    // �c�^�C���ʒm����
    public void OnTimeCount(int time)
    {
        if (OnTimeCountUser == null) return;
        OnTimeCountUser(time);
    }

    // �^�C���A�b�v�ʒm����
    public void OnTimeUp()
    {
        if (OnTimeUpUser == null) return;
        OnTimeUpUser();
    }

    // ���˒ʒm����
    public void OnShot(int cannonID)
    {
        if (OnShotUser == null) return;
        OnShotUser(cannonID);
    }
}
