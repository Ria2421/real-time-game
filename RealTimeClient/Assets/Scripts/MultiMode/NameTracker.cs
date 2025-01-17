//---------------------------------------------------------------
// ユーザー名追従 [ NameTracker.cs ]
// Author:Kenta Nakamoto
// Data:2025/01/16
// Update:2025/01/16
// 参考URL:https://tech.pjin.jp/blog/2017/07/14/unity_ugui_sync_rendermode/
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameTracker : MonoBehaviour
{
    //=====================================
    // フィールド

    /// <summary>
    /// 追従対象
    /// </summary>
    private Transform targetTfm;

    /// <summary>
    /// 表示UIのRectTransform
    /// </summary>
    private RectTransform myRectTfm;

    /// <summary>
    /// 表示オフセット
    /// </summary>
    private Vector3 offset = new Vector3(0, 1.5f, 0);

    //=====================================
    // メソッド

    void Start()
    {
        myRectTfm = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (targetTfm == null) return;

        // 表示位置を追従対象+ オフセットの座標に移動し続ける
        myRectTfm.position
            = RectTransformUtility.WorldToScreenPoint(Camera.main, targetTfm.position + offset);
    }

    /// <summary>
    /// 追従対象設定処理
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target)
    {
        targetTfm = target;
    }
}
