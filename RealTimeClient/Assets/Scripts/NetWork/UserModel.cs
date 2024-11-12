//---------------------------------------------------------------
// ユーザーモデル [ UserModel.cs ]
// Author:Kenta Nakamoto
// Data:2024/11/12
// Update:2024/11/12
//---------------------------------------------------------------
using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserModel : BaseModel
{
    //-------------------------------------------------------
    // フィールド

    /// <summary>
    /// ユーザーID
    /// </summary>
    private int userId;

    //-------------------------------------------------------
    // メソッド

    /// <summary>
    /// ユーザー登録処理
    /// </summary>
    /// <param name="name">ユーザー名</param>
    /// <returns> [true]成功 , [false]失敗 </returns>
    public async UniTask<bool> RegistUserAsync(string name)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // ハンドラーの設定
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // サーバーとのチャンネルを設定
        var client = MagicOnionClient.Create<IUserService>(channel); // サーバーとの接続
        try
        {
            userId = await client.RegistUserAsync(name);   // 関数呼び出し
            return true;
        }catch(RpcException e)
        {
            Debug.Log(e);
            return false;
        }
    }
}
