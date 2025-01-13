//---------------------------------------------------------------
// ソロステージ選択マネージャー [ SoloSelectManager.cs ]
// Author:Kenta Nakamoto
// Data:2025/01/11
// Update:2025/01/12
//---------------------------------------------------------------
using Cysharp.Net.Http;
using Grpc.Net.Client;
using KanKikuchi.AudioManager;
using MagicOnion.Client;
using Shared.Interfaces.Services;
using Shared.Model.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoloSelectManager : MonoBehaviour
{
    //=====================================
    // フィールド

    /// <summary>
    /// ユーザー名格納用
    /// </summary>
    [SerializeField] private Text[] nameTexts;

    /// <summary>
    /// クリアタイム格納用
    /// </summary>
    [SerializeField] private Text[] clearTimeTexts;

    /// <summary>
    /// ランキングモデル格納用
    /// </summary>
    [SerializeField] private RankingModel rankingModel;

    //=====================================
    // メソッド

    /// <summary>
    /// 初期処理
    /// </summary>
    async void Start()
    {
        //再生中のBGMの名前を全て取得
        var currentBGMNames = BGMManager.Instance.GetCurrentAudioNames();

        if (currentBGMNames[0] != "MainBGM")
        {   // MainBGMを再開
            BGMManager.Instance.Stop(BGMPath.TIME_ATTACK);
            BGMManager.Instance.Stop(BGMPath.MULTI_PLAY);
            BGMManager.Instance.Play(BGMPath.MAIN_BGM, 0.75f, 0, 1, true, true);
        }

        // ランキングデータの取得
        List<RankingData> rankingDatas = await rankingModel.GetRankingAsync(1);

        // 画面に反映
        for (int i = 0; i < rankingDatas.Count; i++)
        {
            nameTexts[i].text = rankingDatas[i].UserName;   // 名前格納

            // クリアタイムをテキストに反映
            float clearTIme = (float)rankingDatas[i].ClearTime / 1000.0f;
            string decNum = (clearTIme - (int)clearTIme).ToString(".000");
            clearTimeTexts[i].text = ((int)(clearTIme / 60)).ToString("00") + ":" + ((int)clearTIme % 60).ToString("00") + decNum;
            
        }
    }

    /// <summary>
    /// プレイボタン押下処理
    /// </summary>
    public void OnPlayButton()
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // ソロプレイモード遷移
        SceneManager.LoadScene("04_SoloPlayScene");
    }

    /// <summary>
    /// メニューボタン押下処理
    /// </summary>
    public void OnMenuButton()
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // メニューモード遷移
        SceneManager.LoadScene("02_MenuScene");
    }
}
