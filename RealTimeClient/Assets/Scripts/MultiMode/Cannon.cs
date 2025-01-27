//---------------------------------------------------------------
// 大砲スクリプト [ Cannon.cs ]
// Author:Kenta Nakamoto
// Data:2025/01/27
// Update:2025/01/27
//---------------------------------------------------------------
using DG.Tweening;
using RealTimeServer.Model.Entity;
using Shared.Interfaces.StreamingHubs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    //-------------------------------------------------------
    // フィールド

    /// <summary>
    /// 弾の速度
    /// </summary>
    [SerializeField] private float bulletSpeed = 4.0f;

    /// <summary>
    /// 弾プレハブ
    /// </summary>
    [SerializeField] private GameObject bulletprefab;

    /// <summary>
    ///  発射位置
    /// </summary>
    [SerializeField] private Transform startPosition;

    /// <summary>
    /// 到着位置
    /// </summary>
    [SerializeField] private Transform endPosition;

    //-------------------------------------------------------
    // メソッド

    /// <summary>
    /// 弾発射処理
    /// </summary>
    public void ShotBullet()
    {
        // 弾の生成
        var bulletObj = Instantiate(bulletprefab, startPosition.position, Quaternion.identity);

        // 弾の移動
        bulletObj.transform.DOMove(endPosition.position, bulletSpeed).SetEase(Ease.Linear)
            .SetUpdate(UpdateType.Fixed, true).OnComplete(() =>
            {
                // 移動完了後に消去
                Destroy(bulletObj);
            });
    }
}
