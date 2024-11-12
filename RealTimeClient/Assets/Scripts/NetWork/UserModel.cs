//---------------------------------------------------------------
// ���[�U�[���f�� [ UserModel.cs ]
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
    // �t�B�[���h

    /// <summary>
    /// ���[�U�[ID
    /// </summary>
    private int userId;

    //-------------------------------------------------------
    // ���\�b�h

    /// <summary>
    /// ���[�U�[�o�^����
    /// </summary>
    /// <param name="name">���[�U�[��</param>
    /// <returns> [true]���� , [false]���s </returns>
    public async UniTask<bool> RegistUserAsync(string name)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };   // �n���h���[�̐ݒ�
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });    // �T�[�o�[�Ƃ̃`�����l����ݒ�
        var client = MagicOnionClient.Create<IUserService>(channel); // �T�[�o�[�Ƃ̐ڑ�
        try
        {
            userId = await client.RegistUserAsync(name);   // �֐��Ăяo��
            return true;
        }catch(RpcException e)
        {
            Debug.Log(e);
            return false;
        }
    }
}
