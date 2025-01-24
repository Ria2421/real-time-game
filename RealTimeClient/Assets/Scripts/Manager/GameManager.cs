//---------------------------------------------------------------
// ゲームマネージャー [ GameManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/11/18
// Update:2025/01/23
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
using KanKikuchi.AudioManager;

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
        InGame,
        Result
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
    /// 車体オブジェの位置情報
    /// </summary>
    private Transform visualTransform;

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

    /// <summary>
    /// 参加順
    /// </summary>
    private int joinOrder = 0;

    [Header("数値設定")]

    /// <summary>
    /// 通信速度
    /// </summary>
    [SerializeField] private float internetSpeed = 0.1f;

    /// <summary>
    /// 制限時間
    /// </summary>
    [SerializeField] private int timeLimit = 30;

    [Header("各種Objectをアタッチ")]

    /// <summary>
    /// モバイルインプットスクリプト
    /// </summary>
    [SerializeField] private TinyCarMobileInput tinyCarMobileInput;

    /// <summary>
    /// モバイルインプットobj
    /// </summary>
    [SerializeField] private GameObject mobileInputObj;

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
    /// 爆発パーティクル
    /// </summary>
    [SerializeField] private GameObject explosionPrefab;

    /// <summary>
    /// ユーザー名表示用オブジェ
    /// </summary>
    [SerializeField] private GameObject[] nameObjs;

    [Space (25)]
    [Header("===== UI関連 =====")]

    [Space(10)]
    [Header("---- Text ----")]

    /// <summary>
    /// タイマー
    /// </summary>
    [SerializeField] private Text timerText;

    /// <summary>
    /// 撃破通知表示
    /// </summary>
    [SerializeField] private GameObject crushText;

    /// <summary>
    /// レート表示用オブジェ
    /// </summary>
    [SerializeField] private GameObject rateObjs;

    /// <summary>
    /// レートテキスト
    /// </summary>
    [SerializeField] private Text rateText;

    /// <summary>
    /// 符号表示テキスト
    /// </summary>
    [SerializeField] private Text signText;

    /// <summary>
    /// レート増減テキスト
    /// </summary>
    [SerializeField] private Text changeRateText;

    /// <summary>
    /// ユーザー名テキスト
    /// </summary>
    [SerializeField] private Text[] nameTexts;

    [Space(10)]
    [Header("---- Panel ----")]

    /// <summary>
    /// リザルトパネル
    /// </summary>
    [SerializeField] private GameObject resultPanel;

    /// <summary>
    /// 切断表示パネル
    /// </summary>
    [SerializeField] private GameObject disconnectPanel;

    /// <summary>
    /// 残タイム表示パネル
    /// </summary>
    [SerializeField] private GameObject timerPanel;

    [Space(10)]
    [Header("---- Image ----")]

    /// <summary>
    /// 順位表示画像
    /// </summary>
    [SerializeField] private GameObject[] rankImages;

    /// <summary>
    /// カウントダウン画像オブジェ
    /// </summary>
    [SerializeField] private GameObject countDownImageObj;

    /// <summary>
    /// 終了表示画像
    /// </summary>
    [SerializeField] private GameObject endImageObj;

    /// <summary>
    /// カウントダウン画像
    /// </summary>
    [SerializeField] private Image countDownImage;

    /// <summary>
    /// 引き分け表示用オブジェ
    /// </summary>
    [SerializeField] private GameObject drawImageObj;

    [Space(10)]
    [Header("---- Sprit ----")]

    /// <summary>
    /// カウントダウンスプライト
    /// </summary>
    [SerializeField] private Sprite[] countSprits;

    //=====================================
    // メソッド

    /// <summary>
    /// 初期処理
    /// </summary>
    async void Start()
    {
        // BGM再生
        BGMManager.Instance.Pause(BGMPath.MAIN_BGM);
        BGMManager.Instance.Play(BGMPath.MULTI_PLAY);

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
        roomModel.OnTimeCountUser += OnTimeCountUser;   // タイムカウント
        roomModel.OnTimeUpUser += OnTimeUpUser;         // タイムアップ

        // 制限時間の初期化
        timerText.text = timeLimit.ToString();

        // 接続
        await roomModel.ConnectAsync();
        // 入室 (ルーム名とユーザーIDを渡して入室。最終的にはローカルデータのユーザーIDを使用する
        await roomModel.JoinAsync();

        Debug.Log("入室");
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
            Rotation = visualTransform.eulerAngles,
            WheelAngle = wheelAngle.eulerAngles.y,
            IsTurbo = playerTurboParticle.isPlaying,
            IsDrift = playerDriftParticle.isPlaying
        };

        await roomModel.MoveAsync(moveData);
    }

    /// <summary>
    /// 切断処理
    /// </summary>
    public async void OnDisconnect()
    {
        CancelInvoke();

        // 退出
        await roomModel.DisconnectionAsync();

        // プレイヤーオブジェクトの削除
        foreach (Transform child in parentObj.transform)
        {
            Destroy(child.gameObject);
        }

        gameState = GameState.None;
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
        if (characterList.ContainsKey(user.ConnectionId)) return;

        GameObject characterObj;    // 生成されるオブジェクト

        // 自分か他プレイヤーか判断
        if (user.ConnectionId == roomModel.ConnectionId)
        {
            // 参加順の保存
            joinOrder = user.JoinOrder;

            // 自機・操作用オブジェの生成
            characterObj = Instantiate(playerPrefab, respownList[user.JoinOrder - 1].position, Quaternion.Euler(0, 180, 0));
            inputController = Instantiate(inputPrefab, Vector3.zero, Quaternion.identity);

            // カーコントローラーの取得・反映
            playerController = characterObj.transform.GetChild(0).gameObject;
            wheelAngle = characterObj.transform.Find("Visuals/WheelFrontLeft").transform;

            // プレイヤーの位置情報を取得
            visualTransform = characterObj.transform.GetChild(1).gameObject.GetComponent<Transform>();

            // パーティクルの取得
            playerTurboParticle = characterObj.transform.Find("Visuals/ParticlesBoost").GetComponent<ParticleSystem>();
            playerDriftParticle = characterObj.transform.Find("Visuals/ParticlesDrifting").GetComponent<ParticleSystem>();

            // モバイルインプットにカーコントローラーを設定
            tinyCarMobileInput.carController = playerController.GetComponent<TinyCarController>();

            // ユーザー名UIの追従設定 & 名前反映
            nameObjs[user.JoinOrder - 1].GetComponent<NameTracker>().SetTarget(playerController.transform, 1);
            nameTexts[user.JoinOrder - 1].text = user.UserData.Name;
            nameObjs[user.JoinOrder - 1].SetActive(true);

            // UI変更
            gameState = GameState.Join;
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
            characterObj.GetComponent<OtherPlayerManager>().JoinOrder = user.JoinOrder;         // 参加順の保存

            // ユーザー名UIの追従設定 & 名前反映
            nameObjs[user.JoinOrder - 1].GetComponent<NameTracker>().SetTarget(characterObj.transform, 2);
            nameTexts[user.JoinOrder - 1].text = user.UserData.Name;
            nameObjs[user.JoinOrder - 1].SetActive(true);
        }

        characterObj.transform.parent = parentObj.transform;    // 親を設定
        characterList[user.ConnectionId] = characterObj;        // フィールドに保存

        Debug.Log(user.JoinOrder + "P参加");
    }

    /// <summary>
    /// 退出通知受信時の処理
    /// </summary>
    /// <param name="user"></param>
    private void OnExitedUser(JoinedUser user)
    {
        if (!characterList.ContainsKey(user.ConnectionId)) return;  // 退出者オブジェの存在チェック

        if(gameState == GameState.Result)
        {
            Destroy(characterList[user.ConnectionId]);   // オブジェクトの破棄
            characterList.Remove(user.ConnectionId);     // リストから削除
        }
        else
        {
            // 他プレイヤーの切断表示・押下でメニューに戻る
            disconnectPanel.SetActive(true);
        }
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
        characterList[moveData.ConnectionId].transform.DOMove(moveData.Position, internetSpeed).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed, true);
        characterList[moveData.ConnectionId].transform.DORotate(moveData.Rotation, internetSpeed).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed, true);

        // タイヤ角の更新
        characterList[moveData.ConnectionId].transform.Find("wheels/wheel front right").transform.DORotate(new Vector3(0,moveData.WheelAngle,0),internetSpeed).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed, true);
        characterList[moveData.ConnectionId].transform.Find("wheels/wheel front left").transform.DORotate(new Vector3(0, moveData.WheelAngle, 0), internetSpeed).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed, true);

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
        SEManager.Instance.Play(SEPath.START);

        // テキスト変更
        countDownImage.sprite = countSprits[0];
        StartCoroutine("HiddenText");

        // 制限時間表示・ホストはカウント開始
        timerPanel.transform.DOLocalMove(new Vector3(820, 450, 0), 0.6f);
        if (joinOrder == 1)
        {
            InvokeRepeating("CountTime", 1, 1);
        }

        // カメラをトップダウンに変更
        mainCamera.GetComponent<TinyCarCamera>().whatToFollow = playerController.transform;
        // 操作可能にする
        inputController.GetComponent<TinyCarStandardInput>().carController = playerController.GetComponent<TinyCarController>();
        mobileInputObj.SetActive(true);

        gameState = GameState.InGame;
    }

    /// <summary>
    /// ゲーム終了通知受信処理
    /// </summary>
    private void OnEndGameUser(List<ResultData> result)
    {
        gameState = GameState.Result;

        // 操作不能にする
        mobileInputObj.SetActive(false);

        // 終了SE再生
        SEManager.Instance.Play(SEPath.GOAL);

        // 終了表示
        endImageObj.SetActive(true);

        // リザルトパネルに結果を反映 (自分の順位・レートを反映)
        foreach(ResultData resultData in result)
        {
            if(resultData.UserId == roomModel.UserId)
            {
                rateText.text = resultData.Rate.ToString();                         // 取得したレートを表示
                changeRateText.text = Math.Abs(resultData.ChangeRate).ToString();   // 増減レートを表示
                if(resultData.ChangeRate < 0)
                {   // 減算の場合は - を表示
                    signText.text = "-";
                }

                rankImages[resultData.Rank - 1].SetActive(true);        // 該当順位の表示
            }
        }

        // 1秒後にリザルト表示
        StartCoroutine("DisplayResult");
    }

    /// <summary>
    /// プレイヤー撃破通知処理
    /// </summary>
    /// <param name="attackName">撃破した人のPL名</param>
    /// <param name="crushName"> 撃破された人のPL名</param>
    /// <param name="crushID">   撃破された人の接続ID</param>
    private void OnCrushingUser(string attackName, string crushName, Guid crushID, int deadNo)
    {
        // 撃破通知テキストの内容変更・表示する ()
        if(deadNo == 1)
        {
            crushText.GetComponent<Text>().text = attackName + " が " + crushName + "を撃破！";
        }
        else if(deadNo == 2)
        {
            crushText.GetComponent<Text>().text = crushName + "が落下により爆破！";
        }

        // 通知表示Sequenceを作成
        var sequence = DOTween.Sequence();
        sequence.Append(crushText.transform.DOLocalMove(new Vector3(0f, 450f, 0f), 1.5f));
        sequence.Append(crushText.transform.DOLocalMove(new Vector3(0f, 625f, 0f), 0.5f));
        sequence.Play();
        
        // 撃破されたプレイヤーの名前を非表示
        foreach(Text name in nameTexts)
        {
            if(name.text == crushName)
            {
                name.text = "";
            }
        }

        // 爆発アニメーション生成
        if (roomModel.ConnectionId == crushID)
        {   // 自分が撃破されたとき
            SEManager.Instance.Play(SEPath.BOOM);
            Instantiate(explosionPrefab, playerController.transform.position, Quaternion.identity); // 爆発エフェクト
            mainCamera.GetComponent<TinyCarCamera>().whatToFollow = null;                           // カメラを俯瞰に変更
            characterList[crushID].GetComponent<TinyCarExplosiveBody>().explode();                  // 自爆処理
            characterList.Remove(crushID);                                                          // PLリストから削除
        }
        else
        {
            SEManager.Instance.Play(SEPath.BOOM);
            Instantiate(explosionPrefab, characterList[crushID].transform.position, Quaternion.identity);
            Destroy(characterList[crushID]);
            characterList.Remove(crushID);
        }
    }

    /// <summary>
    /// 残タイム通知処理
    /// </summary>
    /// <param name="time"></param>
    private void OnTimeCountUser(int time)
    {   // 残タイムの反映処理
        timerText.text = time.ToString();

        if (3 >= time)
        {
            timerText.color = Color.yellow;
        }
    }

    /// <summary>
    /// タイムアップ通知
    /// </summary>
    private void OnTimeUpUser()
    {
        if (gameState == GameState.Result) return;

        // 終了SE再生
        SEManager.Instance.Play(SEPath.GOAL);

        // 操作不能にする
        mobileInputObj.SetActive(false);

        // 名前非表示
        foreach (GameObject obj in nameObjs)
        {
            obj.SetActive(false);
        }

        drawImageObj.SetActive(true);           // 引き分け表示
        resultPanel.gameObject.SetActive(true); // リザルトパネル表示

        gameState = GameState.Result;
    }

    /// <summary>
    /// ゲームカウント
    /// </summary>
    /// <returns></returns>
    IEnumerator StartCount()
    {
        for (int i = 3; i > 0; i--)
        {
            countDownImage.sprite = countSprits[i];

            SEManager.Instance.Play(SEPath.COUNT);

            // 1秒待ってコルーチン中断
            yield return new WaitForSeconds(1.0f);

            if (i == 1)
            {
                OnStart();
            }
        }
    }

    /// <summary>
    /// 残タイムカウント処理
    /// </summary>
    private async void CountTime()
    {
        timeLimit--;

        await roomModel.TimeCountAsync(timeLimit);

        if(timeLimit <= 0)
        {   // 0以下の時はカウント終了
            CancelInvoke("CountTime");
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

        countDownImageObj.SetActive(false);
    }

    /// <summary>
    /// リザルト表示
    /// </summary>
    /// <returns></returns>
    IEnumerator DisplayResult()
    {
        CancelInvoke("CountTime");

        // 名前表示をすべてOFF
        foreach (GameObject obj in nameObjs)
        {
            obj.SetActive(false);
        }

        // 1秒待ってコルーチン中断
        yield return new WaitForSeconds(1.0f);

        mobileInputObj.SetActive(false);
        endImageObj.SetActive(false);
        rateObjs.SetActive(true);               // レート表示
        resultPanel.gameObject.SetActive(true); // リザルトパネル表示
    }

    /// <summary>
    /// タイトルボタン押下時
    /// </summary>
    public async void OnTitleButton()
    {
        CancelInvoke();

        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // 退出
        await roomModel.ExitAsync();

        // プレイヤーオブジェクトの削除
        foreach (Transform child in parentObj.transform)
        {
            Destroy(child.gameObject);
        }

        gameState = GameState.None;
        Debug.Log("退出");

        // ルームモデルの破棄
        Destroy(GameObject.Find("RoomModel"));

        // タイトルに戻る
        SceneManager.LoadScene("02_MenuScene");
    }
}
