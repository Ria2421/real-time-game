using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchingManager : MonoBehaviour
{
    //-------------------------------------------------------
    // フィールド

    /// <summary>
    /// ルームモデル格納用
    /// </summary>
    [SerializeField] private RoomModel roomModel;

    /// <summary>
    /// 入力されたユーザーID
    /// </summary>
    [SerializeField] private Text inputText;

    /// <summary>
    /// マッチングボタン
    /// </summary>
    [SerializeField] private Button matchingButton;

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
        matchingButton.interactable = false;

        // 接続
        await roomModel.ConnectAsync();
        // マッチング
        await roomModel.JoinLobbyAsync(UserModel.Instance.UserId);
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

        StartCoroutine("TransGmaeScene");
    }

    // ゲームシーン遷移
    private IEnumerator TransGmaeScene()
    {
        // 1秒待ってコルーチン中断
        yield return new WaitForSeconds(1.0f);

        // ゲームシーンに遷移
        SceneManager.LoadScene("05_OnlineScene");
    }
}
