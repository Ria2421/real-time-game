//---------------------------------------------------------------
// ゲームマネージャー [ GameManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/11/18
// Update:2024/11/26
//---------------------------------------------------------------
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    //=====================================
    // フィールド

    /// <summary>
    /// ゲーム状態種類
    /// </summary>
    private enum GameState
    {
        None = 0,
        Join,
        Ready,
    }

    /// <summary>
    /// 接続IDをキーにキャラクタのオブジェクトを管理
    /// </summary>
    private Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    /// <summary>
    /// ゲーム状態
    /// </summary>
    private GameState gameState = GameState.None;

    [Header("各種Objectをアタッチ")]

    /// <summary>
    /// ルームモデル格納用
    /// </summary>
    [SerializeField] private RoomModel roomModel;

    /// <summary>
    /// 生成するキャラクタープレハブ
    /// </summary>
    [SerializeField] private GameObject characterPrefab;

    /// <summary>
    /// プレイヤーを格納する親オブジェクト
    /// </summary>
    [SerializeField] private GameObject parentObj;

    /// <summary>
    /// プレイヤーごとのリス地点
    /// </summary>
    [SerializeField] private Transform[] respownList;

    /// <summary>
    /// 通信速度
    /// </summary>
    [SerializeField] private float internetSpeed = 0.1f;

    [Space (40)]
    [Header("===== UI関連 =====")]

    [Space(10)]
    [Header("---- Text ----")]

    /// <summary>
    /// ユーザーID
    /// </summary>
    [SerializeField] private Text idText;

    /// <summary>
    /// 準備状態表示テキスト
    /// </summary>
    [SerializeField] private Text[] readyStateTexts;

    [Space(10)]
    [Header("---- InputField ----")]

    /// <summary>
    /// ID入力欄
    /// </summary>
    [SerializeField] private InputField idInput;

    [Space(10)]
    [Header("---- Button ----")]

    /// <summary>
    /// 入室ボタン
    /// </summary>
    [SerializeField] private Button joinButton;

    /// <summary>
    /// 退室ボタン
    /// </summary>
    [SerializeField] private Button exitButton;

    /// <summary>
    /// 準備完了ボタン
    /// </summary>
    [SerializeField] private Button readyButton;

    /// <summary>
    /// 準備キャンセルボタン
    /// </summary>
    [SerializeField] private Button nonReadyButton;

    //=====================================
    // メソッド

    // 初期処理
    void Start()
    {
        // 各通知が届いた際に行う処理をモデルに登録する
        roomModel.OnJoinedUser += OnJoinedUser;     // 入室
        roomModel.OnReadyUser += OnReadyUser;       // 準備完了
        roomModel.OnNonReadyUser += OnNonReadyUser; // 準備キャンセル
        roomModel.OnExitedUser += OnExitedUser;     // 退室
        roomModel.OnMovedUser += OnMovedUser;       // 移動

        ChangeUI(gameState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 移動データ送信処理
    private async void SendMoveData()
    {
        if (gameState == GameState.None) return;

        var moveData = new MoveData()
        {
            ConnectionId = roomModel.ConnectionId,
            Position = characterList[roomModel.ConnectionId].transform.position,
            Rotation = characterList[roomModel.ConnectionId].transform.eulerAngles
        };

        await roomModel.MoveAsync(moveData);
    }

    // 接続処理
    public async void OnConnect()
    {
        int userId = int.Parse(idText.text);    // 入力したユーザーIDの変換

        // 接続
        await roomModel.ConnectAsync();
        // 入室 (ルーム名とユーザーIDを渡して入室。最終的にはローカルデータのユーザーIDを使用)
        await roomModel.JoinAsync("sampleRoom", userId);

        Debug.Log("入室");
    }

    // 準備完了処理
    public async void OnReady()
    {
        await roomModel.ReadyAsync();

        gameState = GameState.Ready;
        ChangeUI(gameState);
    }

    // 準備キャンセル処理
    public async void OnCancel()
    {
        await roomModel.NonReadyAsync();

        gameState = GameState.Join;
        ChangeUI(gameState);
    }

    // 切断処理
    public async void OnDisconnect()
    {
        CancelInvoke();

        // 退出
        await roomModel.ExitAsync();
        // 切断
        await roomModel.DisconnectionAsync();
        // プレイヤーオブジェクトの削除
        foreach(Transform child in parentObj.transform)
        {
            Destroy(child.gameObject);
        }

        gameState = GameState.None;
        ChangeUI(gameState);
        Debug.Log("退出");
    }

    // --------------------------------------------------------------
    // モデル登録用関数

    // 入室通知時の処理
    private void OnJoinedUser(JoinedUser user)
    {
        Debug.Log(user.JoinOrder + "P");

        // プレイヤーの生成
        GameObject characterObj = Instantiate(characterPrefab, respownList[user.JoinOrder-1].position, Quaternion.Euler(0,180,0));

        characterObj.transform.parent = parentObj.transform;    // 親の設定
        characterList[user.ConnectionId] = characterObj;        // フィールドで保存

        if(user.ConnectionId == roomModel.ConnectionId)
        {
            characterList[roomModel.ConnectionId].gameObject.AddComponent<PlayerManager>();
            gameState = GameState.Join;
            ChangeUI(gameState);
            InvokeRepeating("SendMoveData", 0, internetSpeed);
        }
    }


    // 準備完了通知時の処理
    private void OnReadyUser(JoinedUser user)
    {
        readyStateTexts[user.JoinOrder - 1].text = "準備完了！";
    }

    // 準備キャンセル通知時の処理
    private void OnNonReadyUser(JoinedUser user)
    {
        readyStateTexts[user.JoinOrder - 1].text = "準備中...";
    }

    // 退出通知時の処理
    private void OnExitedUser(Guid connectionId)
    {
        // 位置情報の更新
        if (!characterList.ContainsKey(connectionId)) return;

        Destroy(characterList[connectionId]);   // オブジェクトの破棄
        characterList.Remove(connectionId);     // リストから削除
    }

    // プレイヤーが移動したときの処理
    private void OnMovedUser(MoveData moveData)
    {
        // 位置情報の更新
        if (!characterList.ContainsKey(moveData.ConnectionId)) return;

        characterList[moveData.ConnectionId].gameObject.transform.DOMove(moveData.Position, internetSpeed).SetEase(Ease.Linear);
        characterList[moveData.ConnectionId].gameObject.transform.DORotate(moveData.Rotation, internetSpeed).SetEase(Ease.Linear);
    }

    // 表示UIの変更
    private void ChangeUI(GameState gameState)
    {
        switch (gameState)
        {
            // 退室状態 ---------------------------------------------
            case GameState.None:
                // InputField
                idInput.enabled = true;

                // Text
                for (int i = 0;readyStateTexts.Length > i; i++)
                {
                    readyStateTexts[i].gameObject.SetActive(false);
                }

                // Button
                joinButton.gameObject.SetActive(true);
                exitButton.gameObject.SetActive(false);
                readyButton.gameObject.SetActive(false);
                nonReadyButton.gameObject.SetActive(false);
                break;

            // 入室状態 ---------------------------------------------
            case GameState.Join:
                // InputField
                idInput.enabled = false;

                // Text
                for (int i = 0; readyStateTexts.Length > i; i++)
                {
                    readyStateTexts[i].gameObject.SetActive(true);
                }

                // Button
                joinButton.gameObject.SetActive(false);
                exitButton.gameObject.SetActive(true);
                readyButton.gameObject.SetActive(true);
                nonReadyButton.gameObject.SetActive(false);
                break;

            // 準備完了状態 -----------------------------------------
            case GameState.Ready:
                // InputField
                idInput.enabled = false;

                // Text
                for (int i = 0; readyStateTexts.Length > i; i++)
                {
                    readyStateTexts[i].gameObject.SetActive(true);
                }

                // Button
                joinButton.gameObject.SetActive(false);
                exitButton.gameObject.SetActive(true);
                readyButton.gameObject.SetActive(false);
                nonReadyButton.gameObject.SetActive(true);
                break;
        }
    }
}
