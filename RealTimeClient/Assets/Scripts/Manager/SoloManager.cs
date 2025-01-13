//---------------------------------------------------------------
// ソロマネージャー [ SoloManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/10
// Update:2024/12/10
//---------------------------------------------------------------
using DavidJalbert;
using DG.Tweening;
using KanKikuchi.AudioManager;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoloManager : MonoBehaviour
{
    //===================================
    // フィールド

    /// <summary>
    /// ステージNo
    /// </summary>
    [SerializeField] private int stageID; 

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
    /// 最大ラップ数
    /// </summary>
    [SerializeField] private int maxRapNum = 0;

    /// <summary>
    /// リザルトパネル
    /// </summary>
    [SerializeField] private GameObject resultPanel;

    /// <summary>
    /// 計測タイマーパネル
    /// </summary>
    [SerializeField] private GameObject timerPanel;

    /// <summary>
    /// カウントダウンオブジェ
    /// </summary>
    [SerializeField] private GameObject countDownObj;

    /// <summary>
    /// カウントダウン用スプライト
    /// </summary>
    [SerializeField] private Sprite[] countDownSprits;

    /// <summary>
    /// 時間計測用テキスト
    /// </summary>
    [SerializeField] private Text timerText;

    /// <summary>
    /// ラップ数表示用テキスト
    /// </summary>
    [SerializeField] private Text rapText;

    /// <summary>
    /// ランキングモデル格納用
    /// </summary>
    [SerializeField] private RankingModel rankingModel;

    /// <summary>
    /// 新記録画像
    /// </summary>
    [SerializeField] private GameObject newRecordObj;

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

        // フラグ初期化
        isCount = false;

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
        currentRapNum++;    // ラップ数加算

        if(currentRapNum == maxRapNum + 1)
        {
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

            timer = (float)Math.Round(timer, 3, MidpointRounding.AwayFromZero);
            goalTime = (int)(timer * 1000);

            UserModel userModel = UserModel.Instance;

            // 記録登録処理
            bool isRegist = await rankingModel.RegistClearTimeAsync(stageID, userModel.UserId, goalTime);

            if (isRegist)
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
                // 計測開始・操作可能
                countDownObj.GetComponent<Image>().sprite = countDownSprits[i];
                standardInput.SetActive(true);
                mobileInput.SetActive(true);
                isCount = true;

                // カウント非表示
                Invoke("HiddenCount", 0.6f);
            }
            else
            {
                SEManager.Instance.Play(SEPath.COUNT);
                // 1秒待ってコルーチン中断
                yield return new WaitForSeconds(0.9f);
            }
        }
    }

    /// <summary>
    /// メニュー遷移処理
    /// </summary>
    public void OnBackMenu()
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        SceneManager.LoadScene("02_MenuScene");
    }

    /// <summary>
    /// ステージ選択遷移処理
    /// </summary>
    public void OnBackSelect()
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        SceneManager.LoadScene("03_SoloSelectScene");
    }

    /// <summary>
    /// リトライ遷移処理
    /// </summary>
    public void OnRetryButton() 
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
