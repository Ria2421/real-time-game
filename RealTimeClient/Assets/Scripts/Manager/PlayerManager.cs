//---------------------------------------------------------------
// �v���C���[�}�l�[�W���[ [ PlayerManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/05
// Update:2024/12/05
//---------------------------------------------------------------
using DavidJalbert;
using Shared.Interfaces.StreamingHubs;
using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //=====================================
    // �t�B�[���h

    /// <summary>
    /// ���[�����f���i�[�p
    /// </summary>
    private RoomModel roomModel;

    /// <summary>
    /// �Q�[���I���t���O
    /// </summary>
    private bool endFlag = false;

    //=====================================
    // ���\�b�h

    // ��������
    void Start()
    {
        // ���[�����f���̎擾
        roomModel = GameObject.Find("RoomModel").GetComponent<RoomModel>();
    }

    // �Q�[���I���ʒm���M����
    private async void SendEndGame()
    {
        await roomModel.GameEndAsync();
    }

    // ���j�ʒm����
    private async void CrushingPlayerAsync(string attackName, string cruchName, Guid crushID)
    {
        await roomModel.CrushingPlayerAsync(attackName, cruchName, crushID);
    }

    // �g���K�[�R���C�_�[�ڐG������
    private void OnTriggerEnter(Collider collision)
    {
        if(endFlag) return;

        // Tag���̐ڐG������

        if (collision.gameObject.tag == "Goal")
        {
            endFlag = true;

            // �S���[�U�[�ɃQ�[���I���ʒm
            SendEndGame();
        }

        if(collision.gameObject.tag == "Trap")
        {
            // ����
            transform.parent.gameObject.GetComponent<TinyCarExplosiveBody>().explode();
        }

        if(collision.gameObject.tag == "OtherPlayer")
        {
            // ���j�ʒm (�|����PL��,�|���ꂽPL��,�|���ꂽ�l�̐ڑ�ID)
            CrushingPlayerAsync(roomModel.UserName,collision.GetComponent<OtherPlayerManager>().UserName,collision.GetComponent<OtherPlayerManager>().ConnectionID);
        }
    }
}