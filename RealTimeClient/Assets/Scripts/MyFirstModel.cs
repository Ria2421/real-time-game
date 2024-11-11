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
        int sumResult = await Sum(100, 323);   // �񓯊������B�������掟�̍s��
        int subResult = await Sub(150, 100);
        Debug.Log("�����Z" + sumResult);
        Debug.Log("�����Z" + subResult);

        // �ʏ�
        /*Sum(100, 250, result =>
        {
            Debug.Log(result);
        });*/
    }

    
    // UniTask
    public async UniTask<int> Sum(int x, int y)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // �n���h���[�̐ݒ�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // �T�[�o�[�Ƃ̃`�����l����ݒ�
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // �T�[�o�[�Ƃ̐ڑ�
        var result = await client.SumAsync(x, y);   // �֐��Ăяo��
        return result;
    }

    public async UniTask<int> Sub(int x, int y)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // �n���h���[�̐ݒ�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // �T�[�o�[�Ƃ̃`�����l����ݒ�
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // �T�[�o�[�Ƃ̐ڑ�
        var result = await client.SubAsync(x, y);   // �֐��Ăяo��
        return result;
    }

    // �ʏ�
    /*public async void Sum(int x, int y, Action<int> callback)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // �n���h���[�̐ݒ�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // �T�[�o�[�Ƃ̃`�����l����ݒ�
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // �T�[�o�[�Ƃ̐ڑ�
        var result = await client.SumAsync(x, y);   // �֐��Ăяo��
        callback?.Invoke(result);
    }*/
}
