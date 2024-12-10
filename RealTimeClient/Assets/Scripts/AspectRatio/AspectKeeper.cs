//---------------------------------------------------------------
//
// アスペクト比保持 [ AspectKeeper.cs ]
// Author:Kenta Nakamoto
// Data:2024/07/17
// Update:2024/07/17
//
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways] // 再生時以外でも動作する
public class AspectKeeper : MonoBehaviour
{
    //-------------------------------------------
    // フィールド

    /// <summary>
    /// 対象とするカメラ
    /// </summary>
    [SerializeField]
    private Camera targetCamera;

    /// <summary>
    /// 目的解像度
    /// </summary>
    [SerializeField]
    private Vector2 aspectVec;

    //--------------------------------------------
    // メソッド

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        var screenAspect = Screen.width / (float)Screen.height; // 画面のアスペクト比
        var targetAspect = aspectVec.x / aspectVec.y; // 目的のアスペクト比

        var magRate = targetAspect / screenAspect; // 目的アスペクト比にするための倍率

        var viewportRect = new Rect(0, 0, 1, 1); // Viewport初期値でRectを作成

        if (magRate < 1)
        {
            viewportRect.width = magRate; // 使用する横幅を変更
            viewportRect.x = 0.5f - viewportRect.width * 0.5f;// 中央寄せ
        }
        else
        {
            viewportRect.height = 1 / magRate; // 使用する縦幅を変更
            viewportRect.y = 0.5f - viewportRect.height * 0.5f;// 中央寄せ
        }

        targetCamera.rect = viewportRect; // カメラのViewportに適用
    }
}