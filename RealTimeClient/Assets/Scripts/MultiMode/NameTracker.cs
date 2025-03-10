//---------------------------------------------------------------
// ユーザー名追従 [ NameTracker.cs ]
// Author:Kenta Nakamoto
// 参考URL:https://tech.pjin.jp/blog/2017/07/14/unity_ugui_sync_rendermode/
//---------------------------------------------------------------
using DavidJalbert;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private Vector3 offset;

    /// <summary>
    /// 名前フォントサイズ
    /// </summary>
    private int fontSize;

    /// <summary>
    /// 俯瞰視点時のフォントサイズ
    /// </summary>
    private const int topFontSize = 120;

    /// <summary>
    /// 第三者視点時のフォントサイズ
    /// </summary>
    private const int thirdFontSize = 85;

    /// <summary>
    /// メインカメラ
    /// </summary>
    [SerializeField] private Transform cameraTrs;

    /// <summary>
    /// カメラスクリプト
    /// </summary>
    [SerializeField] private TinyCarCamera tinyCarCamera;

    /// <summary>
    /// nameテキスト
    /// </summary>
    [SerializeField] private Text nameText;

    //=====================================
    // メソッド

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        myRectTfm = GetComponent<RectTransform>();
    }

    /// <summary>
    /// 定期更新処理
    /// </summary>
    void FixedUpdate()
    {
        if (targetTfm == null) return;

        //++ 表示位置を追従対象+ オフセットの座標に移動し続ける
        myRectTfm.position = targetTfm.position + offset;

        myRectTfm.rotation = cameraTrs.rotation; 

        // カメラモードによってフォントサイズを変更する
        if(tinyCarCamera.viewMode == TinyCarCamera.CAMERA_MODE.ThirdPerson)
        {
            fontSize = thirdFontSize;
        }
        else if (tinyCarCamera.viewMode == TinyCarCamera.CAMERA_MODE.TopDown)
        {
            fontSize = topFontSize;
        }

        nameText.fontSize = fontSize;
    }

    /// <summary>
    /// 追従対象設定処理
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target,int no)
    {
        targetTfm = target;

        // オフセット距離の変更
        if(no == 1)
        {
            offset = new Vector3(0, 2.3f, 0);
        }else if(no == 2)
        {
            offset = new Vector3(0, 3.5f, 0);
        }
    }
}
