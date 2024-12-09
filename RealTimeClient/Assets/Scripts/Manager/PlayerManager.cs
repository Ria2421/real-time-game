//---------------------------------------------------------------
// プレイヤーマネージャー [ PlayerManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/05
// Update:2024/12/05
//---------------------------------------------------------------
using DavidJalbert;
using Shared.Interfaces.StreamingHubs;
using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //=====================================
    // フィールド

    /// <summary>
    /// ルームモデル格納用
    /// </summary>
    private RoomModel roomModel;

    /// <summary>
    /// ゲーム終了フラグ
    /// </summary>
    private bool endFlag = false;

    //=====================================
    // メソッド

    // 初期処理
    void Start()
    {
        // ルームモデルの取得
        roomModel = GameObject.Find("RoomModel").GetComponent<RoomModel>();
    }

    // ゲーム終了通知送信処理
    private async void SendEndGame()
    {
        await roomModel.GameEndAsync();
    }

    // 撃破通知処理
    private async void CrushingPlayerAsync(string attackName, string cruchName, Guid crushID)
    {
        await roomModel.CrushingPlayerAsync(attackName, cruchName, crushID);
    }

    // トリガーコライダー接触時処理
    private void OnTriggerEnter(Collider collision)
    {
        if(endFlag) return;

        // Tag毎の接触時処理

        if (collision.gameObject.tag == "Goal")
        {
            endFlag = true;

            // 全ユーザーにゲーム終了通知
            SendEndGame();
        }

        if(collision.gameObject.tag == "Trap")
        {
            // 爆発
            transform.parent.gameObject.GetComponent<TinyCarExplosiveBody>().explode();
        }

        if(collision.gameObject.tag == "OtherPlayer")
        {
            // 撃破通知 (倒したPL名,倒されたPL名,倒された人の接続ID)
            CrushingPlayerAsync(roomModel.UserName,collision.GetComponent<OtherPlayerManager>().UserName,collision.GetComponent<OtherPlayerManager>().ConnectionID);
        }
    }
}