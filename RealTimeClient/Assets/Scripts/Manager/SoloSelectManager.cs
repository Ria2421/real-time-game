//---------------------------------------------------------------
// ソロステージ選択マネージャー [ SoloSelectManager.cs ]
// Author:Kenta Nakamoto
// Data:2025/01/11
// Update:2025/01/30
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
using System.IO.Compression;

public class SoloSelectManager : MonoBehaviour
{
    //=====================================
    // フィールド

    /// <summary>
    /// 取得ゴーストデータ
    /// </summary>
    private string getGhostData;

    /// <summary>
    /// プレイステージID
    /// </summary>
    private int playStageID = 1;

    /// <summary>
    /// 各ステージのランキング情報を取得
    /// </summary>
    private List<List<RankingData>> stageRnakings = new List<List<RankingData>>();

    /// <summary>
    /// 最大ステージ数
    /// </summary>
    [SerializeField] private int maxStage;

    /// <summary>
    /// ユーザー名格納用
    /// </summary>
    [SerializeField] private Text[] nameTexts;

    /// <summary>
    /// クリアタイム格納用
    /// </summary>
    [SerializeField] private Text[] clearTimeTexts;

    /// <summary>
    /// ステージ画像格納用
    /// </summary>
    [SerializeField] private Sprite[] stageSprits;

    /// <summary>
    /// ステージ画像
    /// </summary>
    [SerializeField] private Image stageImage;

    /// <summary>
    /// ランキングモデル格納用
    /// </summary>
    [SerializeField] private RankingModel rankingModel;

    /// <summary>
    /// ゴーストオンボタン
    /// </summary>
    [SerializeField] private GameObject onGhostButton;

    /// <summary>
    /// ゴーストオフボタン
    /// </summary>
    [SerializeField] private GameObject offGhostButton;

    /// <summary>
    /// ネクストステージボタン
    /// </summary>
    [SerializeField] private GameObject nextButton;

    /// <summary>
    /// バックステージボタン
    /// </summary>
    [SerializeField] private GameObject backButton;

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

        for (int i=0;i < maxStage; i++)
        {   // ステージ数分のランキング情報リストを生成
            stageRnakings.Add (new List<RankingData>());
        }

        // ランキングデータの取得 (現在はステージ1に固定)
        stageRnakings[0] = await rankingModel.GetRankingAsync(1);

        // ランキング1位のゴーストデータを取得
        UserModel.Instance.GhostData = "";  // リセット
        UserModel.Instance.GhostData = stageRnakings[0][0].GhostData;
        getGhostData = UserModel.Instance.GhostData;

        // 画面に反映
        for (int i = 0; i < stageRnakings[0].Count; i++)
        {
            nameTexts[i].text = stageRnakings[0][i].UserName;   // 名前を格納

            // クリアタイムをテキストに反映
            float clearTIme = (float)stageRnakings[0][i].ClearTime / 1000.0f;
            string decNum = (clearTIme - (int)clearTIme).ToString(".000");
            clearTimeTexts[i].text = ((int)(clearTIme / 60)).ToString("00") + ":" + ((int)clearTIme % 60).ToString("00") + decNum;
        }
    }

    /// <summary>
    /// ステージ切り替え処理
    /// </summary>
    private async void SelectStageButton()
    {
        //--- ボタンの有効切替
        if(playStageID == 1)
        {
            nextButton.GetComponent<Button>().interactable = true;
            backButton.GetComponent<Button>().interactable = false;
        }else if(playStageID == maxStage) 
        {
            nextButton.GetComponent<Button>().interactable = false;
            backButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            nextButton.GetComponent<Button>().interactable = true;
            backButton.GetComponent<Button>().interactable = true;
        }

        //--- 選択ステージのランキング情報を表示

        // 既にランキング情報を取得しているか判断
        if (stageRnakings[playStageID - 1].Count == 0)
        {
            // 選択したステージランキングデータの取得
            stageRnakings[playStageID - 1] = await rankingModel.GetRankingAsync(playStageID);
        }

        // ゴーストデータの切り替え
        UserModel.Instance.GhostData = "";  // リセット
        UserModel.Instance.GhostData = stageRnakings[playStageID - 1][0].GhostData;
        getGhostData = stageRnakings[playStageID - 1][0].GhostData;

        // 画面に反映
        for (int i = 0; i < stageRnakings[playStageID - 1].Count; i++)
        {
            nameTexts[i].text = stageRnakings[playStageID - 1][i].UserName;   // 名前を格納

            // クリアタイムをテキストに反映
            float clearTIme = (float)stageRnakings[playStageID - 1][i].ClearTime / 1000.0f;
            string decNum = (clearTIme - (int)clearTIme).ToString(".000");
            clearTimeTexts[i].text = ((int)(clearTIme / 60)).ToString("00") + ":" + ((int)clearTIme % 60).ToString("00") + decNum;
        }

        // 画像切り替え
        stageImage.sprite = stageSprits[playStageID - 1];
    }

    /// <summary>
    /// ステージ選択ボタン (次へ)
    /// </summary>
    public void OnNextButton()
    {
        playStageID++;
        if (playStageID >= maxStage) playStageID = maxStage;

        SelectStageButton();
    }

    /// <summary>
    /// ステージ選択ボタン (前へ)
    /// </summary>
    public void OnBackButton()
    {
        playStageID--;
        if(playStageID <= 1) playStageID = 1;

        SelectStageButton();
    }

    /// <summary>
    /// プレイボタン押下処理
    /// </summary>
    public void OnPlayButton()
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // ソロプレイモード遷移
        Initiate.DoneFading();
        Initiate.Fade(playStageID.ToString() + "_SoloPlayScene", Color.white, 2.5f);
    }

    /// <summary>
    /// メニューボタン押下処理
    /// </summary>
    public void OnMenuButton()
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // メニューモード遷移
        Initiate.DoneFading();
        Initiate.Fade("2_MenuScene", Color.white, 2.5f);
    }

    /// <summary>
    /// ゴーストオンボタン
    /// </summary>
    public void OnGhostPlayButton()
    {
        UserModel.Instance.GhostData = getGhostData;

        onGhostButton.SetActive(false);
        offGhostButton.SetActive(true);
    }

    /// <summary>
    /// ゴーストオフボタン
    /// </summary>
    public void OnGhostNotButton()
    {
        UserModel.Instance.GhostData = "";

        offGhostButton.SetActive(false);
        onGhostButton.SetActive(true);
    }
}
