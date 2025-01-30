//---------------------------------------------------------------
// 繧ｽ繝ｭ繝槭ロ繝ｼ繧ｸ繝｣繝ｼ [ SoloManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/10
// Update:2025/01/27
//---------------------------------------------------------------
using DavidJalbert;
using DG.Tweening;
using KanKikuchi.AudioManager;
using Newtonsoft.Json;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoloManager : MonoBehaviour
{
    //===================================
    // 繝輔ぅ繝ｼ繝ｫ繝�

    [Header("===== StageOption =====")]

    /// <summary>
    /// 繧ｹ繝�ー繧ｸNo
    /// </summary>
    [SerializeField] private int stageID;

    /// <summary>
    /// 譛�螟ｧ繝ｩ繝�プ謨ｰ
    /// </summary>
    [SerializeField] private int maxRapNum = 0;

    /// <summary>
    /// 繧ｴ繝ｼ繧ｹ繝医ョ繝ｼ繧ｿ險倬鹸髢馴囈
    /// </summary>
    [SerializeField] private float saveSpeed;

    /// <summary>
    /// 迴ｾ繝ｩ繝�プ謨ｰ
    /// </summary>
    private int currentRapNum = 1;

    /// <summary>
    /// 譎る俣險域ｸｬ逕ｨ
    /// </summary>
    private float timer;

    /// <summary>
    /// 繧ｴ繝ｼ繝ｫ繧ｿ繧､繝�菫晏ｭ�
    /// </summary>
    private int goalTime = 0;

    /// <summary>
    /// 險域ｸｬ繝輔Λ繧ｰ
    /// </summary>
    private bool isCount = false;

    /// <summary>
    /// 繧ｴ繝ｼ繧ｹ繝医ョ繝ｼ繧ｿ繝ｪ繧ｹ繝�
    /// </summary>
    private List<GhostData> ghostList = new List<GhostData>();

    /// <summary>
    /// 蜀咲函繧ｴ繝ｼ繧ｹ繝医ョ繝ｼ繧ｿ
    /// </summary>
    private List<GhostData> playGhost = new List<GhostData>();

    /// <summary>
    /// 繧ｴ繝ｼ繧ｹ繝医ョ繝ｼ繧ｿ繧ｫ繧ｦ繝ｳ繝�
    /// </summary>
    private int ghostCnt = 0;

    /// <summary>
    /// 繧ｴ繝ｼ繧ｹ繝郁｡ｨ遉ｺ陬懈ｭ｣蠎ｧ讓�
    /// </summary>
    private Vector3 ghostCorrection;

    [Space(25)]
    [Header("===== DataObject =====")]

    /// <summary>
    /// 蝓ｺ譛ｬ蜈･蜉帷ｮ｡逅��が繝悶ず繧ｧ
    /// </summary>
    [SerializeField] private GameObject standardInput;

    /// <summary>
    /// 繝｢繝舌う繝ｫ蜈･蜉帷ｮ｡逅��が繝悶ず繧ｧ
    /// </summary>
    [SerializeField] private GameObject mobileInput;

    /// <summary>
    /// 霆贋ｽ鍋��遐ｴ繝槭ロ繝ｼ繧ｸ繝｣繝ｼ
    /// </summary>
    [SerializeField] private TinyCarExplosiveBody boomManager;

    /// <summary>
    /// 繝ｩ繝ｳ繧ｭ繝ｳ繧ｰ繝｢繝��Ν譬ｼ邏咲畑
    /// </summary>
    [SerializeField] private RankingModel rankingModel;

    /// <summary>
    /// 菴咲ｽｮ諠��ｱ蜿門ｾ励が繝悶ず繧ｧ
    /// </summary>
    [SerializeField] private Transform visualObj;

    /// <summary>
    /// 繧ｿ繧､繝､隗貞叙蠕励が繝悶ず繧ｧ
    /// </summary>
    [SerializeField] private Transform wheelRot;

    /// <summary>
    /// 繧ｴ繝ｼ繧ｹ繝郁ｻ翫が繝悶ず繧ｧ
    /// </summary>
    [SerializeField] private GameObject ghostCarObj;

    /// <summary>
    /// 繧ｴ繝ｼ繧ｹ繝郁ｻ贋ｽ咲ｽｮ諠��ｱ
    /// </summary>
    [SerializeField] private Transform ghostCatTrs;

    /// <summary>
    /// 繧ｴ繝ｼ繧ｹ繝郁ｻ贋ｽ咲ｽｮ諠��ｱ
    /// </summary>
    [SerializeField] private Transform ghostWheelR;

    /// <summary>
    /// 繧ｴ繝ｼ繧ｹ繝郁ｻ贋ｽ咲ｽｮ諠��ｱ
    /// </summary>
    [SerializeField] private Transform ghostWheelL;

    [Space(25)]
    [Header("===== UI =====")]

    [Space(10)]
    [Header("---- Panel ----")]

    /// <summary>
    /// 繝ｪ繧ｶ繝ｫ繝医ヱ繝阪Ν
    /// </summary>
    [SerializeField] private GameObject resultPanel;

    /// <summary>
    /// 險域ｸｬ繧ｿ繧､繝槭��繝代ロ繝ｫ
    /// </summary>
    [SerializeField] private GameObject timerPanel;

    [Space(10)]
    [Header("---- Image ----")]

    /// <summary>
    /// 繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ繧ｪ繝悶ず繧ｧ
    /// </summary>
    [SerializeField] private GameObject countDownObj;

    /// <summary>
    /// 繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ逕ｨ繧ｹ繝励Λ繧､繝�
    /// </summary>
    [SerializeField] private Sprite[] countDownSprits;

    /// <summary>
    /// 譁ｰ險倬鹸逕ｻ蜒�
    /// </summary>
    [SerializeField] private GameObject newRecordObj;

    [Space(10)]
    [Header("---- Text ----")]

    /// <summary>
    /// 譎る俣險域ｸｬ逕ｨ繝��く繧ｹ繝�
    /// </summary>
    [SerializeField] private Text timerText;

    /// <summary>
    /// 繝ｩ繝�プ謨ｰ陦ｨ遉ｺ逕ｨ繝��く繧ｹ繝�
    /// </summary>
    [SerializeField] private Text rapText;

    //=====================================
    // 繝｡繧ｽ繝��ラ

    /// <summary>
    /// 蛻晄悄蜃ｦ逅�
    /// </summary>
    void Start()
    {
        // 繝｡繧､繝ｳBGM荳�譎ょ●豁｢
        BGMManager.Instance.Pause(BGMPath.MAIN_BGM);
        BGMManager.Instance.Play(BGMPath.TIME_ATTACK);

        // 螟画焚蛻晄悄蛹門��逅�
        isCount = false;
        ghostCnt = 0;
        ghostCorrection = new Vector3(0, -0.74f, 0);

        rapText.text = currentRapNum.ToString() + " / " + maxRapNum.ToString();

        // 蜀咲函縺吶ｋ繧ｴ繝ｼ繧ｹ繝医ョ繝ｼ繧ｿ繧貞叙蠕�
        if (UserModel.Instance.GhostData != "")
        {
            playGhost = JsonConvert.DeserializeObject<List<GhostData>>(UserModel.Instance.GhostData);
            Debug.Log("繧ｴ繝ｼ繧ｹ繝医ョ繝ｼ繧ｿ蜿門ｾ�");
        }
        else
        {
            ghostCarObj.SetActive(false);
        }

        // 繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ髢句ｧ�
        Debug.Log("繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ");
        StartCoroutine("StartCount");
    }

    /// <summary>
    /// 譖ｴ譁ｰ蜃ｦ逅�
    /// </summary>
    void Update()
    {
        if (!isCount) return;

        // timer繧貞茜逕ｨ縺励※邨碁℃譎る俣繧定ｨ域ｸｬ繝ｻ陦ｨ遉ｺ
        timer += Time.deltaTime;
        DisplayTime(timer);
    }

    /// <summary>
    /// 繝ｩ繝�プ謨ｰ蜉�邂怜��逅�
    /// </summary>
    public async void AddRapCnt()
    {
        currentRapNum++;    // 繝ｩ繝�プ蜉�邂�

        if(currentRapNum == maxRapNum + 1)
        {
            CancelInvoke("SaveGhost");

            // SE蜀咲函
            SEManager.Instance.Play(SEPath.GOAL);

            // 邨ゆｺ��愛螳�
            isCount = false;                // 繧ｿ繧､繝槭��繧ｹ繝医ャ繝�
            Invoke("BoomCar", 1);           // 霆贋ｽ鍋��遐ｴ
            resultPanel.SetActive(true);    // 繝ｪ繧ｶ繝ｫ繝郁｡ｨ遉ｺ
            mobileInput.SetActive(false);   // 繝｢繝舌う繝ｫUI髱櫁｡ｨ遉ｺ
            
            // 繧ｿ繧､繝槭��遘ｻ蜍募��逅�
            var sequence = DOTween.Sequence();
            sequence.Append(timerPanel.transform.DOScale(1.7f, 0.7f));
            sequence.Append(timerPanel.transform.DOLocalMove(new Vector3(0,-25,0), 0.5f));
            sequence.Play();

            // 繧ｯ繝ｪ繧｢繧ｿ繧､繝�繧知/sec縺ｫ螟画鋤縺吶ｋ
            timer = (float)Math.Round(timer, 3, MidpointRounding.AwayFromZero);
            goalTime = (int)(timer * 1000);

            UserModel userModel = UserModel.Instance;

            // 騾∽ｿ｡逕ｨ繧ｴ繝ｼ繧ｹ繝医ョ繝ｼ繧ｿ縺ｮ菴懈��
            SaveGhost();
            string ghostData = JsonConvert.SerializeObject(ghostList);

            // 險倬鹸逋ｻ骭ｲ蜃ｦ逅�
            RegistResult result = await rankingModel.RegistClearTimeAsync(stageID, userModel.UserId, goalTime, ghostData);

            if (result.timeRegistFlag)
            {   // newRecord陦ｨ遉ｺ
                SEManager.Instance.Play(SEPath.NEW_RECORD);
                newRecordObj.SetActive(true);
                newRecordObj.transform.DOScale(1.3f, 1.5f).SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);
            }

            Debug.Log("goal : " + goalTime.ToString() + "m/sec");
        }
        else
        {
            Debug.Log("迴ｾ繝ｩ繝�プ謨ｰ" + currentRapNum);
            rapText.text = currentRapNum.ToString() + " / " + maxRapNum.ToString();
        }
    }

    /// <summary>
    /// 蛻ｶ髯先凾髢薙ｒ譖ｴ譁ｰ縺励※[蛻�:遘綻縺ｧ陦ｨ遉ｺ縺吶ｋ
    /// </summary>
    private void DisplayTime(float limitTime)
    {
        // 蠑墓焚縺ｧ蜿励￠蜿悶▲縺溷�､繧端蛻�:遘綻縺ｫ螟画鋤縺励※陦ｨ遉ｺ縺吶ｋ
        // ToString("00")縺ｧ繧ｼ繝ｭ繝励Ξ繝ｼ繧ｹ繝輔か繝ｫ繝�繝ｼ縺励※縲��ｼ第｡√��縺ｨ縺阪��鬆ｭ縺ｫ0繧偵▽縺代ｋ
        string decNum = (limitTime - (int)limitTime).ToString(".000");
        timerText.text = ((int)(limitTime / 60)).ToString("00") + ":" + ((int)limitTime % 60).ToString("00") + decNum;
    }

    /// <summary>
    /// 繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ髱櫁｡ｨ遉ｺ蜃ｦ逅�
    /// </summary>
    /// <param name="obj">髱櫁｡ｨ遉ｺ縺ｫ縺励◆縺��が繝悶ず繧ｧ</param>
    private void HiddenCount()
    {
        countDownObj.SetActive(false);
    }

    /// <summary>
    /// 霆贋ｽ鍋��遐ｴ蜃ｦ逅�
    /// </summary>
    private void BoomCar()
    {
        boomManager.explode();
    }

    /// <summary>
    /// 繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ蜃ｦ逅�
    /// </summary>
    /// <returns></returns>
    IEnumerator StartCount()
    {
        for (int i = 0; i < 4; i++)
        {
            countDownObj.GetComponent<Image>().sprite = countDownSprits[i];

            if (i == 3)
            {
                SEManager.Instance.Play(SEPath.START);
                // 險域ｸｬ髢句ｧ九��謫堺ｽ懷庄閭ｽ繝ｻ繧ｴ繝ｼ繧ｹ繝医ョ繝ｼ繧ｿ菫晏ｭ伜��逅��ｵｷ蜍輔☆繧�
                countDownObj.GetComponent<Image>().sprite = countDownSprits[i];

                if (UserModel.Instance.GhostData != "")
                {   // 繧ｴ繝ｼ繧ｹ繝医ョ繝ｼ繧ｿ縺後≠繧区凾縺ｮ縺ｿ蜀咲函
                    InvokeRepeating("PlayGhost",0,saveSpeed);
                }

                standardInput.SetActive(true);
                mobileInput.SetActive(true);
                isCount = true;
                InvokeRepeating("SaveGhost", 0.1f, saveSpeed);

                // 繧ｫ繧ｦ繝ｳ繝磯撼陦ｨ遉ｺ
                Invoke("HiddenCount", 0.6f);
            }
            else
            {
                SEManager.Instance.Play(SEPath.COUNT);
                // 0.9遘貞ｾ��▲縺ｦ繧ｳ繝ｫ繝ｼ繝√Φ荳ｭ譁ｭ
                yield return new WaitForSeconds(0.9f);
            }
        }
    }

    /// <summary>
    /// 繧ｴ繝ｼ繧ｹ繝医ョ繝ｼ繧ｿ菫晏ｭ伜��逅�
    /// </summary>
    private void SaveGhost()
    {
        GhostData ghostData = new GhostData();
        ghostData.Pos = visualObj.position;        // 菴咲ｽｮ
        ghostData.Rot = visualObj.eulerAngles;     // 隗貞ｺｦ
        ghostData.WRot = wheelRot.localEulerAngles.y;   // 繧ｿ繧､繝､隗�

        ghostList.Add(ghostData);
    }

    /// <summary>
    /// 繧ｴ繝ｼ繧ｹ繝亥��逕溷��逅�
    /// </summary>
    private void PlayGhost()
    {
        // 譛ｬ菴謎ｽ咲ｽｮ縺ｮ譖ｴ譁ｰ
        ghostCarObj.transform.DOMove(playGhost[ghostCnt].Pos + ghostCorrection, saveSpeed).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed, true);
        ghostCarObj.transform.DORotate(playGhost[ghostCnt].Rot, saveSpeed).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed, true);

        // 繧ｿ繧､繝､隗偵��譖ｴ譁ｰ
        ghostWheelL.transform.localEulerAngles = new Vector3 (ghostWheelL.transform.localEulerAngles.x,playGhost[ghostCnt].WRot,0);
        ghostWheelR.transform.localEulerAngles = new Vector3(ghostWheelR.transform.localEulerAngles.x, playGhost[ghostCnt].WRot, 0);

        ghostCnt++;

        if (playGhost.Count - 1 < ghostCnt)
        {   // 蜀咲函縺吶ｋ繝�ー繧ｿ縺檎┌縺��凾縺ｯ蜀咲函蛛懈ｭ｢
            CancelInvoke("PlayGhost");
            return;
        }
    }

    /// <summary>
    /// 繝｡繝九Η繝ｼ驕ｷ遘ｻ蜃ｦ逅�
    /// </summary>
    public void OnBackMenu()
    {
        // SE蜀咲函
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        Initiate.DoneFading();
        Initiate.Fade("2_MenuScene", Color.white, 2.5f);
    }

    /// <summary>
    /// 繧ｹ繝�ー繧ｸ驕ｸ謚樣��遘ｻ蜃ｦ逅�
    /// </summary>
    public void OnBackSelect()
    {
        // SE蜀咲函
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        Initiate.DoneFading();
        Initiate.Fade("3_SoloSelectScene", Color.white, 2.5f);
    }

    /// <summary>
    /// 繝ｪ繝医Λ繧､驕ｷ遘ｻ蜃ｦ逅�
    /// </summary>
    public void OnRetryButton() 
    {
        // SE蜀咲函
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        Initiate.DoneFading();
        Initiate.Fade(SceneManager.GetActiveScene().name, Color.white, 1.0f);
    }

    /// <summary>
    /// 繝代ロ繝ｫ髢峨§繧句��逅�
    /// </summary>
    /// <param name="panel"></param>
    public void OnCloseButton(GameObject panel)
    {
        panel.SetActive(false);
    }

    /// <summary>
    /// 繝｡繝九Η繝ｼ繝懊ち繝ｳ謚ｼ荳区凾
    /// </summary>
    /// <param name="menuPanel"></param>
    public void OnMenuButton(GameObject menuPanel)
    {
        if (menuPanel.activeSelf)
        {
            menuPanel.SetActive(false);
        }
        else
        {
            menuPanel.SetActive(true);
        }
    }
}
