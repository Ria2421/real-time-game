//---------------------------------------------------------------
// ���[����񃂃f�� [ RoomModel.cs ]
// Author:Kenta Nakamoto
// Data:2024/11/18
// Update:2024/11/18
//---------------------------------------------------------------
using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RoomModel : BaseModel,IRoomHubReceiver
{
    //-------------------------------------------------------
    // �t�B�[���h

    private GrpcChannel channel;    // �ڑ����Ɏg�p
    private IRoomHub roomHub;

    /// <summary>
    /// �ڑ�ID
    /// </summary>
    public Guid ConnectionId { get; set; }

    /// <summary>
    /// ���[�U�[�ڑ��ʒm
    /// </summary>
    public Action<JoinedUser> OnJoinedUser {  get; set; }

    /// <summary>
    /// ���[�U�[�ޏo�ʒm
    /// </summary>
    public Action<Guid> OnExitedUser { get; set; }

    /// <summary>
    /// ���[�U�[�ړ��ʒm
    /// </summary>
    public Action<MoveData> OnMovedUser { get; set; }

    //-------------------------------------------------------
    // ���\�b�h

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

    // ��������
    public async Task JoinAsync(string roomName, int userId)
    {
        JoinedUser[] users =await roomHub.JoinAsync(roomName, userId);
        foreach(var user in users)
        {
            if(user.UserData.Id == userId) this.ConnectionId = user.ConnectionId;   // �ڑ�ID�̕ۑ�
            OnJoinedUser(user); // Action��Model���g���N���X�ɒʒm
        }
    }

    // �ޏo����
    public async Task ExitAsync()
    {
        await roomHub.ExitAsync();
    }

    // �ړ�����
    public async Task MoveAsync(MoveData moveData)
    {
        await roomHub.MoveAsync(moveData);
    }

    // �����ʒm (IRoomHubReceiver�C���^�[�t�F�[�X�̎���)
    public void OnJoin(JoinedUser user)
    {
        OnJoinedUser(user);
    }

    // �ޏo�ʒm (IRoomHubReceiver�C���^�[�t�F�[�X�̎���)
    public void OnExit(Guid exitId)
    {
        OnExitedUser(exitId);
    }

    // �ړ��ʒm (IRoomHubReceiver�C���^�[�t�F�[�X�̎���)
    public void OnMove(MoveData moveData)
    {
        OnMovedUser(moveData);
    }
}
