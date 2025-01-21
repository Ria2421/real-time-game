//---------------------------------------------------------------
// �����L���O���f�� [ RankingModel.cs ]
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
    // ���\�b�h

    /// <summary>
    /// �w��X�e�[�W�����L���O�̎擾
    /// </summary>
    /// <param name="stageID">�X�e�[�WID</param>
    /// <returns></returns>
    public async UniTask<List<RankingData>> GetRankingAsync(int stageID)
    {
        List<RankingData> result = new List<RankingData>();

        using var handler = new YetAnotherHttpHandler() { Http2Only = true };                                   // �n���h���[�̐ݒ�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // �T�[�o�[�Ƃ̃`�����l����ݒ�
        var client = MagicOnionClient.Create<ISoloService>(channel);                                            // �T�[�o�[�Ƃ̐ڑ�

        try
        {
            result = await client.GetRankingAsync(stageID);     // �����L���O�擾
            return result;
        }
        catch (RpcException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    /// <summary>
    /// �^�C���o�^
    /// [return:����]
    /// </summary>
    /// <param name="stageID">�v���C�����X�e�[�WNo</param>
    /// <param name="userID"> ���[�U�[ID</param>
    /// <param name="time">   �o�^�^�C��</param>
    /// <returns></returns>
    public async UniTask<RegistResult> RegistClearTimeAsync(int stageID, int userID, int time, string ghostData)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };                                   // �n���h���[��ݒ�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // �T�[�o�[�Ƃ̃`�����l����ݒ�
        var client = MagicOnionClient.Create<ISoloService>(channel);

        RegistResult result = new RegistResult();

        try
        {
            result = await client.RegistClearTimeAsync(stageID,userID,time,ghostData);     // �L�^�o�^ (30�b�̋L�^��15KB��)
            Debug.Log("�N���A�f�[�^�o�^����");
            return result;
        }
        catch (RpcException e)
        {
            Debug.Log(e);
            return result;
        }
    }
}
