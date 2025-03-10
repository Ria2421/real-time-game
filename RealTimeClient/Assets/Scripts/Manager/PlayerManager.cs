//---------------------------------------------------------------
// プレイヤーマネージャー [ PlayerManager.cs ]
// Author:Kenta Nakamoto
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
    private bool isDead = false;

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
    private async void CrushingPlayerAsync(string attackName, string cruchName, Guid crushID, int deadNo)
    {
        await roomModel.CrushingPlayerAsync(attackName, cruchName, crushID, deadNo);
    }

    // トリガーコライダー接触時処理
    private void OnTriggerEnter(Collider collision)
    {
        if(isDead) return;

        // Tag毎の接触時処理

        if(collision.gameObject.tag == "Trap")
        {   // 落下死
            // 撃破通知 (倒したPL名,倒されたPL名,倒された人の接続ID,死亡要因ID)
            isDead = true;
            CrushingPlayerAsync("", roomModel.UserName, roomModel.ConnectionId, 2);
        }

        if(collision.gameObject.tag == "OtherPlayer")
        {   // 他プレイヤーを撃破
            CrushingPlayerAsync(roomModel.UserName,collision.GetComponent<OtherPlayerManager>().UserName,collision.GetComponent<OtherPlayerManager>().ConnectionID, 1);
        }
    }
}