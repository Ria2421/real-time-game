//---------------------------------------------------------------
// 繝ｦ繝ｼ繧ｶ繝ｼ繝｢繝��Ν [ UserModel.cs ]
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
    // 繝輔ぅ繝ｼ繝ｫ繝�

    public enum Status
    {
        True = 0,   // 謌仙粥
        False,      // 螟ｱ謨�
        SameName,   // 蜷榊燕陲ｫ繧�
        NGWord      // NG繝ｯ繝ｼ繝�
    }

    /// <summary>
    /// 繝ｦ繝ｼ繧ｶ繝ｼID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 繝医��繧ｯ繝ｳID
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// 繝√Η繝ｼ繝医Μ繧｢繝ｫ繝輔Λ繧ｰ
    /// </summary>
    public bool TutorialFlag { get; set; }

    /// <summary>
    /// 繧ｴ繝ｼ繧ｹ繝医ョ繝ｼ繧ｿ
    /// </summary>
    public string GhostData { get; set; } = "";

    /// <summary>
    /// get繝励Ο繝代ユ繧｣繧貞他縺ｳ蜃ｺ縺励◆蛻晏屓譎ゅ↓繧､繝ｳ繧ｹ繧ｿ繝ｳ繧ｹ逕滓��縺励※static縺ｧ菫晄戟
    /// </summary>
    private static UserModel instance;

    /// <summary>
    /// NetworkManager繝励Ο繝代ユ繧｣
    /// </summary>
    public static UserModel Instance
    {
        get
        {
            if (instance == null)
            {
                // GameObject繧堤函謌舌＠縲ゞserModel繧定ｿｽ蜉�
                GameObject gameObject = new GameObject("UserModel");
                instance = gameObject.AddComponent<UserModel>();

                // 繧ｷ繝ｼ繝ｳ驕ｷ遘ｻ縺ｧ遐ｴ譽��＆繧後↑縺��ｈ縺��↓險ｭ螳�
                DontDestroyOnLoad(gameObject);
            }

            return instance;
        }
    }

    //-------------------------------------------------------
    // 繝｡繧ｽ繝��ラ

    /// <summary>
    /// 繝ｦ繝ｼ繧ｶ繝ｼ繝�ー繧ｿ菫晏ｭ伜��逅�
    /// </summary>
    public void SaveUserData()
    {
        // 繧ｻ繝ｼ繝悶ョ繝ｼ繧ｿ繧ｯ繝ｩ繧ｹ縺ｮ逕滓��
        SaveData saveData = new SaveData();
        saveData.UserID = this.UserId;
        saveData.Token = this.Token;
        saveData.TutorialFlag = this.TutorialFlag;

        // 繝�ー繧ｿ繧谷SON繧ｷ繝ｪ繧｢繝ｩ繧､繧ｺ
        string json = JsonConvert.SerializeObject(saveData);

        // 謖��ｮ壹＠縺溽ｵｶ蟇ｾ繝代せ縺ｫ"saveData.json"繧剃ｿ晏ｭ�
        var writer = new StreamWriter(Application.persistentDataPath + "/saveData.json");
        writer.Write(json); // 譖ｸ縺榊��縺�
        writer.Flush();     // 繝舌ャ繝輔ぃ縺ｫ谿九▲縺ｦ縺��ｋ蛟､繧貞��縺ｦ譖ｸ縺榊��縺�
        writer.Close();     // 繝輔ぃ繧､繝ｫ髢�
    }

    /// <summary>
    /// 繝ｦ繝ｼ繧ｶ繝ｼ繝�ー繧ｿ隱ｭ縺ｿ霎ｼ縺ｿ蜃ｦ逅�
    /// </summary>
    /// <returns></returns>
    public bool LoadUserData()
    {
        if (!File.Exists(Application.persistentDataPath + "/saveData.json"))
        {   // 謖��ｮ壹��繝代せ縺ｮ繝輔ぃ繧､繝ｫ縺悟ｭ伜惠縺励↑縺九▲縺滓凾縲∵掠譛溘Μ繧ｿ繝ｼ繝ｳ
            return false;
        }

        //  繝ｭ繝ｼ繧ｫ繝ｫ繝輔ぃ繧､繝ｫ縺九ｉ繝ｦ繝ｼ繧ｶ繝ｼ繝�ー繧ｿ縺ｮ隱ｭ霎ｼ蜃ｦ逅�
        var reader = new StreamReader(Application.persistentDataPath + "/saveData.json");
        string json = reader.ReadToEnd();
        reader.Close();

        // 繧ｻ繝ｼ繝悶ョ繝ｼ繧ｿJSON繧偵ョ繧ｷ繝ｪ繧｢繝ｩ繧､繧ｺ縺励※蜿門ｾ�
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);
        this.UserId = saveData.UserID;
        this.Token = saveData.Token;
        this.TutorialFlag = saveData.TutorialFlag;

        // 隱ｭ縺ｿ霎ｼ縺ｿ邨先棡繧偵Μ繧ｿ繝ｼ繝ｳ
        return true;
    }

    /// <summary>
    /// 繝ｦ繝ｼ繧ｶ繝ｼ繝�ー繧ｿ逋ｻ骭ｲ蜃ｦ逅�
    /// </summary>
    /// <param name="name">繝ｦ繝ｼ繧ｶ繝ｼ蜷�</param>
    /// <returns> [true]謌仙粥 , [false]螟ｱ謨� </returns>
    public async UniTask<Status> RegistUserAsync(string name)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // 繝上Φ繝峨Λ繝ｼ縺ｮ險ｭ螳�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ繝√Ε繝ｳ繝阪Ν繧定ｨｭ螳�
        var client = MagicOnionClient.Create<IUserService>(channel);    // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ謗･邯�

        try
        {
            this.Token = Guid.NewGuid().ToString();                     // 繝医��繧ｯ繝ｳ逕滓��
            this.UserId = await client.RegistUserAsync(name, Token);    // 髢｢謨ｰ蜻ｼ縺ｳ蜃ｺ縺�
            SaveUserData();                                             // 繝ｭ繝ｼ繧ｫ繝ｫ縺ｫ菫晏ｭ�
            return Status.True;
        }catch(RpcException e)
        {
            Debug.Log(e);
            if (e.Status.Detail == "SameName")
            {   // 蜷榊燕陲ｫ繧�
                return Status.SameName;
            }else if(e.Status.Detail == "NGWord")
            {   // NG繝ｯ繝ｼ繝�
                return Status.NGWord;
            }
            else
            {   // 騾壻ｿ｡螟ｱ謨�
                return Status.False;
            }
        }
    }

    /// <summary>
    /// 繝ｦ繝ｼ繧ｶ繝ｼ繧棚D謖��ｮ壹〒讀懃ｴ｢
    /// [return : 繝ｦ繝ｼ繧ｶ繝ｼ諠��ｱ]
    /// </summary>
    /// <param name="id">繝ｦ繝ｼ繧ｶ繝ｼID</param>
    /// <returns></returns>
    public async UniTask<User> SearchUserID(int id)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // 繝上Φ繝峨Λ繝ｼ縺ｮ險ｭ螳�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ繝√Ε繝ｳ繝阪Ν繧定ｨｭ螳�
        var client = MagicOnionClient.Create<IUserService>(channel);    // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ謗･邯�

        try
        {
            var userData = await client.SearchUserID(id);    // 髢｢謨ｰ蜻ｼ縺ｳ蜃ｺ縺�
            return userData;
        }
        catch(RpcException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    /// <summary>
    /// 謖��ｮ唔D縺ｮ繝ｦ繝ｼ繧ｶ繝ｼ蜷肴峩譁ｰ
    /// [return : 逵溷⊃]
    /// </summary>
    /// <param name="id">  繝ｦ繝ｼ繧ｶ繝ｼID</param>
    /// <param name="name">繝ｦ繝ｼ繧ｶ繝ｼ蜷�</param>
    /// <returns></returns>
    public async UniTask<Status> UpdateUserName(int id, string name)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // 繝上Φ繝峨Λ繝ｼ縺ｮ險ｭ螳�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ繝√Ε繝ｳ繝阪Ν繧定ｨｭ螳�
        var client = MagicOnionClient.Create<IUserService>(channel);    // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ謗･邯�

        try
        {
            var userData = await client.UpdateUserName(id, name);
            return Status.True;
        }
        catch (RpcException e)
        {
            if (e.Status.Detail == "SameName")
            {   // 蜷榊燕陲ｫ繧�
                return Status.SameName;
            }
            else
            {   // 騾壻ｿ｡螟ｱ謨�
                return Status.False;
            }
        }
    }
}
