//---------------------------------------------------------------
// ソロマネージャー [ SoloManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/10
// Update:2024/12/10
//---------------------------------------------------------------
using DavidJalbert;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoloManager : MonoBehaviour
{
    //=====================================
    // フィールド

    /// <summary>
    /// 現ラップ数
    /// </summary>
    private int currentRapNum = 1;

    /// <summary>
    /// 時間計測用
    /// </summary>
    private float timer;

    /// <summary>
    /// 計測フラグ
    /// </summary>
    private bool isCount = false;

    /// <summary>
    /// 操作入力オブジェ
    /// </summary>
    [SerializeField] private GameObject inputManager;

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
    /// カウントダウン用テキスト
    /// </summary>
    [SerializeField] private Text countDownText;

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
    public void AddRapCnt()
    {
        currentRapNum++;    // ラップ数加算

        if(currentRapNum == maxRapNum + 1)
        {
            // 終了判定
            isCount = false;                // タイマーストップ
            Invoke("BoomCar", 1);           // 車体爆破
            resultPanel.SetActive(true);    // リザルト表示
            
            // タイマー移動処理
            var sequence = DOTween.Sequence();
            sequence.Append(timerPanel.transform.DOScale(1.7f, 0.7f));
            sequence.Append(timerPanel.transform.DOLocalMove(new Vector3(0,80,0), 0.5f));
            sequence.Play();

            Debug.Log("owari");
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
        countDownText.text = "";
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
        for (int i = 3; i > 0; i--)
        {
            countDownText.text = i.ToString();

            // 1秒待ってコルーチン中断
            yield return new WaitForSeconds(1.0f);

            if (i == 1)
            {
                // 計測開始・操作可能に
                countDownText.text = "GO!";
                inputManager.SetActive(true);
                isCount = true;

                // カウント非表示
                Invoke("HiddenCount", 0.6f);
            }
        }
    }

    /// <summary>
    /// メニュー遷移処理
    /// </summary>
    public void OnBackMenu()
    {
        SceneManager.LoadScene("02_MenuScene");
    }
}
