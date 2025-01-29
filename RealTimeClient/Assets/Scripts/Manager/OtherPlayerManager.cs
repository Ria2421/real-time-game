//---------------------------------------------------------------
// ���v���C���[�}�l�[�W���[ [ OtherPlayerManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/09
// Update:2024/12/16
//---------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OtherPlayerManager : MonoBehaviour
{
    //=====================================
    // �t�B�[���h

    /// <summary>
    /// �@�̂Ɋ֘A����ڑ�ID
    /// </summary>
    public Guid ConnectionID { get; set; }

    /// <summary>
    /// �@�̂Ɋ֘A���郆�[�U�[��
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// �Q����
    /// </summary>
    public int JoinOrder { get; set; }

    /// <summary>
    /// �^�[�{�p�[�e�B�N��
    /// </summary>
    [SerializeField] private ParticleSystem turboParticle;

    /// <summary>
    /// �h���t�g�p�[�e�B�N��
    /// </summary>
    [SerializeField] private ParticleSystem driftParticle;

    /// <summary>
    /// �^�C�g���V�[���p�R���C�_�[
    /// </summary>
    [SerializeField] private GameObject titleCollider;

    //=====================================
    // ���\�b�h

    /// <summary>
    /// ��������
    /// </summary>
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "1_TitleScene")
        {   // �^�C�g���V�[���̂ݗL��
            titleCollider.SetActive(true);
        }
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

    /// <summary>
    /// �R���C�_�[�ڐG������
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "TitleObj")
        {
            // ���g���폜
            Destroy(this.gameObject);
        }

        if (collision.gameObject.tag == "Goal")
        {
            Destroy(this.gameObject);
        }
    }
}
