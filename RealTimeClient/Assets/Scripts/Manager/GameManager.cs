//---------------------------------------------------------------
// ゲームマネージャー [ GameManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/11/18
// Update:2024/12/05
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
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks.Triggers;

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
    /// ルームモデル格納用
    /// </summary>
    private RoomModel roomModel;

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

    /// <summary>
    /// プレイヤーのターボパーティクル
    /// </summary>
    private ParticleSystem playerTurboParticle;

    /// <summary>
    /// プレイヤーのドリフトパーティクル
    /// </summary>
    private ParticleSystem playerDriftParticle;

    [Header("各種Objectをアタッチ")]

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
    /// メインカメラ
    /// </summary>
    [SerializeField] private GameObject mainCamera;

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

    /// <summary>
    /// 爆発パーティクル
    /// </summary>
    [SerializeField] private GameObject explosionPrefab;

    [Space (25)]
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

    /// <summary>
    /// ランキング表示
    /// </summary>
    [SerializeField] private Text[] rankTexts;

    /// <summary>
    /// 撃破通知表示
    /// </summary>
    [SerializeField] private GameObject crushText;

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

    [Space(10)]
    [Header("---- Panel ----")]

    /// <summary>
    /// リザルトパネル
    /// </summary>
    [SerializeField] private GameObject resultPanel;

    //=====================================
    // メソッド

    /// <summary>
    /// 初期処理
    /// </summary>
    async void Start()
    {
        // ルームモデルの取得
        roomModel = GameObject.Find("RoomModel").GetComponent<RoomModel>();

        // 各通知が届いた際に行う処理をモデルに登録する
        roomModel.OnJoinedUser += OnJoinedUser;         // 入室
        roomModel.OnExitedUser += OnExitedUser;         // 退室
        roomModel.OnMovedUser += OnMovedUser;           // 移動
        roomModel.OnInGameUser += OnInGameUser;         // インゲーム
        roomModel.OnStartGameUser += OnStartGameUser;   // ゲームスタート
        roomModel.OnEndGameUser += OnEndGameUser;       // ゲーム終了
        roomModel.OnCrushingUser += OnCrushingUser;     // 撃破

        ChangeUI(gameState);

        // 接続
        await roomModel.ConnectAsync();
        // 入室 (ルーム名とユーザーIDを渡して入室。最終的にはローカルデータのユーザーIDを使用
        await roomModel.JoinAsync();

        Debug.Log("入室");
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {

    }

    /// <summary>
    /// 移動データ送信処理
    /// </summary>
    private async void SendMoveData()
    {
        if (gameState == GameState.None) return;

        // 移動データの作成
        var moveData = new MoveData()
        {
            ConnectionId = roomModel.ConnectionId,
            Position = playerController.transform.position + posCorrection,
            Rotation = playerController.transform.eulerAngles,
            WheelAngle = wheelAngle.eulerAngles.y,
            IsTurbo = playerTurboParticle.isPlaying,
            IsDrift = playerDriftParticle.isPlaying
        };

        await roomModel.MoveAsync(moveData);
    }

    /// <summary>
    /// 接続処理
    /// </summary>
    public async void OnConnect()
    {

    }

    /// <summary>
    /// 切断処理
    /// </summary>
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

    /// <summary>
    /// ゲームスタート通知処理
    /// </summary>
    public async void OnStart()
    {
        await roomModel.GameStartAsync();
    }

    // --------------------------------------------------------------
    // モデル登録用関数

    /// <summary>
    /// 入室通知受信時の処理
    /// </summary>
    /// <param name="user"></param>
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

            // パーティクルの取得
            playerTurboParticle = characterObj.transform.Find("Visuals/ParticlesBoost").GetComponent<ParticleSystem>();
            playerDriftParticle = characterObj.transform.Find("Visuals/ParticlesDrifting").GetComponent<ParticleSystem>();

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
            characterObj.GetComponent<OtherPlayerManager>().ConnectionID = user.ConnectionId;   // 接続IDの保存
            characterObj.GetComponent<OtherPlayerManager>().UserName = user.UserData.Name;      // ユーザー名の保存
        }

        characterObj.transform.parent = parentObj.transform;    // 親の設定
        characterList[user.ConnectionId] = characterObj;        // フィールドで保存
    }

    /// <summary>
    /// 退出通知受信時の処理
    /// </summary>
    /// <param name="user"></param>
    private void OnExitedUser(JoinedUser user)
    {
        // 位置情報の更新
        if (!characterList.ContainsKey(user.ConnectionId)) return;  // 退出者オブジェの存在チェック

        Destroy(characterList[user.ConnectionId]);   // オブジェクトの破棄
        characterList.Remove(user.ConnectionId);     // リストから削除
    }

    /// <summary>
    /// プレイヤーが移動したときの処理
    /// </summary>
    /// <param name="moveData"></param>
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

        // パーティクルの取得・更新
        characterList[moveData.ConnectionId].GetComponent<OtherPlayerManager>().playDrift(moveData.IsDrift);
        characterList[moveData.ConnectionId].GetComponent<OtherPlayerManager>().playTurbo(moveData.IsTurbo);
    }

    /// <summary>
    /// インゲーム通知受信処理
    /// </summary>
    private void OnInGameUser()
    {
        // カウントダウン開始
        Debug.Log("カウントダウン");
        StartCoroutine("StartCount");
    }

    /// <summary>
    /// ゲームスタート通知受信処理
    /// </summary>
    private void OnStartGameUser()
    {
        // テキスト変更
        timerText.text = "開始!";
        StartCoroutine("HiddenText");

        // カメラをトップダウンに変更
        mainCamera.GetComponent<TinyCarCamera>().whatToFollow = playerController.transform;
        // 操作可能にする
        inputController.GetComponent<TinyCarStandardInput>().carController = playerController.GetComponent<TinyCarController>();
    }

    /// <summary>
    /// ゲーム終了通知受信処理
    /// </summary>
    private void OnEndGameUser(Dictionary<int, string> result)
    {
        // 操作不能にする
        playerController.GetComponent<Rigidbody>().isKinematic = true;

        //++ 終了SE再生

        // 終了表示
        timerText.text = "終了!";

        // リザルトパネルに結果を反映
        for (int i = 0; i < result.Count; i++)
        {
            rankTexts[i].text = result[i + 1];
        }

        // 1秒後にリザルト表示
        StartCoroutine("DisplayResult");
    }

    /// <summary>
    /// プレイヤー撃破通知処理
    /// </summary>
    /// <param name="attackName">撃破した人のPL名</param>
    /// <param name="cruchName"> 撃破された人のPL名</param>
    /// <param name="crushID">   撃破された人の接続ID</param>
    private void OnCrushingUser(string attackName, string cruchName, Guid crushID)
    {
        // 撃破通知テキストの内容変更・表示
        crushText.GetComponent<Text>().text = attackName + " が " + cruchName + "を撃破！";

        // 通知表示Sequenceを作成
        var sequence = DOTween.Sequence();
        sequence.Append(crushText.transform.DOLocalMove(new Vector3(0f, 450f, 0f), 1.5f));
        sequence.Append(crushText.transform.DOLocalMove(new Vector3(0f, 625f, 0f), 0.5f));
        sequence.Play();

        // 爆発アニメーション生成
        if (roomModel.ConnectionId == crushID)
        {   // 自分が撃破されたとき
            Instantiate(explosionPrefab, playerController.transform.position, Quaternion.identity); // 爆発エフェクト
            mainCamera.GetComponent<TinyCarCamera>().whatToFollow = null;                           // カメラを俯瞰に変更
            characterList[crushID].GetComponent<TinyCarExplosiveBody>().explode();                  // 自爆処理
            characterList.Remove(crushID);                                                          // PLリストから削除
        }
        else
        {
            Instantiate(explosionPrefab, characterList[crushID].transform.position, Quaternion.identity);
            Destroy(characterList[crushID]);
            characterList.Remove(crushID);
        }
    }

    /// <summary>
    /// 表示UIの変更
    /// </summary>
    /// <param name="gameState"></param>
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

    /// <summary>
    /// リザルト表示
    /// </summary>
    /// <returns></returns>
    IEnumerator DisplayResult()
    {
        // 1秒待ってコルーチン中断
        yield return new WaitForSeconds(1.0f);

        timerText.text = "";    // 終了非表示
        resultPanel.gameObject.SetActive(true); // リザルトパネル表示
    }

    /// <summary>
    /// タイトルボタン押下時
    /// </summary>
    public async void OnTitleButton()
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

        // ルームモデルの破棄
        Destroy(GameObject.Find("RoomModel"));

        // タイトルに戻る
        SceneManager.LoadScene("02_MenuScene");
    }
}
