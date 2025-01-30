//---------------------------------------------------------------
// タイトルマネージャー [ TitleManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/05
// Update:2025/01/30
//---------------------------------------------------------------
using DG.Tweening;
using KanKikuchi.AudioManager;
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

    /// <summary>
    /// エラーボタン
    /// </summary>
    [SerializeField] private GameObject errorButton;

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

        // BGM再生
        BGMManager.Instance.Play(BGMPath.MAIN_BGM,0.75f,0,1,true,true);

        // タイトル画像アニメーション
        titleImage.transform.DOScale(0.9f, 1.3f).SetEase(Ease.InCubic).SetLoops(-1,LoopType.Yoyo);
        InvokeRepeating("BlinkingImage", 0, 0.8f);
    }

    /// <summary>
    /// スタートボタン押下時
    /// </summary>
    public void OnStartButton()
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // ユーザーデータの読込処理・結果を取得
        bool isSuccess = UserModel.Instance.LoadUserData();

        if (!isSuccess)
        {
            // 登録用パネル表示
            Debug.Log("データなし");
            registPanel.SetActive(true);
        }
        else
        {   // シーン遷移処理
            Debug.Log("データあり");
            Initiate.DoneFading();
            Initiate.Fade("2_MenuScene", Color.white, 2.5f);
        }
    }

    /// <summary>
    /// 登録ボタン押下時
    /// </summary>
    public async void OnRegistUser()
    {
        if (nameText.text == "") return;

        // SE再生
        SEManager.Instance.Play(SEPath.MENU_SELECT);

        // ボタン無効
        registButton.interactable = false;

        // 登録処理
        UserModel.Status statusCode = await UserModel.Instance.RegistUserAsync(nameText.text);

        switch (statusCode)
        {
            case UserModel.Status.True:
                Debug.Log("登録成功");
                Initiate.DoneFading();
                Initiate.Fade("2_MenuScene", Color.white, 2.5f);
                break;

            case UserModel.Status.False:
                Debug.Log("通信失敗");
                registButton.interactable = true;
                break;

            case UserModel.Status.SameName:
                Debug.Log("名前被り");
                errorButton.SetActive(true);
                break;

            default:
                break;
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

        Initiate.DoneFading();
        Initiate.Fade("2_MenuScene", Color.white, 2.5f);
    }

    /// <summary>
    /// エラーボタン押下時
    /// </summary>
    public void OnErrorButton()
    {
        errorButton.SetActive(false);

        // 登録ボタンの有効化
        registButton.interactable = true;
    }
}