//---------------------------------------------------------------
// ���v���C���[�}�l�[�W���[ [ OtherPlayerManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/09
// Update:2024/12/09
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerManager : MonoBehaviour
{
    //=====================================
    // �t�B�[���h
    
    /// <summary>
    /// �^�[�{�p�[�e�B�N��
    /// </summary>
    [SerializeField] ParticleSystem turboParticle;

    /// <summary>
    /// �h���t�g�p�[�e�B�N��
    /// </summary>
    [SerializeField] ParticleSystem driftParticle;

    //=====================================
    // ���\�b�h

    /// <summary>
    /// ��������
    /// </summary>
    void Start()
    {
        
    }

    /// <summary>
    /// ���X�V����
    /// </summary>
    void FixedUpdate()
    {
        
    }

    /// <summary>
    /// �^�[�{�Đ�����
    /// </summary>
    /// <param name="flag"></param>
    public void playTurbo(bool flag)
    {
        if (flag)
        {   // �Đ�
            if(!turboParticle.isPlaying) turboParticle.Play();
        }
        else
        {   // ��~
            if(turboParticle.isPlaying) turboParticle.Stop();
        }
    }

    /// <summary>
    /// �h���t�g�Đ�����
    /// </summary>
    /// <param name="flag"></param>
    public void playDrift(bool flag)
    {
        if (flag)
        {
            if(!driftParticle.isPlaying) driftParticle.Play();
        }
        else
        {
            if(driftParticle.isPlaying) driftParticle.Stop();
        }
    }
}