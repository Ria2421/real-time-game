//---------------------------------------------------------------
// �\���v���C���[�}�l�[�W���[ [ SoloPlayerManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/18
// Update:2024/12/18
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloPlayerManager : MonoBehaviour
{
    //=====================================
    // �t�B�[���h

    /// <summary>
    /// ���`�F�b�N�|�C���g��
    /// </summary>
    private int currentCheckPointCnt = 1;

    /// <summary>
    /// �`�F�b�N�|�C���g�I�u�W�F�i�[�p
    /// </summary>
    [SerializeField] private GameObject[] checkPoints;

    /// <summary>
    /// �Q�[���}�l�[�W���[
    /// </summary>
    [SerializeField] private SoloManager soloManager;

    //=====================================
    // ���\�b�h

    /// <summary>
    /// ��������
    /// </summary>
    void Start()
    {
        
    }

    /// <summary>
    /// �X�V����
    /// </summary>
    void Update()
    {
        
    }

    /// <summary>
    /// �R���C�_�[�ڐG������
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "CheckPoint" + currentCheckPointCnt.ToString())
        {
            Debug.Log(currentCheckPointCnt.ToString() + "�Ԃ�ʉ�");

            if (currentCheckPointCnt == checkPoints.Length)
            {   // �Ō�̃`�F�b�N�|�C���g��ʉ߂�����
                currentCheckPointCnt = 1;   // �`�F�b�N�|�C���g�J�E���g�̃��Z�b�g

                // ���b�v���X�V�����̌Ăяo��
                soloManager.AddRapCnt();
            }
            else
            {
                currentCheckPointCnt++;
            }
        }
    }
}
