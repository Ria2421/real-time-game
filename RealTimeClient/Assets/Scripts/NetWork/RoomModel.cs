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
    /// �Q���� (PLNo)
    /// </summary>
    public int JoinOrder { get; set; }

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
    public Action<int,string> OnEndGameUser { get; set; }

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
    public async UniTask JoinAsync(string roomName, int userId)
    {
        JoinedUser[] users =await roomHub.JoinAsync(roomName, userId);
        foreach(var user in users)
        {
            if (user.UserData.Id == userId)
            {
                this.ConnectionId = user.ConnectionId;  // �ڑ�ID�̕ۑ�
                this.JoinOrder = user.JoinOrder;        // �Q����(PLNo)�̕ۑ�
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

    public async UniTask GameEndAsync()
    {
        await roomHub.GameEndAsync();
    }

    //==================================================================
    // IRoomHubReceiver�C���^�[�t�F�[�X�̎���

    // �����ʒm
    public void OnJoin(JoinedUser user)
    {
        OnJoinedUser(user);
    }

    // �ޏo�ʒm
    public void OnExit(JoinedUser user)
    {
        OnExitedUser(user);
    }

    // �ړ��ʒm
    public void OnMove(MoveData moveData)
    {
        OnMovedUser(moveData);
    }

    // �C���Q�[���ʒm
    public void OnInGame()
    {
        OnInGameUser();
    }

    // �Q�[���J�n�ʒm
    public void OnStartGame()
    {
        OnStartGameUser();
    }

    // �Q�[���I���ʒm
    public void OnEndGame(int plNo, string name)
    {
        OnEndGameUser(plNo,name);
    }
}