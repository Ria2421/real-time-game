//---------------------------------------------------------------
// 他プレイヤーマネージャー [ OtherPlayerManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/09
// Update:2024/12/09
//---------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerManager : MonoBehaviour
{
    //=====================================
    // フィールド

    /// <summary>
    /// 機体に関連する接続ID
    /// </summary>
    public Guid ConnectionID { get; set; }

    /// <summary>
    /// 機体に関連するユーザー名
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// ターボパーティクル
    /// </summary>
    [SerializeField] private ParticleSystem turboParticle;

    /// <summary>
    /// ドリフトパーティクル
    /// </summary>
    [SerializeField] private ParticleSystem driftParticle;

    //=====================================
    // メソッド

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        
    }

    /// <summary>
    /// 一定更新処理
    /// </summary>
    void FixedUpdate()
    {
        
    }

    /// <summary>
    /// ターボ再生処理
    /// </summary>
    /// <param name="flag"></param>
    public void playTurbo(bool flag)
    {
        if (flag)
        {   // 再生
            if(!turboParticle.isPlaying) turboParticle.Play();
        }
        else
        {   // 停止
            if(turboParticle.isPlaying) turboParticle.Stop();
        }
    }

    /// <summary>
    /// ドリフト再生処理
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
