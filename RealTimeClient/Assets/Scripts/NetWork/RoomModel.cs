//---------------------------------------------------------------
// 繝ｫ繝ｼ繝�諠��ｱ繝｢繝��Ν [ RoomModel.cs ]
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
    // 繝輔ぅ繝ｼ繝ｫ繝�

    private GrpcChannel channel;    // 謗･邯壽凾縺ｫ菴ｿ逕ｨ
    private IRoomHub roomHub;

    /// <summary>
    /// 繝ｦ繝ｼ繧ｶ繝ｼID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 謗･邯唔D
    /// </summary>
    public Guid ConnectionId { get; set; }

    /// <summary>
    /// 蜿ょ刈繝ｫ繝ｼ繝�蜷�
    /// </summary>
    public string RoomName { get; set; }

    /// <summary>
    /// 蜿ょ刈鬆� (PLNo)
    /// </summary>
    public int JoinOrder { get; set; }

    /// <summary>
    /// 繝ｦ繝ｼ繧ｶ繝ｼ蜷�
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 繝槭ャ繝√Φ繧ｰ螳御ｺ��夂衍
    /// </summary>
    public Action<string,int> OnMatchingUser { get; set; }

    /// <summary>
    /// 繝ｦ繝ｼ繧ｶ繝ｼ謗･邯夐�夂衍
    /// </summary>
    public Action<JoinedUser> OnJoinedUser {  get; set; }

    /// <summary>
    /// 繝ｦ繝ｼ繧ｶ繝ｼ騾�蜃ｺ騾夂衍
    /// </summary>
    public Action<JoinedUser> OnExitedUser { get; set; }

    /// <summary>
    /// 繝ｦ繝ｼ繧ｶ繝ｼ遘ｻ蜍暮�夂衍
    /// </summary>
    public Action<MoveData> OnMovedUser { get; set; }

    /// <summary>
    /// 繧､繝ｳ繧ｲ繝ｼ繝�騾夂衍
    /// </summary>
    public Action OnInGameUser { get; set; }

    /// <summary>
    /// 繧ｲ繝ｼ繝�髢句ｧ矩�夂衍
    /// </summary>
    public Action OnStartGameUser { get; set; }

    /// <summary>
    /// 繧ｲ繝ｼ繝�邨ゆｺ��夂衍
    /// </summary>
    public Action<List<ResultData>> OnEndGameUser { get; set; }

    /// <summary>
    /// 繝ｦ繝ｼ繧ｶ繝ｼ謦��ｴ騾夂衍
    /// </summary>
    public Action<string,string,Guid,int> OnCrushingUser { get; set; }

    /// <summary>
    /// 谿九ち繧､繝�騾夂衍
    /// </summary>
    public Action<int> OnTimeCountUser { get; set; }

    /// <summary>
    /// 繧ｿ繧､繝�繧｢繝�プ騾夂衍
    /// </summary>
    public Action OnTimeUpUser {  get; set; }

    /// <summary>
    ///  逋ｺ蟆��夂衍
    /// </summary>
    public Action<int> OnShotUser { get; set; }

    //-------------------------------------------------------
    // 繝｡繧ｽ繝��ラ
    void Start()
    {
        // roomModel縺檎�ｴ譽��＆繧後↑縺��ｈ縺��↓險ｭ螳�
        DontDestroyOnLoad(this.gameObject);
    }

    // 謗･邯壼��逅�
    public async UniTask ConnectAsync()
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true };   // 繝上Φ繝峨Λ繝ｼ縺ｮ險ｭ螳�
        channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ繝√Ε繝ｳ繝阪Ν繧定ｨｭ螳�
        roomHub = await StreamingHubClient.ConnectAsync<IRoomHub, IRoomHubReceiver>(channel, this); // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ謗･邯�
    }

    // 蛻��妙蜃ｦ逅�
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

    // 遐ｴ譽�処逅�
    async void OnDestroy()
    {
        // 蛻��妙蜃ｦ逅�
        await DisconnectionAsync();
    }

    // 繝ｭ繝薙��謗･邯壼��逅�
    public async UniTask JoinLobbyAsync(int userId)
    {
        this.UserId = userId;   // 繝ｦ繝ｼ繧ｶ繝ｼID縺ｮ菫晏ｭ�
        await roomHub.JoinLobbyAsync(userId);
    }

    // 蜈･螳､蜃ｦ逅�
    public async UniTask JoinAsync()
    {
        JoinedUser[] users =await roomHub.JoinAsync(RoomName, UserId);
        foreach(var user in users)
        {
            if (user.UserData.Id == UserId)
            {
                this.ConnectionId = user.ConnectionId;  // 謗･邯唔D縺ｮ菫晏ｭ�
                this.JoinOrder = user.JoinOrder;        // 蜿ょ刈鬆�(PLNo)縺ｮ菫晏ｭ�
                this.UserName = user.UserData.Name;     // 繝ｦ繝ｼ繧ｶ繝ｼ蜷阪��菫晏ｭ�
            }
            OnJoinedUser(user); // Action縺ｧModel繧剃ｽｿ縺��け繝ｩ繧ｹ縺ｫ騾夂衍
        }
    }

    // 騾�蜃ｺ蜃ｦ逅�
    public async UniTask ExitAsync()
    {
        await roomHub.ExitAsync();
    }

    // 遘ｻ蜍募��逅�
    public async UniTask MoveAsync(MoveData moveData)
    {
        await roomHub.MoveAsync(moveData);
    }

    // 繧ｲ繝ｼ繝�髢句ｧ矩�夂衍蜃ｦ逅�
    public async UniTask GameStartAsync()
    {
        await roomHub.GameStartAsync();
    }

    // 繧ｲ繝ｼ繝�邨ゆｺ��夂衍蜃ｦ逅�
    public async UniTask GameEndAsync()
    {
        await roomHub.GameEndAsync();
    }

    // 謦��ｴ騾夂衍蜃ｦ逅�
    public async UniTask CrushingPlayerAsync(string attackName, string cruchName, Guid crushID, int deadNo)
    {
        await roomHub.CrushingPlayerAsync(attackName, cruchName, crushID, deadNo);
    }

    // 谿九ち繧､繝�騾夂衍蜃ｦ逅�
    public async UniTask TimeCountAsync(int time)
    {
        await roomHub.TimeCountAsync(time);
    }

    // 螟ｧ遐ｲ蟆�出蜃ｦ逅�
    public async UniTask ShotCannonAsync()
    { 
        await roomHub.ShotCannonAsync();
    }

    //==================================================================
    // IRoomHubReceiver繧､繝ｳ繧ｿ繝ｼ繝輔ぉ繝ｼ繧ｹ縺ｮ螳溯｣�

    // 繝槭ャ繝√Φ繧ｰ螳御ｺ��夂衍
    public void OnMatching(string roomName, int stageID)
    {
        OnMatchingUser(roomName,stageID);
    }

    // 蜈･螳､騾夂衍
    public void OnJoin(JoinedUser user)
    {
        if (OnJoinedUser == null) return;
        OnJoinedUser(user);
    }

    // 騾�蜃ｺ騾夂衍
    public void OnExit(JoinedUser user)
    {
        if (OnExitedUser == null) return;
        OnExitedUser(user);
    }

    // 遘ｻ蜍暮�夂衍
    public void OnMove(MoveData moveData)
    {
        if (OnMovedUser == null) return;
        OnMovedUser(moveData);
    }

    // 繧､繝ｳ繧ｲ繝ｼ繝�騾夂衍
    public void OnInGame()
    {
        if (OnInGameUser == null) return;
        OnInGameUser();
    }

    // 繧ｲ繝ｼ繝�髢句ｧ矩�夂衍
    public void OnStartGame()
    {
        if (OnStartGameUser == null) return;
        OnStartGameUser();
    }

    // 繧ｲ繝ｼ繝�邨ゆｺ��夂衍
    public void OnEndGame(List<ResultData> result)
    {
        if (OnEndGameUser == null) return;
        OnEndGameUser(result);
    }

    // 謦��ｴ騾夂衍蜃ｦ逅�
    public void OnCrushing(string attackName, string cruchName, Guid crushID, int deadNo)
    {
        if (OnCrushingUser == null) return;
        OnCrushingUser(attackName, cruchName, crushID, deadNo);
    }

    // 谿九ち繧､繝�騾夂衍蜃ｦ逅�
    public void OnTimeCount(int time)
    {
        if (OnTimeCountUser == null) return;
        OnTimeCountUser(time);
    }

    // 繧ｿ繧､繝�繧｢繝�プ騾夂衍蜃ｦ逅�
    public void OnTimeUp()
    {
        if (OnTimeUpUser == null) return;
        OnTimeUpUser();
    }

    // 逋ｺ蟆��夂衍蜃ｦ逅�
    public void OnShot(int cannonID)
    {
        if (OnShotUser == null) return;
        OnShotUser(cannonID);
    }
}
