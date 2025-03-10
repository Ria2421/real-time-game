//---------------------------------------------------------------
// メニューマネージャー [ MenuManager.cs ]
// Author:Kenta Nakamoto
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

    private int imageNo = 0;

    [Header("---- Button ----")]

    // メニューボタン
    [SerializeField] private Button acountButton;           // アカウント
    [SerializeField] private Button shopButton;             // ショップ
    [SerializeField] private Button optionButton;           // オプション
    [SerializeField] private Button updateButton;           // 更新編集

    [Header("---- Slider ----")]

    // サウンドスライダー
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    [Header("---- Panel ----")]

    // メニューパネル
    [SerializeField] private GameObject accountPanel;
    [SerializeField] private GameObject soundPanel;
    [SerializeField] private GameObject helpPanel;

    [Header("---- AccountPanel ----")]

    // アカウントパネル表示UI
    [SerializeField] private Text displayNameText;
    [SerializeField] private Text inputNameText;
    [SerializeField] private Text registText;
    [SerializeField] private Text rateText;
    [SerializeField] private GameObject errorButton;        // エラー (名前被り)
    [SerializeField] private GameObject netErrorButton;     // エラー (通信エラー)
    [SerializeField] private GameObject ngWordButton;       // エラー (NGワード)
    [SerializeField] private GameObject nameUpdateButton;   // 名前更新完了

    [Header("---- HelpPanel ----")]

    // ヘルプパネル表示UI
    [SerializeField] private Text nowPageText;
    [SerializeField] private Text maxPageText;
    [SerializeField] private Image helpImage;
    [SerializeField] private Sprite[] helpSprites;

    //=====================================
    // メソッド

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        if (GameObject.Find("RoomModel"))
        {
            Destroy(GameObject.Find("RoomModel"));
        }

        //再生中のBGMの名前を全て取得
        var currentBGMNames = BGMManager.Instance.GetCurrentAudioNames();

        maxPageText.text = helpSprites.Length.ToString();

        // チュートリアル表示判断
        if (!UserModel.Instance.TutorialFlag)
        {
            helpPanel.SetActive(true);
            UserModel.Instance.TutorialFlag = true;
            UserModel.Instance.SaveUserData();
        }

        if (currentBGMNames[0] != "MainBGM")
        {   // MainBGMを再開
            BGMManager.Instance.Stop(BGMPath.TIME_ATTACK);
            BGMManager.Instance.Stop(BGMPath.MULTI_PLAY);
            BGMManager.Instance.Play(BGMPath.MAIN_BGM, 0.75f, 0, 1, true, true);
        }
    }

    /// <summary>
    /// エラーボタン非表示・更新ボタン復活
    /// </summary>
    public void OnErrorButton(GameObject button)
    {
        button.SetActive(false);

        updateButton.interactable = true;
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

    /// <summary>
    /// 指定パネルの表示処理
    /// </summary>
    private void DisplayPanel(GameObject panel)
    {
        // 全パネルを非表示
        accountPanel.SetActive(false);
        soundPanel.SetActive(false);
        helpPanel.SetActive(false);

        // 指定パネルを表示
        panel.SetActive(true);
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
        Initiate.DoneFading();
        Initiate.Fade("3_SoloSelectScene", Color.white, 2.5f);
    }

    /// <summary>
    /// オンラインボタン押下時
    /// </summary>
    public void OnOnlineButton()
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // オンラインモード遷移
        Initiate.DoneFading();
        Initiate.Fade("4_MatchingScene", Color.white, 2.5f);
    }

    /// <summary>
    /// タイトルボタン押下時
    /// </summary>
    public void OnTitleButton()
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // タイトル遷移
        Initiate.DoneFading();
        Initiate.Fade("1_TitleScene", Color.white, 2.5f);
    }

    /// <summary>
    /// アカウントボタン押下時
    /// </summary>
    public async void OnAcountButton()
    {
        if (accountPanel.activeSelf)
        {
            accountPanel.SetActive(false);
        }
        else
        {
            // SE再生
            SEManager.Instance.Play(SEPath.TAP_BUTTON);

            // ユーザーデータの取得
            var userData = await UserModel.Instance.SearchUserID(UserModel.Instance.UserId);

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
                DisplayPanel(accountPanel);
            }
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
                Debug.Log("登録成功");
                nameUpdateButton.SetActive(true);
                updateButton.interactable = true;
                break;

            case UserModel.Status.False:
                // ネットエラーボタン表示
                Debug.Log("通信失敗");
                netErrorButton.SetActive(true);
                break;

            case UserModel.Status.SameName:
                // エラー表示
                Debug.Log("名前被り");
                errorButton.SetActive(true);
                break;

            case UserModel.Status.NGWord:
                Debug.Log("NGワード");
                ngWordButton.SetActive(true);
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
        // 現在表示されているか
        if (soundPanel.activeSelf)
        {   // 表示している時
            soundPanel.SetActive(false);
        }
        else
        {
            // SE再生
            SEManager.Instance.Play(SEPath.TAP_BUTTON);
            // パネル表示
            DisplayPanel(soundPanel);
        }
    }

    /// <summary>
    /// ヘルプボタン押下時
    /// </summary>
    public void OnHelpButton()
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // パネル表示
        DisplayPanel(helpPanel);
    }

    /// <summary>
    /// ヘルプネクストボタン押下時
    /// </summary>
    public void OnHelpNextButton()
    {
        imageNo++;

        // 数値の上限設定
        if(imageNo >= helpSprites.Length - 1) imageNo = helpSprites.Length - 1;

        // 画像・ページNo更新
        nowPageText.text = (imageNo + 1).ToString();
        helpImage.sprite = helpSprites[imageNo];
    }

    /// <summary>
    /// ヘルプバックボタン押下時
    /// </summary>
    public void OnHelpBackButton()
    {
        imageNo--;

        // 数値の下限設定
        if (imageNo <= 0) imageNo = 0;

        // 画像・ページNo更新
        nowPageText.text = (imageNo + 1).ToString();
        helpImage.sprite = helpSprites[imageNo];
    }

    /// <summary>
    /// パネル非表示処理
    /// </summary>
    public void OnCloseDisplay(GameObject gameObject)
    {
        // SE再生
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        gameObject.SetActive(false);
    }
}
