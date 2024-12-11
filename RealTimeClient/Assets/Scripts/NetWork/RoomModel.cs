//---------------------------------------------------------------
// ルーム情報モデル [ RoomModel.cs ]
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
    // フィールド

    private GrpcChannel channel;    // 接続時に使用
    private IRoomHub roomHub;
    private int userId;

    /// <summary>
    /// 接続ID
    /// </summary>
    public Guid ConnectionId { get; set; }

    /// <summary>
    /// 参加ルーム名
    /// </summary>
    public string RoomName { get; set; }

    /// <summary>
    /// 参加順 (PLNo)
    /// </summary>
    public int JoinOrder { get; set; }

    /// <summary>
    /// ユーザー名
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// マッチング完了通知
    /// </summary>
    public Action<string> OnMatchingUser { get; set; }

    /// <summary>
    /// ユーザー接続通知
    /// </summary>
    public Action<JoinedUser> OnJoinedUser {  get; set; }

    /// <summary>
    /// ユーザー退出通知
    /// </summary>
    public Action<JoinedUser> OnExitedUser { get; set; }

    /// <summary>
    /// ユーザー移動通知
    /// </summary>
    public Action<MoveData> OnMovedUser { get; set; }

    /// <summary>
    /// インゲーム通知
    /// </summary>
    public Action OnInGameUser { get; set; }

    /// <summary>
    /// ゲーム開始通知
    /// </summary>
    public Action OnStartGameUser { get; set; }

    /// <summary>
    /// ゲーム終了通知
    /// </summary>
    public Action<Dictionary<int,string>> OnEndGameUser { get; set; }

    /// <summary>
    /// ユーザー撃破通知
    /// </summary>
    public Action<string,string,Guid> OnCrushingUser { get; set; }

    //-------------------------------------------------------
    // メソッド
    void Start()
    {
        // roomModelが破棄されないように設定
        DontDestroyOnLoad(this.gameObject);
    }

    // 接続処理
    public async UniTask ConnectAsync()
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true };   // ハンドラーの設定
        channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // サーバーとのチャンネルを設定
        roomHub = await StreamingHubClient.ConnectAsync<IRoomHub, IRoomHubReceiver>(channel, this); // サーバーとの接続
    }

    // 切断処理
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

    // 破棄処理
    async void OnDestroy()
    {
        // 切断
        await DisconnectionAsync();
    }

    // ロビー接続処理
    public async UniTask JoinLobbyAsync(int userId)
    {
        this.userId = userId;   // ユーザーIDの保存
        await roomHub.JoinLobbyAsync(userId);
    }

    // 入室処理
    public async UniTask JoinAsync()
    {
        JoinedUser[] users =await roomHub.JoinAsync(RoomName, userId);
        foreach(var user in users)
        {
            if (user.UserData.Id == userId)
            {
                this.ConnectionId = user.ConnectionId;  // 接続IDの保存
                this.JoinOrder = user.JoinOrder;        // 参加順(PLNo)の保存
                this.UserName = user.UserData.Name;     // ユーザー名の保存
            }
            OnJoinedUser(user); // ActionでModelを使うクラスに通知
        }
    }

    // 退出処理
    public async UniTask ExitAsync()
    {
        await roomHub.ExitAsync();
    }

    // 移動処理
    public async UniTask MoveAsync(MoveData moveData)
    {
        await roomHub.MoveAsync(moveData);
    }

    // ゲーム開始通知処理
    public async UniTask GameStartAsync()
    {
        await roomHub.GameStartAsync();
    }

    // ゲーム終了通知処理
    public async UniTask GameEndAsync()
    {
        await roomHub.GameEndAsync();
    }

    // 撃破通知処理
    public async UniTask CrushingPlayerAsync(string attackName, string cruchName, Guid crushID)
    {
        await roomHub.CrushingPlayerAsync(attackName, cruchName, crushID);
    }

    //==================================================================
    // IRoomHubReceiverインターフェースの実装

    // マッチング完了通知
    public void OnMatching(string roomName)
    {
        OnMatchingUser(roomName);
    }

    // 入室通知
    public void OnJoin(JoinedUser user)
    {
        if (OnJoinedUser == null) return;
        OnJoinedUser(user);
    }

    // 退出通知
    public void OnExit(JoinedUser user)
    {
        if (OnExitedUser == null) return;
        OnExitedUser(user);
    }

    // 移動通知
    public void OnMove(MoveData moveData)
    {
        if (OnMovedUser == null) return;
        OnMovedUser(moveData);
    }

    // インゲーム通知
    public void OnInGame()
    {
        if (OnInGameUser == null) return;
        OnInGameUser();
    }

    // ゲーム開始通知
    public void OnStartGame()
    {
        if (OnStartGameUser == null) return;
        OnStartGameUser();
    }

    // ゲーム終了通知
    public void OnEndGame(Dictionary<int,string> result)
    {
        if (OnEndGameUser == null) return;
        OnEndGameUser(result);
    }

    // 撃破通知処理
    public void OnCrushing(string attackName, string cruchName, Guid crushID)
    {
        if (OnCrushingUser == null) return;
        OnCrushingUser(attackName, cruchName, crushID);
    }
}
