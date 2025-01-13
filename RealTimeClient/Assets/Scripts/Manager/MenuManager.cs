//---------------------------------------------------------------
// メニューマネージャー [ MenuManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/10
// Update:2024/12/10
//---------------------------------------------------------------
using KanKikuchi.AudioManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    //=====================================
    // フィールド

    [Header("---- Button ----")]

    // メニューボタン
    [SerializeField] private Button acountButton;   // アカウント
    [SerializeField] private Button shopButton;     // ショップ
    [SerializeField] private Button optionButton;   // オプション

    //=====================================
    // メソッド

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        // BGM再生
        //再生中のBGMの名前を全て取得
        var currentBGMNames = BGMManager.Instance.GetCurrentAudioNames();

        if (currentBGMNames[0] != "MainBGM")
        {   // MainBGMを再開
            BGMManager.Instance.Stop(BGMPath.TIME_ATTACK);
            BGMManager.Instance.Stop(BGMPath.MULTI_PLAY);
            BGMManager.Instance.Play(BGMPath.MAIN_BGM, 0.75f, 0, 1, true, true);
        }

        //++ プロフィール情報取得・UI反映

        //++ ショップ情報取得・UI反映
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        
    }

    //-----------------------------
    // ボタン押下処理

    /// <summary>
    /// パネル表示
    /// </summary>
    /// <param name="panel">表示パネル</param>
    public void OnDisplayPanel(GameObject panel)
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);
        panel.SetActive(true);
    }

    /// <summary>
    /// パネル非表示
    /// </summary>
    /// <param name="panel">閉じるパネル</param>
    public void OnClosePanel(GameObject panel)
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);
        panel.SetActive(false);
    }

    /// <summary>
    /// ソロボタン押下時
    /// </summary>
    public void OnSoloButton()
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // ソロ選択モード遷移
        SceneManager.LoadScene("03_SoloSelectScene");
    }

    /// <summary>
    /// オンラインボタン押下時
    /// </summary>
    public void OnOnlineButton()
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // オンラインモード遷移
        SceneManager.LoadScene("05_MatchingScene");
    }
}
