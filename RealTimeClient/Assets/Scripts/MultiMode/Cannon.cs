//---------------------------------------------------------------
// 螟ｧ遐ｲ繧ｹ繧ｯ繝ｪ繝励ヨ [ Cannon.cs ]
// Author:Kenta Nakamoto
// Data:2025/01/27
// Update:2025/01/28
//---------------------------------------------------------------
using DG.Tweening;
using RealTimeServer.Model.Entity;
using Shared.Interfaces.StreamingHubs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;

public class Cannon : MonoBehaviour
{
    //-------------------------------------------------------
    // 繝輔ぅ繝ｼ繝ｫ繝�

    /// <summary>
    /// 蠑ｾ縺ｮ騾溷ｺｦ
    /// </summary>
    [SerializeField] private float bulletSpeed = 4.0f;

    /// <summary>
    /// 蠑ｾ繝励Ξ繝上ヶ
    /// </summary>
    [SerializeField] private GameObject bulletprefab;

    /// <summary>
    /// 蛻ｰ逹�菴咲ｽｮ
    /// </summary>
    [SerializeField] private Transform endPosition;

    //-------------------------------------------------------
    // 繝｡繧ｽ繝��ラ

    /// <summary>
    /// 蠑ｾ逋ｺ蟆�処逅�
    /// </summary>
    public void ShotBullet()
    {
        // 蠑ｾ縺ｮ逕滓��
        var bulletObj = Instantiate(bulletprefab, this.gameObject.transform.position, Quaternion.identity);

        // SE蜀咲函
        SEManager.Instance.Play(SEPath.CANNON);

        // 蠑ｾ縺ｮ遘ｻ蜍�
        bulletObj.transform.DOMove(endPosition.position, bulletSpeed).SetEase(Ease.Linear)
            .SetUpdate(UpdateType.Fixed, true).OnComplete(() =>
            {
                // 遘ｻ蜍募ｮ御ｺ��ｾ後↓豸亥悉
                Destroy(bulletObj);
            });
    }
}
