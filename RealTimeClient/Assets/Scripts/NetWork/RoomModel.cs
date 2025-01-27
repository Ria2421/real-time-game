//---------------------------------------------------------------
// ルーム情報モデル [ RoomModel.cs ]
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
    // フィールド

    private GrpcChannel channel;    // 接続時に使用
    private IRoomHub roomHub;

    /// <summary>
    /// ユーザーID
    /// </summary>
    public int UserId { get; set; }

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
    public Action<string,int> OnMatchingUser { get; set; }

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
    public Action<List<ResultData>> OnEndGameUser { get; set; }

    /// <summary>
    /// ユーザー撃破通知
    /// </summary>
    public Action<string,string,Guid,int> OnCrushingUser { get; set; }

    /// <summary>
    /// 残タイム通知
    /// </summary>
    public Action<int> OnTimeCountUser { get; set; }

    /// <summary>
    /// タイムアップ通知
    /// </summary>
    public Action OnTimeUpUser {  get; set; }

    /// <summary>
    ///  発射通知
    /// </summary>
    public Action<int> OnShotUser { get; set; }

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
        // 切断処理
        await DisconnectionAsync();
    }

    // ロビー接続処理
    public async UniTask JoinLobbyAsync(int userId)
    {
        this.UserId = userId;   // ユーザーIDの保存
        await roomHub.JoinLobbyAsync(userId);
    }

    // 入室処理
    public async UniTask JoinAsync()
    {
        JoinedUser[] users =await roomHub.JoinAsync(RoomName, UserId);
        foreach(var user in users)
        {
            if (user.UserData.Id == UserId)
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
    public async UniTask CrushingPlayerAsync(string attackName, string cruchName, Guid crushID, int deadNo)
    {
        await roomHub.CrushingPlayerAsync(attackName, cruchName, crushID, deadNo);
    }

    // 残タイム通知処理
    public async UniTask TimeCountAsync(int time)
    {
        await roomHub.TimeCountAsync(time);
    }

    //==================================================================
    // IRoomHubReceiverインターフェースの実装

    // マッチング完了通知
    public void OnMatching(string roomName, int stageID)
    {
        OnMatchingUser(roomName,stageID);
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
    public void OnEndGame(List<ResultData> result)
    {
        if (OnEndGameUser == null) return;
        OnEndGameUser(result);
    }

    // 撃破通知処理
    public void OnCrushing(string attackName, string cruchName, Guid crushID, int deadNo)
    {
        if (OnCrushingUser == null) return;
        OnCrushingUser(attackName, cruchName, crushID, deadNo);
    }

    // 残タイム通知処理
    public void OnTimeCount(int time)
    {
        if (OnTimeCountUser == null) return;
        OnTimeCountUser(time);
    }

    // タイムアップ通知処理
    public void OnTimeUp()
    {
        if (OnTimeUpUser == null) return;
        OnTimeUpUser();
    }

    // 発射通知処理
    public void OnShot(int cannonID)
    {
        if (OnShotUser == null) return;
        OnShotUser(cannonID);
    }
}
