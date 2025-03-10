//---------------------------------------------------------------
// ソロプレイヤーマネージャー [ SoloPlayerManager.cs ]
// Author:Kenta Nakamoto
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloPlayerManager : MonoBehaviour
{
    //=====================================
    // フィールド

    /// <summary>
    /// 現チェックポイント数
    /// </summary>
    private int currentCheckPointCnt = 1;

    /// <summary>
    /// チェックポイントオブジェ格納用
    /// </summary>
    [SerializeField] private GameObject[] checkPoints;

    /// <summary>
    /// ゲームマネージャー
    /// </summary>
    [SerializeField] private SoloManager soloManager;

    //=====================================
    // メソッド

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        
    }

    /// <summary>
    /// コライダー接触時処理
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "CheckPoint" + currentCheckPointCnt.ToString())
        {
            Debug.Log(currentCheckPointCnt.ToString() + "番を通過");

            if (currentCheckPointCnt == checkPoints.Length)
            {   // 最後のチェックポイントを通過したら
                currentCheckPointCnt = 1;   // チェックポイントカウントのリセット

                // ラップ数更新処理の呼び出し
                soloManager.AddRapCnt();
            }
            else
            {
                currentCheckPointCnt++;
            }
        }
    }
}
