using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFirstModel : BaseModel
{
    async void Start()
    {
        // UniTask
        int sumResult = await Sum(100, 323);   // 非同期処理。完了次第次の行へ

        int subResult = await Sub(150, 100);

        int[] numList = { 5, 10, 15, 20, 25 };
        int sumAllResult = await SumAll(numList);

        int[] calcResult = await CalcForOperation(5, 5);

        IMyFirstService.Number numArray = new IMyFirstService.Number();
        numArray.x = 5.8f;
        numArray.y = 6.2f;
        float result = await SumAllNumber(numArray);

        // ログ出力
        //Debug.Log("足し算" + sumResult);

        //Debug.Log("引き算" + subResult);

        //Debug.Log("配列足し算" + sumAllResult);

        //Debug.Log("足し算" + calcResult[0]);
        //Debug.Log("引き算" + calcResult[1]);
        //Debug.Log("かけ算" + calcResult[2]);
        //Debug.Log("わり算" + calcResult[3]);

        Debug.Log("小数足し算：" + result);

        // 通常
        /*Sum(100, 250, result =>
        {
            Debug.Log(result);
        });*/
    }


    // UniTask ----------------------------------------------------------------------------------------------------------------------------------

    // 「足し算API」二つの整数を引数で受け取り合計値を返す
    public async UniTask<int> Sum(int x, int y)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // ハンドラーの設定
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // サーバーとのチャンネルを設定
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // サーバーとの接続
        var result = await client.SumAsync(x, y);   // 関数呼び出し
        return result;
    }

    // 「引き算API」二つの整数を引数で受け取り減算値を返す
    public async UniTask<int> Sub(int x, int y)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // ハンドラーの設定
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // サーバーとのチャンネルを設定
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // サーバーとの接続
        var result = await client.SubAsync(x, y);   // 関数呼び出し
        return result;
    }

    // 「足し算API」int配列を引数で受け取り合計値を返す
    public async UniTask<int> SumAll(int[] numList)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // ハンドラーの設定
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // サーバーとのチャンネルを設定
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // サーバーとの接続
        var result = await client.SumAllAsync(numList);   // 関数呼び出し
        return result;
    }

    // [0] x+y , [1] x-y , [2] x*y , [3] x/y の配列を返す
    public async UniTask<int[]> CalcForOperation(int x, int y)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // ハンドラーの設定
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // サーバーとのチャンネルを設定
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // サーバーとの接続
        var result = await client.CalcForOperationAsync(x,y);   // 関数呼び出し
        return result;
    }

    public async UniTask<float> SumAllNumber(IMyFirstService.Number numArray)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // ハンドラーの設定
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // サーバーとのチャンネルを設定
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // サーバーとの接続
        var result = await client.SumAllNumberAsync(numArray);   // 関数呼び出し
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
