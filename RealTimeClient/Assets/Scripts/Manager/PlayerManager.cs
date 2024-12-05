//---------------------------------------------------------------
// プレイヤーマネージャー [ PlayerManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/05
// Update:2024/12/05
//---------------------------------------------------------------
using Shared.Interfaces.StreamingHubs;
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

    // トリガーコライダー接触時処理
    private void OnTriggerEnter(Collider collision)
    {
        if(endFlag) return;

        if (collision.gameObject.tag == "Goal")
        {   // "Goal"タグのオブジェクトに接触した時
            endFlag = true;

            // 全ユーザーにゲーム終了通知
            SendEndGame();
        }
    }
}