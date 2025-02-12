//---------------------------------------------------------------
// ソロマネージャー [ SoloManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/10
// Update:2025/01/27
//---------------------------------------------------------------
using DavidJalbert;
using DG.Tweening;
using KanKikuchi.AudioManager;
using Newtonsoft.Json;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO.Compression;

public class SoloManager : MonoBehaviour
{
    //===================================
    // フィールド

    [Header("===== StageOption =====")]

    /// <summary>
    /// ステージNo
    /// </summary>
    [SerializeField] private int stageID;

    /// <summary>
    /// 最大ラップ数
    /// </summary>
    [SerializeField] private int maxRapNum = 0;

    /// <summary>
    /// ゴーストデータ記録間隔
    /// </summary>
    [SerializeField] private float saveSpeed;

    /// <summary>
    /// 現ラップ数
    /// </summary>
    private int currentRapNum = 1;

    /// <summary>
    /// 時間計測用
    /// </summary>
    private float timer;

    /// <summary>
    /// ゴールタイム保存
    /// </summary>
    private int goalTime = 0;

    /// <summary>
    /// 計測フラグ
    /// </summary>
    private bool isCount = false;

    /// <summary>
    /// ゴーストデータリスト
    /// </summary>
    private List<GhostData> ghostList = new List<GhostData>();

    /// <summary>
    /// 再生ゴーストデータ
    /// </summary>
    private List<GhostData> playGhost = new List<GhostData>();

    /// <summary>
    /// ゴーストデータカウント
    /// </summary>
    private int ghostCnt = 0;

    /// <summary>
    /// ゴースト表示補正座標
    /// </summary>
    private Vector3 ghostCorrection;

    [Space(25)]
    [Header("===== DataObject =====")]

    /// <summary>
    /// 基本入力管理オブジェ
    /// </summary>
    [SerializeField] private GameObject standardInput;

    /// <summary>
    /// モバイル入力管理オブジェ
    /// </summary>
    [SerializeField] private GameObject mobileInput;

    /// <summary>
    /// 車体爆破マネージャー
    /// </summary>
    [SerializeField] private TinyCarExplosiveBody boomManager;

    /// <summary>
    /// ランキングモデル格納用
    /// </summary>
    [SerializeField] private RankingModel rankingModel;

    /// <summary>
    /// 位置情報取得オブジェ
    /// </summary>
    [SerializeField] private Transform visualObj;

    /// <summary>
    /// タイヤ角取得オブジェ
    /// </summary>
    [SerializeField] private Transform wheelRot;

    /// <summary>
    /// ゴースト車オブジェ
    /// </summary>
    [SerializeField] private GameObject ghostCarObj;

    /// <summary>
    /// ゴースト車位置情報
    /// </summary>
    [SerializeField] private Transform ghostCatTrs;

    /// <summary>
    /// ゴースト車位置情報
    /// </summary>
    [SerializeField] private Transform ghostWheelR;

    /// <summary>
    /// ゴースト車位置情報
    /// </summary>
    [SerializeField] private Transform ghostWheelL;

    [Space(25)]
    [Header("===== UI =====")]

    [Space(10)]
    [Header("---- Panel ----")]

    /// <summary>
    /// リザルトパネル
    /// </summary>
    [SerializeField] private GameObject resultPanel;

    /// <summary>
    /// 計測タイマーパネル
    /// </summary>
    [SerializeField] private GameObject timerPanel;

    [Space(10)]
    [Header("---- Image ----")]

    /// <summary>
    /// カウントダウンオブジェ
    /// </summary>
    [SerializeField] private GameObject countDownObj;

    /// <summary>
    /// カウントダウン用スプライト
    /// </summary>
    [SerializeField] private Sprite[] countDownSprits;

    /// <summary>
    /// 新記録画像
    /// </summary>
    [SerializeField] private GameObject newRecordObj;

    [Space(10)]
    [Header("---- Text ----")]

