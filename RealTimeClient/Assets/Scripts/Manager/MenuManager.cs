//---------------------------------------------------------------
// 繝｡繝九Η繝ｼ繝槭ロ繝ｼ繧ｸ繝｣繝ｼ [ MenuManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/10
// Update:2025/01/30
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
    // 繝輔ぅ繝ｼ繝ｫ繝�

    private int imageNo = 0;

    [Header("---- Button ----")]

    // 繝｡繝九Η繝ｼ繝懊ち繝ｳ
    [SerializeField] private Button acountButton;           // 繧｢繧ｫ繧ｦ繝ｳ繝�
    [SerializeField] private Button shopButton;             // 繧ｷ繝ｧ繝�プ
    [SerializeField] private Button optionButton;           // 繧ｪ繝励す繝ｧ繝ｳ
    [SerializeField] private Button updateButton;           // 譖ｴ譁ｰ邱ｨ髮�

    [Header("---- Slider ----")]

    // 繧ｵ繧ｦ繝ｳ繝峨せ繝ｩ繧､繝�繝ｼ
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    [Header("---- Panel ----")]

    // 繝｡繝九Η繝ｼ繝代ロ繝ｫ
    [SerializeField] private GameObject accountPanel;
    [SerializeField] private GameObject soundPanel;
    [SerializeField] private GameObject helpPanel;

    [Header("---- AccountPanel ----")]

    // 繧｢繧ｫ繧ｦ繝ｳ繝医ヱ繝阪Ν陦ｨ遉ｺUI
    [SerializeField] private Text displayNameText;
    [SerializeField] private Text inputNameText;
    [SerializeField] private Text registText;
    [SerializeField] private Text rateText;
    [SerializeField] private GameObject errorButton;        // 繧ｨ繝ｩ繝ｼ (蜷榊燕陲ｫ繧�)
    [SerializeField] private GameObject netErrorButton;     // 繧ｨ繝ｩ繝ｼ (騾壻ｿ｡繧ｨ繝ｩ繝ｼ)
    [SerializeField] private GameObject nameUpdateButton;   // 蜷榊燕譖ｴ譁ｰ螳御ｺ�

    [Header("---- HelpPanel ----")]

    // 繝倥Ν繝励ヱ繝阪Ν陦ｨ遉ｺUI
    [SerializeField] private Text nowPageText;
    [SerializeField] private Text maxPageText;
    [SerializeField] private Image helpImage;
    [SerializeField] private Sprite[] helpSprites;

    //=====================================
    // 繝｡繧ｽ繝��ラ

    /// <summary>
    /// 蛻晄悄蜃ｦ逅�
    /// </summary>
    void Start()
    {
        if (GameObject.Find("RoomModel"))
        {
            Destroy(GameObject.Find("RoomModel"));
        }

        //蜀咲函荳ｭ縺ｮBGM縺ｮ蜷榊燕繧貞��縺ｦ蜿門ｾ�
        var currentBGMNames = BGMManager.Instance.GetCurrentAudioNames();

        maxPageText.text = helpSprites.Length.ToString();

        // 繝√Η繝ｼ繝医Μ繧｢繝ｫ陦ｨ遉ｺ蛻､譁ｭ
        if (!UserModel.Instance.TutorialFlag)
        {
            helpPanel.SetActive(true);
            UserModel.Instance.TutorialFlag = true;
            UserModel.Instance.SaveUserData();
        }

        if (currentBGMNames[0] != "MainBGM")
        {   // MainBGM繧貞��髢�
            BGMManager.Instance.Stop(BGMPath.TIME_ATTACK);
            BGMManager.Instance.Stop(BGMPath.MULTI_PLAY);
            BGMManager.Instance.Play(BGMPath.MAIN_BGM, 0.75f, 0, 1, true, true);
        }
    }

    /// <summary>
    /// 繧ｨ繝ｩ繝ｼ繝懊ち繝ｳ髱櫁｡ｨ遉ｺ繝ｻ譖ｴ譁ｰ繝懊ち繝ｳ蠕ｩ豢ｻ
    /// </summary>
    public void OnErrorButton()
    {
        errorButton.SetActive(false);
        netErrorButton.SetActive(false);
        updateButton.interactable = true;
    }

    /// <summary>
    /// BGM髻ｳ驥丞､画峩蜃ｦ逅�
    /// </summary>
    public void ChangeBgmVolume()
    {
        BGMManager.Instance.ChangeBaseVolume(bgmSlider.value);
    }

    /// <summary>
    /// SE髻ｳ驥丞､画峩蜃ｦ逅�
    /// </summary>
    public void ChangeSeVolume()
    {
        SEManager.Instance.ChangeBaseVolume(seSlider.value);
    }

    /// <summary>
    /// 謖��ｮ壹ヱ繝阪Ν縺ｮ陦ｨ遉ｺ蜃ｦ逅�
    /// </summary>
    private void DisplayPanel(GameObject panel)
    {
        // 蜈ｨ繝代ロ繝ｫ繧帝撼陦ｨ遉ｺ
        accountPanel.SetActive(false);
        soundPanel.SetActive(false);
        helpPanel.SetActive(false);

        // 謖��ｮ壹ヱ繝阪Ν繧定｡ｨ遉ｺ
        panel.SetActive(true);
    }

    //-----------------------------
    // 繝懊ち繝ｳ謚ｼ荳句��逅�

    /// <summary>
    /// 繧ｽ繝ｭ繝懊ち繝ｳ謚ｼ荳区凾
    /// </summary>
    public void OnSoloButton()
    {
        // SE蜀咲函
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // 繧ｽ繝ｭ驕ｸ謚槭Δ繝ｼ繝蛾��遘ｻ
        Initiate.DoneFading();
        Initiate.Fade("3_SoloSelectScene", Color.white, 2.5f);
    }

    /// <summary>
    /// 繧ｪ繝ｳ繝ｩ繧､繝ｳ繝懊ち繝ｳ謚ｼ荳区凾
    /// </summary>
    public void OnOnlineButton()
    {
        // SE蜀咲函
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // 繧ｪ繝ｳ繝ｩ繧､繝ｳ繝｢繝ｼ繝蛾��遘ｻ
        Initiate.DoneFading();
        Initiate.Fade("4_MatchingScene", Color.white, 2.5f);
    }

    /// <summary>
    /// 繧ｿ繧､繝医Ν繝懊ち繝ｳ謚ｼ荳区凾
    /// </summary>
    public void OnTitleButton()
    {
        // SE蜀咲函
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // 繧ｿ繧､繝医Ν驕ｷ遘ｻ
        Initiate.DoneFading();
        Initiate.Fade("1_TitleScene", Color.white, 2.5f);
    }

    /// <summary>
    /// 繧｢繧ｫ繧ｦ繝ｳ繝医��繧ｿ繝ｳ謚ｼ荳区凾
    /// </summary>
    public async void OnAcountButton()
    {
        if (accountPanel.activeSelf)
        {
            accountPanel.SetActive(false);
        }
        else
        {
            // SE蜀咲函
            SEManager.Instance.Play(SEPath.TAP_BUTTON);

            // 繝ｦ繝ｼ繧ｶ繝ｼ繝�ー繧ｿ縺ｮ蜿門ｾ�
            var userData = await UserModel.Instance.SearchUserID(UserModel.Instance.UserId);

            if (userData == null)
            {   // 繧ｨ繝ｩ繝ｼ陦ｨ遉ｺ
                errorButton.SetActive(true);
                return;
            }
            else
            {   // 繝ｦ繝ｼ繧ｶ繝ｼ繝�ー繧ｿ蜿肴丐繝ｻ陦ｨ遉ｺ
                displayNameText.text = userData.Name;
                registText.text = userData.Created_at.ToString();
                rateText.text = userData.Rate.ToString();
                DisplayPanel(accountPanel);
            }
        }
    }

    /// <summary>
    /// 繝ｦ繝ｼ繧ｶ繝ｼ蜷榊､画峩繝懊ち繝ｳ
    /// </summary>
    public async void OnNameUpdateButton()
    {
        // 繝懊ち繝ｳ辟｡蜉ｹ蛹�
        updateButton.interactable = false;

        // 逋ｻ骭ｲ蜃ｦ逅�
        UserModel.Status statusCode = await UserModel.Instance.UpdateUserName(UserModel.Instance.UserId,inputNameText.text);

        switch (statusCode)
        {
            case UserModel.Status.True:
                Debug.Log("逋ｻ骭ｲ謌仙粥");
                nameUpdateButton.SetActive(true);
                updateButton.interactable = true;
                break;

            case UserModel.Status.False:
                // 繝阪ャ繝医お繝ｩ繝ｼ繝懊ち繝ｳ陦ｨ遉ｺ
                Debug.Log("騾壻ｿ｡螟ｱ謨�");
                netErrorButton.SetActive(true);
                break;

            case UserModel.Status.SameName:
                // 繧ｨ繝ｩ繝ｼ陦ｨ遉ｺ
                Debug.Log("蜷榊燕陲ｫ繧�");
                errorButton.SetActive(true);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// 繧ｵ繧ｦ繝ｳ繝峨し繧ｦ繝ｳ繝峨��繧ｿ繝ｳ謚ｼ荳区凾
    /// </summary>
    public void OnSoundButton()
    {
        // 迴ｾ蝨ｨ陦ｨ遉ｺ縺輔ｌ縺ｦ縺��ｋ縺�
        if (soundPanel.activeSelf)
        {   // 陦ｨ遉ｺ縺励※縺��ｋ譎�
            soundPanel.SetActive(false);
        }
        else
        {
            // SE蜀咲函
            SEManager.Instance.Play(SEPath.TAP_BUTTON);
            // 繝代ロ繝ｫ陦ｨ遉ｺ
            DisplayPanel(soundPanel);
        }
    }

    /// <summary>
    /// 繝倥Ν繝励��繧ｿ繝ｳ謚ｼ荳区凾
    /// </summary>
    public void OnHelpButton()
    {
        // SE蜀咲函
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // 繝代ロ繝ｫ陦ｨ遉ｺ
        DisplayPanel(helpPanel);
    }

    /// <summary>
    /// 繝倥Ν繝励ロ繧ｯ繧ｹ繝医��繧ｿ繝ｳ謚ｼ荳区凾
    /// </summary>
    public void OnHelpNextButton()
    {
        imageNo++;

        // 謨ｰ蛟､縺ｮ荳企剞險ｭ螳�
        if(imageNo >= helpSprites.Length - 1) imageNo = helpSprites.Length - 1;

        // 逕ｻ蜒上��繝壹��繧ｸNo譖ｴ譁ｰ
        nowPageText.text = (imageNo + 1).ToString();
        helpImage.sprite = helpSprites[imageNo];
    }

    /// <summary>
    /// 繝倥Ν繝励ヰ繝��け繝懊ち繝ｳ謚ｼ荳区凾
    /// </summary>
    public void OnHelpBackButton()
    {
        imageNo--;

        // 謨ｰ蛟､縺ｮ荳矩剞險ｭ螳�
        if (imageNo <= 0) imageNo = 0;

        // 逕ｻ蜒上��繝壹��繧ｸNo譖ｴ譁ｰ
        nowPageText.text = (imageNo + 1).ToString();
        helpImage.sprite = helpSprites[imageNo];
    }

    /// <summary>
    /// 繝代ロ繝ｫ髱櫁｡ｨ遉ｺ蜃ｦ逅�
    /// </summary>
    public void OnCloseDisplay(GameObject gameObject)
    {
        // SE蜀咲函
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        gameObject.SetActive(false);
    }
}
