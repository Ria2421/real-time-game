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
        int sumResult = await Sum(100, 323);   // 髱槫酔譛溷��逅��ょｮ御ｺ��ｬ｡隨ｬ谺｡縺ｮ陦後∈

        int subResult = await Sub(150, 100);

        int[] numList = { 5, 10, 15, 20, 25 };
        int sumAllResult = await SumAll(numList);

        int[] calcResult = await CalcForOperation(5, 5);

        IMyFirstService.Number numArray = new IMyFirstService.Number();
        numArray.x = 5.8f;
        numArray.y = 6.2f;
        float result = await SumAllNumber(numArray);

        // 繝ｭ繧ｰ蜃ｺ蜉�
        //Debug.Log("雜ｳ縺礼ｮ�" + sumResult);

        //Debug.Log("蠑輔″邂�" + subResult);

        //Debug.Log("驟榊��雜ｳ縺礼ｮ�" + sumAllResult);

        //Debug.Log("雜ｳ縺礼ｮ�" + calcResult[0]);
        //Debug.Log("蠑輔″邂�" + calcResult[1]);
        //Debug.Log("縺九￠邂�" + calcResult[2]);
        //Debug.Log("繧上ｊ邂�" + calcResult[3]);

        Debug.Log("蟆乗焚雜ｳ縺礼ｮ暦ｼ�" + result);

        // 騾壼ｸｸ
        /*Sum(100, 250, result =>
        {
            Debug.Log(result);
        });*/
    }


    // UniTask ----------------------------------------------------------------------------------------------------------------------------------

    // 縲瑚ｶｳ縺礼ｮ輸PI縲堺ｺ後▽縺ｮ謨ｴ謨ｰ繧貞ｼ墓焚縺ｧ蜿励￠蜿悶ｊ蜷郁ｨ亥�､繧定ｿ斐☆
    public async UniTask<int> Sum(int x, int y)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // 繝上Φ繝峨Λ繝ｼ縺ｮ險ｭ螳�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ繝√Ε繝ｳ繝阪Ν繧定ｨｭ螳�
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ謗･邯�
        var result = await client.SumAsync(x, y);   // 髢｢謨ｰ蜻ｼ縺ｳ蜃ｺ縺�
        return result;
    }

    // 縲悟ｼ輔″邂輸PI縲堺ｺ後▽縺ｮ謨ｴ謨ｰ繧貞ｼ墓焚縺ｧ蜿励￠蜿悶ｊ貂帷ｮ怜�､繧定ｿ斐☆
    public async UniTask<int> Sub(int x, int y)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // 繝上Φ繝峨Λ繝ｼ縺ｮ險ｭ螳�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ繝√Ε繝ｳ繝阪Ν繧定ｨｭ螳�
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ謗･邯�
        var result = await client.SubAsync(x, y);   // 髢｢謨ｰ蜻ｼ縺ｳ蜃ｺ縺�
        return result;
    }

    // 縲瑚ｶｳ縺礼ｮ輸PI縲絞nt驟榊��繧貞ｼ墓焚縺ｧ蜿励￠蜿悶ｊ蜷郁ｨ亥�､繧定ｿ斐☆
    public async UniTask<int> SumAll(int[] numList)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // 繝上Φ繝峨Λ繝ｼ縺ｮ險ｭ螳�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ繝√Ε繝ｳ繝阪Ν繧定ｨｭ螳�
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ謗･邯�
        var result = await client.SumAllAsync(numList);   // 髢｢謨ｰ蜻ｼ縺ｳ蜃ｺ縺�
        return result;
    }

    // [0] x+y , [1] x-y , [2] x*y , [3] x/y 縺ｮ驟榊��繧定ｿ斐☆
    public async UniTask<int[]> CalcForOperation(int x, int y)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // 繝上Φ繝峨Λ繝ｼ縺ｮ險ｭ螳�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ繝√Ε繝ｳ繝阪Ν繧定ｨｭ螳�
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ謗･邯�
        var result = await client.CalcForOperationAsync(x,y);   // 髢｢謨ｰ蜻ｼ縺ｳ蜃ｺ縺�
        return result;
    }

    public async UniTask<float> SumAllNumber(IMyFirstService.Number numArray)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // 繝上Φ繝峨Λ繝ｼ縺ｮ險ｭ螳�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ繝√Ε繝ｳ繝阪Ν繧定ｨｭ螳�
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ謗･邯�
        var result = await client.SumAllNumberAsync(numArray);   // 髢｢謨ｰ蜻ｼ縺ｳ蜃ｺ縺�
        return result;
    }

    // 騾壼ｸｸ
    /*public async void Sum(int x, int y, Action<int> callback)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // 繝上Φ繝峨Λ繝ｼ縺ｮ險ｭ螳�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ繝√Ε繝ｳ繝阪Ν繧定ｨｭ螳�
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // 繧ｵ繝ｼ繝舌��縺ｨ縺ｮ謗･邯�
        var result = await client.SumAsync(x, y);   // 髢｢謨ｰ蜻ｼ縺ｳ蜃ｺ縺�
        callback?.Invoke(result);
    }*/
}
