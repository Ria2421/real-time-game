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
using RealTimeServer.Model.Entity;
using DavidJalbert;

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
        InGame
    }

    /// <summary>
    /// 接続IDをキーにキャラクタのオブジェクトを管理
    /// </summary>
    private Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    /// <summary>
    /// ゲーム状態
    /// </summary>
    private GameState gameState = GameState.None;

    /// <summary>
    /// 補正pos
    /// </summary>
    private Vector3 posCorrection = new Vector3(0.0f,-0.9f,0.0f);

    /// <summary>
    /// プレイヤーコントローラー
    /// </summary>
    private GameObject playerController;

    /// <summary>
    /// 操作コントローラー
    /// </summary>
    private GameObject inputController;

    /// <summary>
    /// ホイールの角度取得用
    /// </summary>
    private Transform wheelAngle;

    [Header("各種Objectをアタッチ")]

    /// <summary>
    /// ルームモデル格納用
    /// </summary>
    [SerializeField] private RoomModel roomModel;

    /// <summary>
    /// 生成するプレイヤーのキャラクタープレハブ
    /// </summary>
    [SerializeField] private GameObject playerPrefab;

    /// <summary>
    /// 生成する他プレイヤーのキャラクタープレハブ
    /// </summary>
    [SerializeField] private GameObject otherPrefab;

    /// <summary>
    /// 入力処理プレハブ
    /// </summary>
    [SerializeField] private GameObject inputPrefab;

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

    /// <summary>
    /// 制限時間
    /// </summary>
    [SerializeField] private float timeLimit = 3.0f;

    [Space (40)]
    [Header("===== UI関連 =====")]

    [Space(10)]
    [Header("---- Text ----")]

    /// <summary>
    /// ユーザーID
    /// </summary>
    [SerializeField] private Text idText;

    /// <summary>
    /// タイマー
    /// </summary>
    [SerializeField] private Text timerText;

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

    //=====================================
    // メソッド

    // 初期処理
    void Start()
    {
        // 各通知が届いた際に行う処理をモデルに登録する
        roomModel.OnJoinedUser += OnJoinedUser;         // 入室
        roomModel.OnExitedUser += OnExitedUser;         // 退室
        roomModel.OnMovedUser += OnMovedUser;           // 移動
        roomModel.OnInGameUser += OnInGameUser;         // インゲーム
        roomModel.OnStartGameUser += OnStartGameUser;   // ゲームスタート

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

        // 移動データの作成
        var moveData = new MoveData()
        {
            ConnectionId = roomModel.ConnectionId,
            Position = playerController.transform.position + posCorrection,
            Rotation = playerController.transform.eulerAngles,
            WheelAngle = wheelAngle.eulerAngles.y
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

    // 切断処理
    public async void OnDisconnect()
    {
        CancelInvoke();

        // 退出
        await roomModel.ExitAsync();

        // プレイヤーオブジェクトの削除
        foreach (Transform child in parentObj.transform)
        {
            Destroy(child.gameObject);
        }

        gameState = GameState.None;
        ChangeUI(gameState);
        Debug.Log("退出");
    }

    // ゲームスタート通知処理
    public async void OnStart()
    {
        await roomModel.StartAsync();
    }

    // --------------------------------------------------------------
    // モデル登録用関数

    // 入室通知時の処理
    private void OnJoinedUser(JoinedUser user)
    {
        Debug.Log(user.JoinOrder + "P");

        GameObject characterObj;    // 生成されるオブジェ

        // 自分か他プレイヤーか
        if (user.ConnectionId == roomModel.ConnectionId)
        {
            // 自機・操作用オブジェの生成
            characterObj = Instantiate(playerPrefab, respownList[user.JoinOrder - 1].position, Quaternion.Euler(0, 180, 0));
            inputController = Instantiate(inputPrefab, Vector3.zero, Quaternion.identity);

            // カーコントローラーの取得・反映
            playerController = characterObj.transform.GetChild(0).gameObject;
            wheelAngle = characterObj.transform.Find("Visuals/WheelFrontLeft").transform;

            // UI変更
            gameState = GameState.Join;
            ChangeUI(gameState);
            Debug.Log("自機生成完了");

            // 位置送信開始
            InvokeRepeating("SendMoveData", 0.5f, internetSpeed);
        }
        else
        {
            // 他プレイヤーの生成
            characterObj = Instantiate(otherPrefab, respownList[user.JoinOrder - 1].position, Quaternion.Euler(0, 180, 0));
        }

        characterObj.transform.parent = parentObj.transform;    // 親の設定
        characterList[user.ConnectionId] = characterObj;        // フィールドで保存
    }

    // 退出通知時の処理
    private void OnExitedUser(JoinedUser user)
    {
        // 位置情報の更新
        if (!characterList.ContainsKey(user.ConnectionId)) return;  // 退出者オブジェの存在チェック

        Destroy(characterList[user.ConnectionId]);   // オブジェクトの破棄
        characterList.Remove(user.ConnectionId);     // リストから削除
    }

    // プレイヤーが移動したときの処理
    private void OnMovedUser(MoveData moveData)
    {
        // 位置情報の更新
        if (!characterList.ContainsKey(moveData.ConnectionId)) return;

        // 本体位置の更新
        characterList[moveData.ConnectionId].transform.DOMove(moveData.Position, internetSpeed).SetEase(Ease.Linear);
        characterList[moveData.ConnectionId].transform.DORotate(moveData.Rotation, internetSpeed).SetEase(Ease.Linear);

        // タイヤ角の更新
        characterList[moveData.ConnectionId].transform.Find("wheels/wheel front right").transform.DORotate(new Vector3(0,moveData.WheelAngle,0),internetSpeed).SetEase(Ease.Linear);
        characterList[moveData.ConnectionId].transform.Find("wheels/wheel front left").transform.DORotate(new Vector3(0, moveData.WheelAngle, 0), internetSpeed).SetEase(Ease.Linear);
    }

    /// <summary>
    /// インゲーム通知処理
    /// </summary>
    private void OnInGameUser()
    {
        // カウントダウン開始
        Debug.Log("カウントダウン");
        StartCoroutine("StartCount");
    }

    /// <summary>
    /// ゲームスタート通知処理
    /// </summary>
    private void OnStartGameUser()
    {
        // テキスト変更
        timerText.text = "Start!";
        StartCoroutine("HiddenText");
        // 操作可能にする
        inputController.GetComponent<TinyCarStandardInput>().carController = playerController.GetComponent<TinyCarController>();
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

                // Button
                joinButton.gameObject.SetActive(true);
                exitButton.gameObject.SetActive(false);
                break;

            // 入室状態 ---------------------------------------------
            case GameState.Join:
                // InputField
                idInput.enabled = false;

                // Button
                joinButton.gameObject.SetActive(false);
                exitButton.gameObject.SetActive(true);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// ゲームカウント
    /// </summary>
    /// <returns></returns>
    IEnumerator StartCount()
    {
        for (int i = 3; i > 0; i--)
        {
            timerText.text = i.ToString();

            // 1秒待ってコルーチン中断
            yield return new WaitForSeconds(1.0f);

            if (i == 1)
            {
                OnStart();
            }
        }
    }

    /// <summary>
    /// タイマーテキスト非表示処理
    /// </summary>
    /// <returns></returns>
    IEnumerator HiddenText()
    {
        // 1秒待ってコルーチン中断
        yield return new WaitForSeconds(0.8f);

        timerText.text = "";
    }
}
