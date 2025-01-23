using KanKikuchi.AudioManager;
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
    /// マッチングテキストオブジェ
    /// </summary>
    [SerializeField] private GameObject matchingTextObj;

    /// <summary>
    /// マッチング完了テキストオブジェ
    /// </summary>
    [SerializeField] private GameObject completeTextObj;

    /// <summary>
    /// ルームモデル格納用
    /// </summary>
    [SerializeField] private RoomModel roomModel;

    /// <summary>
    /// キャンセルボタン
    /// </summary>
    [SerializeField] private Button cancelButton;

    /// <summary>
    /// 車背景
    /// </summary>
    [SerializeField] private Transform carBG;

    //-------------------------------------------------------
    // メソッド

    /// <summary>
    /// 初期処理
    /// </summary>
    async void Start()
    {
        roomModel.OnMatchingUser += OnMatchingUser;     // マッチング完了通知

        // 接続
        await roomModel.ConnectAsync();
        // マッチング
        await roomModel.JoinLobbyAsync(UserModel.Instance.UserId);

        Debug.Log("マッチング開始");
    }

    /// <summary>
    /// 定期更新処理
    /// </summary>
    private void FixedUpdate()
    {
        carBG.localEulerAngles += new Vector3(0,0,1.0f);
    }

    /// <summary>
    /// キャンセルボタン
    /// </summary>
    public async void OnCancelButton()
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        cancelButton.interactable = false;

        // 切断
        await roomModel.DisconnectionAsync();

        // メニューシーンに遷移
        SceneManager.LoadScene("02_MenuScene");

        Debug.Log("マッチングキャンセル");
    }

    /// <summary>
    /// マッチング完了通知受信時の処理
    /// </summary>
    /// <param name="roomName"></param>
    private async void OnMatchingUser(string roomName)
    {
        cancelButton.interactable = false;

        roomModel.RoomName = roomName;  // 発行されたルーム名を保存

        // SE再生
        SEManager.Instance.Play(SEPath.MATCHING_COMPLETE);
        // 表示切替
        matchingTextObj.SetActive(false);
        completeTextObj.SetActive(true);

        // 退出
        await roomModel.ExitAsync();

        StartCoroutine("TransGmaeScene");

        Debug.Log("マッチング完了");
    }

    // ゲームシーン遷移
    private IEnumerator TransGmaeScene()
    {
        // 1秒待ってコルーチン中断
        yield return new WaitForSeconds(1.2f);

        // ゲームシーンに遷移
        SceneManager.LoadScene("06_OnlinePlayScene");
    }
}
