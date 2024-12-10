using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchingManager : MonoBehaviour
{
    //-------------------------------------------------------
    // フィールド

    [SerializeField] private RoomModel roomModel;

    [SerializeField] private Text inputText;

    //-------------------------------------------------------
    // メソッド

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        roomModel.OnMatchingUser += OnMatchingUser;     // マッチング完了通知
    }

    /// <summary>
    /// マッチングボタン
    /// </summary>
    public async void OnMatchingButton()
    {
        // 接続
        await roomModel.ConnectAsync();
        // マッチング
        await roomModel.JoinLobbyAsync(int.Parse(inputText.text));
    }

    /// <summary>
    /// マッチング完了通知受信時の処理
    /// </summary>
    /// <param name="roomName"></param>
    private async void OnMatchingUser(string roomName)
    {
        roomModel.RoomName = roomName;  // 発行されたルーム名を保存

        // 退出
        await roomModel.ExitAsync();

        // ゲームシーンに移動
        SceneManager.LoadScene("05_OnlineScene");
    }
}
