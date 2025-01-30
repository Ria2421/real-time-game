//---------------------------------------------------------------
// 莉悶��繝ｬ繧､繝､繝ｼ繝槭ロ繝ｼ繧ｸ繝｣繝ｼ [ OtherPlayerManager.cs ]
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
    // 繝輔ぅ繝ｼ繝ｫ繝�

    /// <summary>
    /// 讖滉ｽ薙↓髢｢騾｣縺吶ｋ謗･邯唔D
    /// </summary>
    public Guid ConnectionID { get; set; }

    /// <summary>
    /// 讖滉ｽ薙↓髢｢騾｣縺吶ｋ繝ｦ繝ｼ繧ｶ繝ｼ蜷�
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 蜿ょ刈鬆�
    /// </summary>
    public int JoinOrder { get; set; }

    /// <summary>
    /// 繧ｿ繝ｼ繝懊ヱ繝ｼ繝��ぅ繧ｯ繝ｫ
    /// </summary>
    [SerializeField] private ParticleSystem turboParticle;

    /// <summary>
    /// 繝峨Μ繝輔ヨ繝代��繝��ぅ繧ｯ繝ｫ
    /// </summary>
    [SerializeField] private ParticleSystem driftParticle;

    /// <summary>
    /// 繧ｿ繧､繝医Ν繧ｷ繝ｼ繝ｳ逕ｨ繧ｳ繝ｩ繧､繝�繝ｼ
    /// </summary>
    [SerializeField] private GameObject titleCollider;

    //=====================================
    // 繝｡繧ｽ繝��ラ

    /// <summary>
    /// 蛻晄悄蜃ｦ逅�
    /// </summary>
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "1_TitleScene")
        {   // 繧ｿ繧､繝医Ν繧ｷ繝ｼ繝ｳ縺ｮ縺ｿ譛牙柑
            titleCollider.SetActive(true);
        }
    }

    /// <summary>
    /// 荳�螳壽峩譁ｰ蜃ｦ逅�
    /// </summary>
    void FixedUpdate()
    {
        
    }

    /// <summary>
    /// 繧ｿ繝ｼ繝懷��逕溷��逅�
    /// </summary>
    /// <param name="flag"></param>
    public void playTurbo(bool flag)
    {
        if (flag)
        {   // 蜀咲函
            if(!turboParticle.isPlaying) turboParticle.Play();
        }
        else
        {   // 蛛懈ｭ｢
            if(turboParticle.isPlaying) turboParticle.Stop();
        }
    }

    /// <summary>
    /// 繝峨Μ繝輔ヨ蜀咲函蜃ｦ逅�
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
    /// 繧ｳ繝ｩ繧､繝�繝ｼ謗･隗ｦ譎ょ��逅�
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "TitleObj")
        {
            // 閾ｪ霄ｫ繧貞炎髯､
            Destroy(this.gameObject);
        }

        if (collision.gameObject.tag == "Goal")
        {
            Destroy(this.gameObject);
        }
    }
}
