using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFirstModel : MonoBehaviour
{
    const string ServerURL = "http://localhost:7000";

    async void Start()
    {
        // UniTask
        int sumResult = await Sum(100, 323);   // 非同期処理。完了次第次の行へ
        int subResult = await Sub(150, 100);
        Debug.Log("足し算" + sumResult);
        Debug.Log("引き算" + subResult);

        // 通常
        /*Sum(100, 250, result =>
        {
            Debug.Log(result);
        });*/
    }

    
    // UniTask
    public async UniTask<int> Sum(int x, int y)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // ハンドラーの設定
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // サーバーとのチャンネルを設定
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // サーバーとの接続
        var result = await client.SumAsync(x, y);   // 関数呼び出し
        return result;
    }

    public async UniTask<int> Sub(int x, int y)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // ハンドラーの設定
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // サーバーとのチャンネルを設定
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // サーバーとの接続
        var result = await client.SubAsync(x, y);   // 関数呼び出し
        return result;
    }

    // 通常
    /*public async void Sum(int x, int y, Action<int> callback)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // ハンドラーの設定
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // サーバーとのチャンネルを設定
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // サーバーとの接続
        var result = await client.SumAsync(x, y);   // 関数呼び出し
        callback?.Invoke(result);
    }*/
}
