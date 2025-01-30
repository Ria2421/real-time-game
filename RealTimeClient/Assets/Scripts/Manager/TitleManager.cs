//---------------------------------------------------------------
// 繧ｿ繧､繝医Ν繝槭ロ繝ｼ繧ｸ繝｣繝ｼ [ TitleManager.cs ]
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
    // 繝輔ぅ繝ｼ繝ｫ繝�

    /// <summary>
    /// 繧ｿ繧､繝医Ν逕ｻ蜒�
    /// </summary>
    [SerializeField] private GameObject titleImage;

    /// <summary>
    /// 繧ｿ繝��メ逕ｻ蜒�
    /// </summary>
    [SerializeField] private GameObject touchImage;

    /// <summary>
    /// 繝ｦ繝ｼ繧ｶ繝ｼ逋ｻ骭ｲ繝代ロ繝ｫ
    /// </summary>
    [SerializeField] private GameObject registPanel;

    /// <summary>
    /// 逋ｻ骭ｲ繝ｦ繝ｼ繧ｶ繝ｼ蜷�
    /// </summary>
    [SerializeField] private Text nameText;

    /// <summary>
    /// 逋ｻ骭ｲ繝懊ち繝ｳ
    /// </summary>
    [SerializeField] private Button registButton;

    /// <summary>
    /// 繧ｨ繝ｩ繝ｼ繝懊ち繝ｳ
    /// </summary>
    [SerializeField] private GameObject errorButton;

    /// <summary>
    /// NG繝ｯ繝ｼ繝峨��繧ｿ繝ｳ
    /// </summary>
    [SerializeField] private GameObject ngWordButton;

    // 繝��ヰ繝��げ逕ｨ *******************************

    /// <summary>
    /// 繝��ヰ繝��げ逕ｨID
    /// </summary>
    [SerializeField] private Text debugIDText;

    /// <summary>
    /// 繝��ヰ繝��げ逕ｨ繝懊ち繝ｳ
    /// </summary>
    [SerializeField] private Button debugButton;

    //=====================================
    // 繝｡繧ｽ繝��ラ

    /// <summary>
    /// 蛻晄悄蜃ｦ逅�
    /// </summary>
    void Start()
    {
        Application.targetFrameRate = 60;

        // BGM蜀咲函
        BGMManager.Instance.Play(BGMPath.MAIN_BGM,0.75f,0,1,true,true);

        // 繧ｿ繧､繝医Ν逕ｻ蜒上い繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ
        titleImage.transform.DOScale(0.9f, 1.3f).SetEase(Ease.InCubic).SetLoops(-1,LoopType.Yoyo);
        InvokeRepeating("BlinkingImage", 0, 0.8f);
    }

    /// <summary>
    /// 繧ｹ繧ｿ繝ｼ繝医��繧ｿ繝ｳ謚ｼ荳区凾
    /// </summary>
    public void OnStartButton()
    {
        // SE蜀咲函
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // 繝ｦ繝ｼ繧ｶ繝ｼ繝�ー繧ｿ縺ｮ隱ｭ霎ｼ蜃ｦ逅�・邨先棡繧貞叙蠕�
        bool isSuccess = UserModel.Instance.LoadUserData();

        if (!isSuccess)
        {
            // 逋ｻ骭ｲ逕ｨ繝代ロ繝ｫ陦ｨ遉ｺ
            Debug.Log("繝�ー繧ｿ縺ｪ縺�");
            registPanel.SetActive(true);
        }
        else
        {   // 繧ｷ繝ｼ繝ｳ驕ｷ遘ｻ蜃ｦ逅�
            Debug.Log("繝�ー繧ｿ縺ゅｊ");
            Initiate.DoneFading();
            Initiate.Fade("2_MenuScene", Color.white, 2.5f);
        }
    }

    /// <summary>
    /// 逋ｻ骭ｲ繝懊ち繝ｳ謚ｼ荳区凾
    /// </summary>
    public async void OnRegistUser()
    {
        if (nameText.text == "") return;

        // SE蜀咲函
        SEManager.Instance.Play(SEPath.MENU_SELECT);

        // 繝懊ち繝ｳ辟｡蜉ｹ
        registButton.interactable = false;

        // 逋ｻ骭ｲ蜃ｦ逅�
        UserModel.Status statusCode = await UserModel.Instance.RegistUserAsync(nameText.text);

        switch (statusCode)
        {
            case UserModel.Status.True:
                Debug.Log("逋ｻ骭ｲ謌仙粥");
                Initiate.DoneFading();
                Initiate.Fade("2_MenuScene", Color.white, 2.5f);
                break;

            case UserModel.Status.False:
                Debug.Log("騾壻ｿ｡螟ｱ謨�");
                registButton.interactable = true;
                break;

            case UserModel.Status.SameName:
                Debug.Log("蜷榊燕陲ｫ繧�");
                errorButton.SetActive(true);
                break;

            case UserModel.Status.NGWord:
                Debug.Log("NG繝ｯ繝ｼ繝�");
                ngWordButton.SetActive(true);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// 逕ｻ蜒冗せ貊�処逅�
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

    // 繝��ヰ繝��げ逕ｨ ********************

    /// <summary>
    /// ID菫晏ｭ伜��逅�
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
    /// 繧ｨ繝ｩ繝ｼ繝懊ち繝ｳ謚ｼ荳区凾
    /// </summary>
    public void OnErrorButton(GameObject button)
    {
        // 陦ｨ遉ｺ豸亥悉
        button.SetActive(false);

        // 逋ｻ骭ｲ繝懊ち繝ｳ縺ｮ譛牙柑蛹�
        registButton.interactable = true;
    }
}