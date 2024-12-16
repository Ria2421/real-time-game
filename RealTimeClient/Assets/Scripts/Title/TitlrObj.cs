//---------------------------------------------------------------
// タイトルオブジェ用スクリプト [ TitleObj.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/16
// Update:2024/12/16
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitlrObj : MonoBehaviour
{
    //-------------------------------------------------------
    // フィールド

    /// <summary>
    /// 爆発エフェクト
    /// </summary>
    [SerializeField] GameObject boomEffect;

    //-------------------------------------------------------
    // メソッド

    /// <summary>
    /// トリガーコライダー接触時処理
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "OtherPlayer")
        {
            // 爆発エフェクト
            Instantiate(boomEffect, collision.gameObject.transform.position,Quaternion.identity);
        }
    }
}
