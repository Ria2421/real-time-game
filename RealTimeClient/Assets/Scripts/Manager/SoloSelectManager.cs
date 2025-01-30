//---------------------------------------------------------------
// 繧ｽ繝ｭ繧ｹ繝�ー繧ｸ驕ｸ謚槭��繝阪��繧ｸ繝｣繝ｼ [ SoloSelectManager.cs ]
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

public class SoloSelectManager : MonoBehaviour
{
    //=====================================
    // 繝輔ぅ繝ｼ繝ｫ繝�

    /// <summary>
    /// 蜿門ｾ励ざ繝ｼ繧ｹ繝医ョ繝ｼ繧ｿ
    /// </summary>
    private string getGhostData;

    /// <summary>
    /// 繝励Ξ繧､繧ｹ繝�ー繧ｸID
    /// </summary>
    private int playStageID = 1;

    /// <summary>
    /// 蜷��せ繝�ー繧ｸ縺ｮ繝ｩ繝ｳ繧ｭ繝ｳ繧ｰ諠��ｱ繧貞叙蠕�
    /// </summary>
    private List<List<RankingData>> stageRnakings = new List<List<RankingData>>();

    /// <summary>
    /// 譛�螟ｧ繧ｹ繝�ー繧ｸ謨ｰ
    /// </summary>
    [SerializeField] private int maxStage;

    /// <summary>
    /// 繝ｦ繝ｼ繧ｶ繝ｼ蜷肴�ｼ邏咲畑
    /// </summary>
    [SerializeField] private Text[] nameTexts;

    /// <summary>
    /// 繧ｯ繝ｪ繧｢繧ｿ繧､繝�譬ｼ邏咲畑
    /// </summary>
    [SerializeField] private Text[] clearTimeTexts;

    /// <summary>
    /// 繧ｹ繝�ー繧ｸ逕ｻ蜒乗�ｼ邏咲畑
    /// </summary>
    [SerializeField] private Sprite[] stageSprits;

    /// <summary>
    /// 繧ｹ繝�ー繧ｸ逕ｻ蜒�
    /// </summary>
    [SerializeField] private Image stageImage;

    /// <summary>
    /// 繝ｩ繝ｳ繧ｭ繝ｳ繧ｰ繝｢繝��Ν譬ｼ邏咲畑
    /// </summary>
    [SerializeField] private RankingModel rankingModel;

    /// <summary>
    /// 繧ｴ繝ｼ繧ｹ繝医が繝ｳ繝懊ち繝ｳ
    /// </summary>
    [SerializeField] private GameObject onGhostButton;

    /// <summary>
    /// 繧ｴ繝ｼ繧ｹ繝医が繝輔��繧ｿ繝ｳ
    /// </summary>
    [SerializeField] private GameObject offGhostButton;

    /// <summary>
    /// 繝阪け繧ｹ繝医せ繝�ー繧ｸ繝懊ち繝ｳ
    /// </summary>
    [SerializeField] private GameObject nextButton;

    /// <summary>
    /// 繝舌ャ繧ｯ繧ｹ繝�ー繧ｸ繝懊ち繝ｳ
    /// </summary>
    [SerializeField] private GameObject backButton;

    //=====================================
    // 繝｡繧ｽ繝��ラ

    /// <summary>
    /// 蛻晄悄蜃ｦ逅�
    /// </summary>
    async void Start()
    {
        //蜀咲函荳ｭ縺ｮBGM縺ｮ蜷榊燕繧貞��縺ｦ蜿門ｾ�
        var currentBGMNames = BGMManager.Instance.GetCurrentAudioNames();

        if (currentBGMNames[0] != "MainBGM")
        {   // MainBGM繧貞��髢�
            BGMManager.Instance.Stop(BGMPath.TIME_ATTACK);
            BGMManager.Instance.Stop(BGMPath.MULTI_PLAY);
            BGMManager.Instance.Play(BGMPath.MAIN_BGM, 0.75f, 0, 1, true, true);
        }

        for (int i=0;i < maxStage; i++)
        {   // 繧ｹ繝�ー繧ｸ謨ｰ蛻�の繝ｩ繝ｳ繧ｭ繝ｳ繧ｰ諠��ｱ繝ｪ繧ｹ繝医ｒ逕滓��
            stageRnakings.Add (new List<RankingData>());
        }

        // 繝ｩ繝ｳ繧ｭ繝ｳ繧ｰ繝�ー繧ｿ縺ｮ蜿門ｾ� (迴ｾ蝨ｨ縺ｯ繧ｹ繝�ー繧ｸ1縺ｫ蝗ｺ螳�)
        stageRnakings[0] = await rankingModel.GetRankingAsync(1);

        // 繝ｩ繝ｳ繧ｭ繝ｳ繧ｰ1菴阪��繧ｴ繝ｼ繧ｹ繝医ョ繝ｼ繧ｿ繧貞叙蠕�
        UserModel.Instance.GhostData = "";  // 繝ｪ繧ｻ繝��ヨ
        UserModel.Instance.GhostData = stageRnakings[0][0].GhostData;
        getGhostData = stageRnakings[0][0].GhostData;

        // 逕ｻ髱｢縺ｫ蜿肴丐
        for (int i = 0; i < stageRnakings[0].Count; i++)
        {
            nameTexts[i].text = stageRnakings[0][i].UserName;   // 蜷榊燕繧呈�ｼ邏�

            // 繧ｯ繝ｪ繧｢繧ｿ繧､繝�繧偵ユ繧ｭ繧ｹ繝医↓蜿肴丐
            float clearTIme = (float)stageRnakings[0][i].ClearTime / 1000.0f;
            string decNum = (clearTIme - (int)clearTIme).ToString(".000");
            clearTimeTexts[i].text = ((int)(clearTIme / 60)).ToString("00") + ":" + ((int)clearTIme % 60).ToString("00") + decNum;
        }
    }

