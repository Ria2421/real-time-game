//---------------------------------------------------------------
// ユーザーモデル [ UserModel.cs ]
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
    // フィールド

    public enum Status
    {
        True = 0,   // 成功
        False,      // 失敗
        SameName,   // 名前被り
    }

    /// <summary>
    /// ユーザーID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// トークンID
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// ゴーストデータ
    /// </summary>
    public string GhostData { get; set; } = "";

    /// <summary>
    /// getプロパティを呼び出した初回時にインスタンス生成してstaticで保持
    /// </summary>
    private static UserModel instance;

    /// <summary>
    /// NetworkManagerプロパティ
    /// </summary>
    public static UserModel Instance
    {
        get
        {
            if (instance == null)
            {
                // GameObjectを生成し、UserModelを追加
                GameObject gameObject = new GameObject("UserModel");
                instance = gameObject.AddComponent<UserModel>();

                // シーン遷移で破棄されないように設定
                DontDestroyOnLoad(gameObject);
            }

            return instance;
        }
    }

    //-------------------------------------------------------
    // メソッド

    /// <summary>
    /// ユーザーデータ保存処理
    /// </summary>
    private void SaveUserData()
    {
        // セーブデータクラスの生成
        SaveData saveData = new SaveData();
        saveData.UserID = this.UserId;
        saveData.Token = this.Token;

        // データをJSONシリアライズ
        string json = JsonConvert.SerializeObject(saveData);

        // 指定した絶対パスに"saveData.json"を保存
        var writer = new StreamWriter(Application.persistentDataPath + "/saveData.json");
        writer.Write(json); // 書き出し
        writer.Flush();     // バッファに残っている値を全て書き出し
        writer.Close();     // ファイル閉
    }

    /// <summary>
    /// ユーザーデータ読み込み処理
    /// </summary>
    /// <returns></returns>
    public bool LoadUserData()
    {
        if (!File.Exists(Application.persistentDataPath + "/saveData.json"))
        {   // 指定のパスのファイルが存在しなかった時、早期リターン
            return false;
        }

        //  ローカルファイルからユーザーデータの読込処理
        var reader = new StreamReader(Application.persistentDataPath + "/saveData.json");
        string json = reader.ReadToEnd();
        reader.Close();

        // セーブデータJSONをデシリアライズして取得
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);
        this.UserId = saveData.UserID;
        this.Token = saveData.Token;

        // 読み込み結果をリターン
        return true;
    }

    /// <summary>
    /// ユーザーデータ登録処理
    /// </summary>
    /// <param name="name">ユーザー名</param>
    /// <returns> [true]成功 , [false]失敗 </returns>
    public async UniTask<Status> RegistUserAsync(string name)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // ハンドラーの設定
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // サーバーとのチャンネルを設定
        var client = MagicOnionClient.Create<IUserService>(channel);    // サーバーとの接続

        try
        {
            this.Token = Guid.NewGuid().ToString();                     // トークン生成
            this.UserId = await client.RegistUserAsync(name, Token);    // 関数呼び出し
            SaveUserData();                                             // ローカルに保存
            return Status.True;
        }catch(RpcException e)
        {
            Debug.Log(e);
            if (e.Status.Detail == "SameName")
            {   // 名前被り
                return Status.SameName;
            }
            else
            {   // 通信失敗
                return Status.False;
            }
        }
    }

    /// <summary>
    /// ユーザーをID指定で検索
    /// [return : ユーザー情報]
    /// </summary>
    /// <param name="id">ユーザーID</param>
    /// <returns></returns>
    public async UniTask<User> SearchUserID(int id)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // ハンドラーの設定
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // サーバーとのチャンネルを設定
        var client = MagicOnionClient.Create<IUserService>(channel);    // サーバーとの接続

        try
        {
            var userData = await client.SearchUserID(id);    // 関数呼び出し
            return userData;
        }
        catch(RpcException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    /// <summary>
    /// 指定IDのユーザー名更新
    /// [return : 真偽]
    /// </summary>
    /// <param name="id">  ユーザーID</param>
    /// <param name="name">ユーザー名</param>
    /// <returns></returns>
    public async UniTask<Status> UpdateUserName(int id, string name)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // ハンドラーの設定
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // サーバーとのチャンネルを設定
        var client = MagicOnionClient.Create<IUserService>(channel);    // サーバーとの接続

        try
        {
            var userData = await client.UpdateUserName(id, name);
            return Status.True;
        }
        catch (RpcException e)
        {
            if (e.Status.Detail == "SameName")
            {   // 名前被り
                return Status.SameName;
            }
            else
            {   // 通信失敗
                return Status.False;
            }
        }
    }
}
