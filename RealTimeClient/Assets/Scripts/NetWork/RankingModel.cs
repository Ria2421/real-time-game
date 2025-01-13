//---------------------------------------------------------------
// ランキングモデル [ RankingModel.cs ]
// Author:Kenta Nakamoto
// Data:2025/01/12
// Update:2025/01/12
//---------------------------------------------------------------
using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces.Services;
using Shared.Model.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingModel : BaseModel
{
    //-------------------------------------------------------
    // メソッド

    /// <summary>
    /// 指定ステージランキングの取得
    /// </summary>
    /// <param name="stageID">ステージID</param>
    /// <returns></returns>
    public async UniTask<List<RankingData>> GetRankingAsync(int stageID)
    {
        List<RankingData> result = new List<RankingData>();

        using var handler = new YetAnotherHttpHandler() { Http2Only = true };                                   // ハンドラーの設定
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // サーバーとのチャンネルを設定
        var client = MagicOnionClient.Create<ISoloService>(channel);                                            // サーバーとの接続

        try
        {
            result = await client.GetRankingAsync(stageID);     // ランキング取得
            return result;
        }
        catch (RpcException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    /// <summary>
    /// タイム登録
    /// [return:成否]
    /// </summary>
    /// <param name="stageID">プレイしたステージNo</param>
    /// <param name="userID"> ユーザーID</param>
    /// <param name="time">   登録タイム</param>
    /// <returns></returns>
    public async UniTask<bool> RegistClearTimeAsync(int stageID, int userID, int time)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };                                   // ハンドラーの設定
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // サーバーとのチャンネルを設定
        var client = MagicOnionClient.Create<ISoloService>(channel);

        try
        {
            bool isRegist = await client.RegistClearTimeAsync(stageID,userID,time);     // 記録登録
            Debug.Log("タイム登録処理");
            return isRegist;
        }
        catch (RpcException e)
        {
            Debug.Log(e);
            return false;
        }
    }
}