    /// <summary>
    /// 繧ｹ繝�ー繧ｸ蛻��ｊ譖ｿ縺亥��逅�
    /// </summary>
    private async void SelectStageButton()
    {
        //--- 繝懊ち繝ｳ縺ｮ譛牙柑蛻��崛
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

        //--- 驕ｸ謚槭せ繝�ー繧ｸ縺ｮ繝ｩ繝ｳ繧ｭ繝ｳ繧ｰ諠��ｱ繧定｡ｨ遉ｺ

        // 譌｢縺ｫ繝ｩ繝ｳ繧ｭ繝ｳ繧ｰ諠��ｱ繧貞叙蠕励＠縺ｦ縺��ｋ縺句愛譁ｭ
        if (stageRnakings[playStageID - 1].Count == 0)
        {
            // 驕ｸ謚槭＠縺溘せ繝�ー繧ｸ繝ｩ繝ｳ繧ｭ繝ｳ繧ｰ繝�ー繧ｿ縺ｮ蜿門ｾ�
            stageRnakings[playStageID - 1] = await rankingModel.GetRankingAsync(playStageID);
        }

        // 繧ｴ繝ｼ繧ｹ繝医ョ繝ｼ繧ｿ縺ｮ蛻��ｊ譖ｿ縺�
        UserModel.Instance.GhostData = "";  // 繝ｪ繧ｻ繝��ヨ
        UserModel.Instance.GhostData = stageRnakings[playStageID - 1][0].GhostData;
        getGhostData = stageRnakings[playStageID - 1][0].GhostData;

        // 逕ｻ髱｢縺ｫ蜿肴丐
        for (int i = 0; i < stageRnakings[playStageID - 1].Count; i++)
        {
            nameTexts[i].text = stageRnakings[playStageID - 1][i].UserName;   // 蜷榊燕繧呈�ｼ邏�

            // 繧ｯ繝ｪ繧｢繧ｿ繧､繝�繧偵ユ繧ｭ繧ｹ繝医↓蜿肴丐
            float clearTIme = (float)stageRnakings[playStageID - 1][i].ClearTime / 1000.0f;
            string decNum = (clearTIme - (int)clearTIme).ToString(".000");
            clearTimeTexts[i].text = ((int)(clearTIme / 60)).ToString("00") + ":" + ((int)clearTIme % 60).ToString("00") + decNum;
        }

        // 逕ｻ蜒丞��繧頑崛縺�
        stageImage.sprite = stageSprits[playStageID - 1];
    }

    /// <summary>
    /// 繧ｹ繝�ー繧ｸ驕ｸ謚槭��繧ｿ繝ｳ (谺｡縺ｸ)
    /// </summary>
    public void OnNextButton()
    {
        playStageID++;
        if (playStageID >= maxStage) playStageID = maxStage;

        SelectStageButton();
    }

    /// <summary>
    /// 繧ｹ繝�ー繧ｸ驕ｸ謚槭��繧ｿ繝ｳ (蜑阪∈)
    /// </summary>
    public void OnBackButton()
    {
        playStageID--;
        if(playStageID <= 1) playStageID = 1;

        SelectStageButton();
    }

    /// <summary>
    /// 繝励Ξ繧､繝懊ち繝ｳ謚ｼ荳句��逅�
    /// </summary>
    public void OnPlayButton()
    {
        // SE蜀咲函
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // 繧ｽ繝ｭ繝励Ξ繧､繝｢繝ｼ繝蛾��遘ｻ
        Initiate.DoneFading();
        Initiate.Fade(playStageID.ToString() + "_SoloPlayScene", Color.white, 2.5f);
    }

    /// <summary>
    /// 繝｡繝九Η繝ｼ繝懊ち繝ｳ謚ｼ荳句��逅�
    /// </summary>
    public void OnMenuButton()
    {
        // SE蜀咲函
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // 繝｡繝九Η繝ｼ繝｢繝ｼ繝蛾��遘ｻ
        Initiate.DoneFading();
        Initiate.Fade("2_MenuScene", Color.white, 2.5f);
    }

    /// <summary>
    /// 繧ｴ繝ｼ繧ｹ繝医が繝ｳ繝懊ち繝ｳ
    /// </summary>
    public void OnGhostPlayButton()
    {
        UserModel.Instance.GhostData = getGhostData;

        onGhostButton.SetActive(false);
        offGhostButton.SetActive(true);
    }

    /// <summary>
    /// 繧ｴ繝ｼ繧ｹ繝医が繝輔��繧ｿ繝ｳ
    /// </summary>
    public void OnGhostNotButton()
    {
        UserModel.Instance.GhostData = "";

        offGhostButton.SetActive(false);
        onGhostButton.SetActive(true);
    }
}