    /// <summary>
    /// 時間計測用テキスト
    /// </summary>
    [SerializeField] private Text timerText;

    /// <summary>
    /// ラップ数表示用テキスト
    /// </summary>
    [SerializeField] private Text rapText;

    //=====================================
    // メソッド

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        // メインBGM一時停止
        BGMManager.Instance.Pause(BGMPath.MAIN_BGM);
        BGMManager.Instance.Play(BGMPath.TIME_ATTACK);

        // 変数初期化処理
        isCount = false;
        ghostCnt = 0;
        ghostCorrection = new Vector3(0, -0.74f, 0);

        rapText.text = currentRapNum.ToString() + " / " + maxRapNum.ToString();

        // 再生するゴーストデータを取得
        if (UserModel.Instance.GhostData != "")
        {
            playGhost = JsonConvert.DeserializeObject<List<GhostData>>(UserModel.Instance.GhostData);
            Debug.Log("ゴーストデータ取得");
        }
        else
        {
            ghostCarObj.SetActive(false);
        }

        // カウントダウン開始
        Debug.Log("カウントダウン");
        StartCoroutine("StartCount");
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        if (!isCount) return;

        // timerを利用して経過時間を計測・表示
        timer += Time.deltaTime;
        DisplayTime(timer);
    }

    /// <summary>
    /// ラップ数加算処理
    /// </summary>
    public async void AddRapCnt()
    {
        currentRapNum++;    // ラップ加算

        if(currentRapNum == maxRapNum + 1)
        {
            CancelInvoke("SaveGhost");

            // SE再生
            SEManager.Instance.Play(SEPath.GOAL);

            // 終了判定
            isCount = false;                // タイマーストップ
            Invoke("BoomCar", 1);           // 車体爆破
            resultPanel.SetActive(true);    // リザルト表示
            mobileInput.SetActive(false);   // モバイルUI非表示
            
            // タイマー移動処理
            var sequence = DOTween.Sequence();
            sequence.Append(timerPanel.transform.DOScale(1.7f, 0.7f));
            sequence.Append(timerPanel.transform.DOLocalMove(new Vector3(0,-25,0), 0.5f));
            sequence.Play();

            // クリアタイムをm/secに変換する
            timer = (float)Math.Round(timer, 3, MidpointRounding.AwayFromZero);
            goalTime = (int)(timer * 1000);

            UserModel userModel = UserModel.Instance;

            // 送信用ゴーストデータの作成
            SaveGhost();
            string ghostData = JsonConvert.SerializeObject(ghostList);

            // 記録登録処理
            RegistResult result = await rankingModel.RegistClearTimeAsync(stageID, userModel.UserId, goalTime, ghostData);

            if (result.timeRegistFlag)
            {   // newRecord表示
                SEManager.Instance.Play(SEPath.NEW_RECORD);
                newRecordObj.SetActive(true);
                newRecordObj.transform.DOScale(1.3f, 1.5f).SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);
            }

            Debug.Log("goal : " + goalTime.ToString() + "m/sec");
        }
        else
        {
            Debug.Log("現ラップ数" + currentRapNum);
            rapText.text = currentRapNum.ToString() + " / " + maxRapNum.ToString();
        }
    }

    /// <summary>
    /// 制限時間を更新して[分:秒]で表示する
    /// </summary>
    private void DisplayTime(float limitTime)
    {
        // 引数で受け取った値を[分:秒]に変換して表示する
        // ToString("00")でゼロプレースフォルダーして、１桁のときは頭に0をつける
        string decNum = (limitTime - (int)limitTime).ToString(".000");
        timerText.text = ((int)(limitTime / 60)).ToString("00") + ":" + ((int)limitTime % 60).ToString("00") + decNum;
    }

    /// <summary>
    /// カウントダウン非表示処理
    /// </summary>
    /// <param name="obj">非表示にしたいオブジェ</param>
    private void HiddenCount()
    {
        countDownObj.SetActive(false);
    }

    /// <summary>
    /// 車体爆破処理
    /// </summary>
    private void BoomCar()
    {
        boomManager.explode();
    }

    /// <summary>
    /// カウントダウン処理
    /// </summary>
    /// <returns></returns>
    IEnumerator StartCount()
    {
        for (int i = 0; i < 4; i++)
        {
            countDownObj.GetComponent<Image>().sprite = countDownSprits[i];

            if (i == 3)
            {
                SEManager.Instance.Play(SEPath.START);
                // 計測開始・操作可能・ゴーストデータ保存処理起動する
                countDownObj.GetComponent<Image>().sprite = countDownSprits[i];

                if (UserModel.Instance.GhostData != "")
                {   // ゴーストデータがある時のみ再生
                    InvokeRepeating("PlayGhost",0,saveSpeed);
                }

                standardInput.SetActive(true);
                mobileInput.SetActive(true);
                isCount = true;
                InvokeRepeating("SaveGhost", 0.1f, saveSpeed);

                // カウント非表示
                Invoke("HiddenCount", 0.6f);
            }
            else
            {
                SEManager.Instance.Play(SEPath.COUNT);
                // 0.9秒待ってコルーチン中断
                yield return new WaitForSeconds(0.9f);
            }
        }
    }

    /// <summary>
    /// ゴーストデータ保存処理
    /// </summary>
    private void SaveGhost()
    {
        GhostData ghostData = new GhostData();
        ghostData.Pos = visualObj.position;        // 位置
        ghostData.Rot = visualObj.eulerAngles;     // 角度
        ghostData.WRot = wheelRot.localEulerAngles.y;   // タイヤ角

        ghostList.Add(ghostData);
    }

    /// <summary>
    /// ゴースト再生処理
    /// </summary>
    private void PlayGhost()
    {
        // 本体位置の更新
        ghostCarObj.transform.DOMove(playGhost[ghostCnt].Pos + ghostCorrection, saveSpeed).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed, true);
        ghostCarObj.transform.DORotate(playGhost[ghostCnt].Rot, saveSpeed).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed, true);

        // タイヤ角の更新
        ghostWheelL.transform.localEulerAngles = new Vector3 (ghostWheelL.transform.localEulerAngles.x,playGhost[ghostCnt].WRot,0);
        ghostWheelR.transform.localEulerAngles = new Vector3(ghostWheelR.transform.localEulerAngles.x, playGhost[ghostCnt].WRot, 0);

        ghostCnt++;

        if (playGhost.Count - 1 < ghostCnt)
        {   // 再生するデータが無い時は再生停止
            CancelInvoke("PlayGhost");
            return;
        }
    }

    /// <summary>
    /// メニュー遷移処理
    /// </summary>
    public void OnBackMenu()
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        Initiate.DoneFading();
        Initiate.Fade("2_MenuScene", Color.white, 2.5f);
    }

    /// <summary>
    /// ステージ選択遷移処理
    /// </summary>
    public void OnBackSelect()
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        Initiate.DoneFading();
        Initiate.Fade("3_SoloSelectScene", Color.white, 2.5f);
    }

    /// <summary>
    /// リトライ遷移処理
    /// </summary>
    public void OnRetryButton() 
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        Initiate.DoneFading();
        Initiate.Fade(SceneManager.GetActiveScene().name, Color.white, 1.0f);
    }

    /// <summary>
    /// パネル閉じる処理
    /// </summary>
    /// <param name="panel"></param>
    public void OnCloseButton(GameObject panel)
    {
        panel.SetActive(false);
    }

    /// <summary>
    /// メニューボタン押下時
    /// </summary>
    /// <param name="menuPanel"></param>
    public void OnMenuButton(GameObject menuPanel)
    {
        if (menuPanel.activeSelf)
        {
            menuPanel.SetActive(false);
        }
        else
        {
            menuPanel.SetActive(true);
        }
    }
}
