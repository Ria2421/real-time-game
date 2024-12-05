//---------------------------------------------------------------
// �v���C���[�}�l�[�W���[ [ PlayerManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/05
// Update:2024/12/05
//---------------------------------------------------------------
using Shared.Interfaces.StreamingHubs;
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

    // �g���K�[�R���C�_�[�ڐG������
    private void OnTriggerEnter(Collider collision)
    {
        if(endFlag) return;

        if (collision.gameObject.tag == "Goal")
        {   // "Goal"�^�O�̃I�u�W�F�N�g�ɐڐG������
            endFlag = true;

            // �S���[�U�[�ɃQ�[���I���ʒm
            SendEndGame();
        }
    }
}