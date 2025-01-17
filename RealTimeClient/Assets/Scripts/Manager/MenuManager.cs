//---------------------------------------------------------------
// メニューマネージャー [ MenuManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/10
// Update:2024/12/10
//---------------------------------------------------------------
using KanKikuchi.AudioManager;
using RealTimeServer.Model.Entity;
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
    [SerializeField] private Button acountButton;       // アカウント
    [SerializeField] private Button shopButton;         // ショップ
    [SerializeField] private Button optionButton;       // オプション
    [SerializeField] private Button updateButton;       // 更新編集
    [SerializeField] private GameObject errorButton;    // エラー (名前被り)
    [SerializeField] private GameObject netErrorButton; // エラー (通信エラー)

    // サウンドスライダー
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    // メニューパネル
    [SerializeField] private GameObject acountPanel;
    [SerializeField] private GameObject soundPanel;

    // アカウントパネル表示UI
    [SerializeField] private Text displayNameText;
    [SerializeField] private Text inputNameText;
    [SerializeField] private Text registText;
    [SerializeField] private Text rateText;

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

    /// <summary>
    /// タイトルボタン押下時
    /// </summary>
    public void OnTitleButton()
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // オンラインモード遷移
        SceneManager.LoadScene("01_TitleScene");
    }

    /// <summary>
    /// アカウントボタン押下時
    /// </summary>
    public async void OnAcountButton()
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // ユーザーデータの取得
        var userData =  await UserModel.Instance.SearchUserID(UserModel.Instance.UserId);

        if (userData == null)
        {   // エラー表示
            errorButton.SetActive(true);
            return;
        }
        else
        {   // ユーザーデータ反映・表示
            displayNameText.text = userData.Name;
            registText.text = userData.Created_at.ToString();
            rateText.text = userData.Rate.ToString();
            acountPanel.SetActive(true);
        }
    }

    /// <summary>
    /// ユーザー名変更ボタン
    /// </summary>
    public async void OnNameUpdateButton()
    {
        // ボタン無効化
        updateButton.interactable = false;

        // 登録処理
        UserModel.Status statusCode = await UserModel.Instance.UpdateUserName(UserModel.Instance.UserId,inputNameText.text);

        switch (statusCode)
        {
            case UserModel.Status.True:
                //++ 変更完了ウィンドウを表示
                Debug.Log("登録成功");
                break;

            case UserModel.Status.False:
                // ネットエラーボタン表示
                Debug.Log("通信失敗");
                netErrorButton.SetActive(false);
                break;

            case UserModel.Status.SameName:
                // エラー表示
                Debug.Log("名前被り");
                errorButton.SetActive(true);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// サウンドサウンドボタン押下時
    /// </summary>
    public void OnSoundButton()
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        soundPanel.SetActive(true);
    }

    /// <summary>
    /// パネル非表示処理
    /// </summary>
    public void OnCloseDisplay(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// エラーボタン非表示・更新ボタン復活
    /// </summary>
    public void OnErrorButton()
    {
        errorButton.SetActive(false);
        netErrorButton.SetActive(false);
        updateButton.interactable=true;
    }

    /// <summary>
    /// BGM音量変更処理
    /// </summary>
    public void ChangeBgmVolume()
    {
        BGMManager.Instance.ChangeBaseVolume(bgmSlider.value);
    }

    /// <summary>
    /// SE音量変更処理
    /// </summary>
    public void ChangeSeVolume()
    {
        SEManager.Instance.ChangeBaseVolume(seSlider.value);
    }
}
