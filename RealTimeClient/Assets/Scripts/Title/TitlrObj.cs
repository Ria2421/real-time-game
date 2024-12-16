//---------------------------------------------------------------
// �^�C�g���I�u�W�F�p�X�N���v�g [ TitleObj.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/16
// Update:2024/12/16
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitlrObj : MonoBehaviour
{
    //-------------------------------------------------------
    // �t�B�[���h

    /// <summary>
    /// �����G�t�F�N�g
    /// </summary>
    [SerializeField] GameObject boomEffect;

    //-------------------------------------------------------
    // ���\�b�h

    /// <summary>
    /// �g���K�[�R���C�_�[�ڐG������
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "OtherPlayer")
        {
            // �����G�t�F�N�g
            Instantiate(boomEffect, collision.gameObject.transform.position,Quaternion.identity);
        }
    }
}
