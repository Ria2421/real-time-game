//---------------------------------------------------------------
// タップエフェクトマネージャー [ TapEffectManager.cs ]
// Author:Kenta Nakamoto
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapEffectManager : MonoBehaviour
{
    //=====================================
    // フィールド

    /// <summary>
    /// メインカメラ
    /// </summary>
    [SerializeField] private Camera mainCamera;

    /// <summary>
    /// 生成エフェクト
    /// </summary>
    [SerializeField] private GameObject tapEffect;

    //=====================================
    // メソッド

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 画面をタップしたときの処理
            Vector2 tapPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition + mainCamera.transform.forward * 10);

            // エフェクトを生成
            GameObject effect = Instantiate(tapEffect, tapPosition, Quaternion.identity, this.transform);
        }
    }
}
