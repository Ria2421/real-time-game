//---------------------------------------------------------------
// メニューマネージャー [ MenuManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/10
// Update:2024/12/10
//---------------------------------------------------------------
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
    [SerializeField] private Button profileButton;  // プロフィール
    [SerializeField] private Button shopButton;     // ショップ
    [SerializeField] private Button optionButton;   // オプション

    //=====================================
    // メソッド

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
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
        panel.SetActive(true);
    }

    /// <summary>
    /// パネル非表示
    /// </summary>
    /// <param name="panel">閉じるパネル</param>
    public void OnClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    /// <summary>
    /// ソロボタン押下時
    /// </summary>
    public void OnSoloButton()
    {
        // ソロモード遷移
        SceneManager.LoadScene("03_SoloScene");
    }

    /// <summary>
    /// オンラインボタン押下時
    /// </summary>
    public void OnOnlineButton()
    {
        // オンラインモード遷移
        SceneManager.LoadScene("04_MatchingScene");
    }
}
