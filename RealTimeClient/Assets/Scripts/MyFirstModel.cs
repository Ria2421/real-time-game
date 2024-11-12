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
        int sumResult = await Sum(100, 323);   // �񓯊������B�������掟�̍s��

        int subResult = await Sub(150, 100);

        int[] numList = { 5, 10, 15, 20, 25 };
        int sumAllResult = await SumAll(numList);

        int[] calcResult = await CalcForOperation(5, 5);

        IMyFirstService.Number numArray = new IMyFirstService.Number();
        numArray.x = 5.8f;
        numArray.y = 6.2f;
        float result = await SumAllNumber(numArray);

        // ���O�o��
        //Debug.Log("�����Z" + sumResult);

        //Debug.Log("�����Z" + subResult);

        //Debug.Log("�z�񑫂��Z" + sumAllResult);

        //Debug.Log("�����Z" + calcResult[0]);
        //Debug.Log("�����Z" + calcResult[1]);
        //Debug.Log("�����Z" + calcResult[2]);
        //Debug.Log("���Z" + calcResult[3]);

        Debug.Log("���������Z�F" + result);

        // �ʏ�
        /*Sum(100, 250, result =>
        {
            Debug.Log(result);
        });*/
    }


    // UniTask ----------------------------------------------------------------------------------------------------------------------------------

    // �u�����ZAPI�v��̐����������Ŏ󂯎�荇�v�l��Ԃ�
    public async UniTask<int> Sum(int x, int y)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // �n���h���[�̐ݒ�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // �T�[�o�[�Ƃ̃`�����l����ݒ�
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // �T�[�o�[�Ƃ̐ڑ�
        var result = await client.SumAsync(x, y);   // �֐��Ăяo��
        return result;
    }

    // �u�����ZAPI�v��̐����������Ŏ󂯎�茸�Z�l��Ԃ�
    public async UniTask<int> Sub(int x, int y)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // �n���h���[�̐ݒ�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // �T�[�o�[�Ƃ̃`�����l����ݒ�
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // �T�[�o�[�Ƃ̐ڑ�
        var result = await client.SubAsync(x, y);   // �֐��Ăяo��
        return result;
    }

    // �u�����ZAPI�vint�z��������Ŏ󂯎�荇�v�l��Ԃ�
    public async UniTask<int> SumAll(int[] numList)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // �n���h���[�̐ݒ�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // �T�[�o�[�Ƃ̃`�����l����ݒ�
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // �T�[�o�[�Ƃ̐ڑ�
        var result = await client.SumAllAsync(numList);   // �֐��Ăяo��
        return result;
    }

    // [0] x+y , [1] x-y , [2] x*y , [3] x/y �̔z���Ԃ�
    public async UniTask<int[]> CalcForOperation(int x, int y)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // �n���h���[�̐ݒ�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // �T�[�o�[�Ƃ̃`�����l����ݒ�
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // �T�[�o�[�Ƃ̐ڑ�
        var result = await client.CalcForOperationAsync(x,y);   // �֐��Ăяo��
        return result;
    }

    public async UniTask<float> SumAllNumber(IMyFirstService.Number numArray)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // �n���h���[�̐ݒ�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // �T�[�o�[�Ƃ̃`�����l����ݒ�
        var client = MagicOnionClient.Create<IMyFirstService>(channel); // �T�[�o�[�Ƃ̐ڑ�
        var result = await client.SumAllNumberAsync(numArray);   // �֐��Ăяo��
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
