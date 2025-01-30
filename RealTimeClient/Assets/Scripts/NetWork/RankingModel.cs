//---------------------------------------------------------------
// 繝ｩ繝ｳ繧ｭ繝ｳ繧ｰ繝｢繝��Ν [ RankingModel.cs ]
// Author:Kenta Nakamoto
// Data:2025/01/12
// Update:2025/01/21
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
    // 繝｡繧ｽ繝��ラ

    /// <summary>
    /// 謖��ｮ壹せ繝�ー繧ｸ繝ｩ繝ｳ繧ｭ繝ｳ繧ｰ縺ｮ蜿門ｾ�
    /// </summary>
    /// <param name="stageID">繧ｹ繝�ー繧ｸID</param>
    /// <returns></returns>
    public async UniTask<List<RankingData>> GetRankingAsync(int stageID)
    {
        List<RankingData> result = new List<RankingData>();

        using var handler = new YetAnotherHttpHandler() { Http2Only = true };                                   // 繝上Φ繝峨Λ繝ｼ縺ｮ險ｭ螳�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ繝√Ε繝ｳ繝阪Ν繧定ｨｭ螳�
        var client = MagicOnionClient.Create<ISoloService>(channel);                                            // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ謗･邯�

        try
        {
            result = await client.GetRankingAsync(stageID);     // 繝ｩ繝ｳ繧ｭ繝ｳ繧ｰ蜿門ｾ�
            return result;
        }
        catch (RpcException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    /// <summary>
    /// 繧ｿ繧､繝�逋ｻ骭ｲ
    /// [return:謌仙凄]
    /// </summary>
    /// <param name="stageID">繝励Ξ繧､縺励◆繧ｹ繝�ー繧ｸNo</param>
    /// <param name="userID"> 繝ｦ繝ｼ繧ｶ繝ｼID</param>
    /// <param name="time">   逋ｻ骭ｲ繧ｿ繧､繝�</param>
    /// <returns></returns>
    public async UniTask<RegistResult> RegistClearTimeAsync(int stageID, int userID, int time, string ghostData)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };                                   // 繝上Φ繝峨Λ繝ｼ繧定ｨｭ螳�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ繝√Ε繝ｳ繝阪Ν繧定ｨｭ螳�
        var client = MagicOnionClient.Create<ISoloService>(channel);

        RegistResult result = new RegistResult();

        try
        {
            result = await client.RegistClearTimeAsync(stageID,userID,time,ghostData);     // 險倬鹸逋ｻ骭ｲ (30遘偵��險倬鹸縺ｧ15KB遞�)
            Debug.Log("繧ｯ繝ｪ繧｢繝�ー繧ｿ逋ｻ骭ｲ蜃ｦ逅�");
            return result;
        }
        catch (RpcException e)
        {
            Debug.Log(e);
            return result;
        }
    }
}
