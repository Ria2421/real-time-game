//---------------------------------------------------------------
// タイトルマネージャー [ MatchingManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/10
// Update:2025/01/30
//---------------------------------------------------------------
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
    /// プレイステージID
    /// </summary>
    private int playStageID = 0;

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
    /// キャンセルボタンオブジェ
    /// </summary>
    [SerializeField] private GameObject cancelObj;

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

        Invoke("StartMatching",2.0f);
    }

    /// <summary>
    /// 定期更新処理
    /// </summary>
    private void FixedUpdate()
    {
        // 車画像を回す
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

        // 切断 (鯖から帰ってきたらシーン移動にする)
        await roomModel.DisconnectionAsync();

        // メニューシーンに遷移
        Initiate.DoneFading();
        Initiate.Fade("2_MenuScene", Color.white, 2.5f);

        Debug.Log("マッチング中止");
    }

    /// <summary>
    /// マッチング完了通知受信時の処理
    /// </summary>
    /// <param name="roomName"></param>
    private async void OnMatchingUser(string roomName,int stageID)
    {
        roomModel.RoomName = roomName;  // 発行されたルーム名を保存
        playStageID = stageID;          // ステージIDを保存

        // SE再生
        SEManager.Instance.Play(SEPath.MATCHING_COMPLETE);
        // 表示切替
        cancelObj.SetActive(false);
        matchingTextObj.SetActive(false);
        completeTextObj.SetActive(true);

        // 退出
        await roomModel.ExitAsync();

        StartCoroutine("TransGmaeScene");

        Debug.Log("マッチング完了");
    }

    /// <summary>
    /// ゲームシーン遷移
    /// </summary>
    /// <returns></returns>
    private IEnumerator TransGmaeScene()
    {
        // 1秒待ってコルーチン中断
        yield return new WaitForSeconds(1.2f);

        // playStageIDに応じて対応するゲームシーンに遷移
        Initiate.DoneFading();
        Initiate.Fade(playStageID.ToString() + "_OnlinePlayScene", Color.white, 2.5f);
    }

    /// <summary>
    /// マッチング開始処理
    /// </summary>
    private async void StartMatching()
    {
        // 接続
        await roomModel.ConnectAsync();
        // マッチング
        await roomModel.JoinLobbyAsync(UserModel.Instance.UserId);
        // キャンセルボタン有効化
        cancelButton.interactable = true;

        Debug.Log("マッチング開始");
    }
}
