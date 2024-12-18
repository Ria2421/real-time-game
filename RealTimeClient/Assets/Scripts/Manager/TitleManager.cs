//---------------------------------------------------------------
// タイトルマネージャー [ TitleManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/05
// Update:2024/12/17
//---------------------------------------------------------------
using DG.Tweening;
using Shared.Interfaces.StreamingHubs;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class TitleManager : MonoBehaviour
{
    //=====================================
    // フィールド

    /// <summary>
    /// タイトル画像
    /// </summary>
    [SerializeField] private GameObject titleImage;

    /// <summary>
    /// タッチ画像
    /// </summary>
    [SerializeField] private GameObject touchImage;

    /// <summary>
    /// ユーザー登録パネル
    /// </summary>
    [SerializeField] private GameObject registPanel;

    /// <summary>
    /// 登録ユーザー名
    /// </summary>
    [SerializeField] private Text nameText;

    /// <summary>
    /// 登録ボタン
    /// </summary>
    [SerializeField] private Button registButton;

    // デバッグ用 *******************************

    /// <summary>
    /// デバッグ用ID
    /// </summary>
    [SerializeField] private Text debugIDText;

    /// <summary>
    /// デバッグ用ボタン
    /// </summary>
    [SerializeField] private Button debugButton;

    //=====================================
    // メソッド

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        Application.targetFrameRate = 60;

        titleImage.transform.DOScale(0.9f, 1.3f).SetEase(Ease.InCubic).SetLoops(-1,LoopType.Yoyo);

        InvokeRepeating("BlinkingImage", 0, 0.8f);
    }

    /// <summary>
    /// スタートボタン押下時
    /// </summary>
    public void OnStartButton()
    {
        // ユーザーデータの読込処理・結果を取得
        bool isSuccess = UserModel.Instance.LoadUserData();

        if (!isSuccess)
        {
            // 登録用パネル表示
            Debug.Log("データなし");
            registPanel.SetActive(true);
        }
        else
        {   // シーン遷移
            Debug.Log("データあり");
            SceneManager.LoadScene("02_MenuScene");
        }
    }

    /// <summary>
    /// 登録ボタン押下時
    /// </summary>
    public async void OnRegistUser()
    {
        if (nameText.text == "") return;

        // ボタン無効化
        registButton.interactable = false;

        // 登録処理
        bool isSucces = await UserModel.Instance.RegistUserAsync(nameText.text);

        if (isSucces)
        {
            Debug.Log("登録成功");
            SceneManager.LoadScene("02_MenuScene");
        }
        else
        {
            Debug.Log("登録失敗");
            registButton.interactable = true;
        }
    }

    /// <summary>
    /// 画像点滅処理
    /// </summary>
    private void BlinkingImage()
    {
        if(touchImage.activeSelf == true)
        {
            touchImage.SetActive(false);
        }
        else
        {
            touchImage.SetActive(true);
        }
    }

    // デバッグ用 ********************

    /// <summary>
    /// ID保存処理
    /// </summary>
    public void DebugOnSaveID()
    {
        if (debugIDText.text == "") return;

        debugButton.interactable = false;

        UserModel.Instance.UserId = int.Parse(debugIDText.text);

        SceneManager.LoadScene("02_MenuScene");
    }
}