//---------------------------------------------------------------
// 繧ｲ繝ｼ繝�繝槭ロ繝ｼ繧ｸ繝｣繝ｼ [ GameManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/11/18
// Update:2025/01/28
//---------------------------------------------------------------
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using DG.Tweening;
using RealTimeServer.Model.Entity;
using DavidJalbert;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks.Triggers;
using KanKikuchi.AudioManager;

public class GameManager : MonoBehaviour
{
    //=====================================
    // 繝輔ぅ繝ｼ繝ｫ繝�

    /// <summary>
    /// 繧ｲ繝ｼ繝�迥ｶ諷狗ｨｮ鬘�
    /// </summary>
    private enum GameState
    {
        None = 0,
        Join,
        InGame,
        Result
    }

    /// <summary>
    /// 繝��せ繝槭ャ繝��幕蟋九ち繧､繝�
    /// </summary>
    private const int DEATH_MATCH_TIME = 10;

    /// <summary>
    /// 繝ｫ繝ｼ繝�繝｢繝��Ν譬ｼ邏咲畑
    /// </summary>
    private RoomModel roomModel;

    /// <summary>
    /// 謗･邯唔D繧偵く繝ｼ縺ｫ繧ｭ繝｣繝ｩ繧ｯ繧ｿ縺ｮ繧ｪ繝悶ず繧ｧ繧ｯ繝医ｒ邂｡逅�
    /// </summary>
    private Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    /// <summary>
    /// 繧ｲ繝ｼ繝�迥ｶ諷�
    /// </summary>
    private GameState gameState = GameState.None;

    /// <summary>
    /// 陬懈ｭ｣pos
    /// </summary>
    private Vector3 posCorrection = new Vector3(0.0f,-0.9f,0.0f);

    /// <summary>
    /// 繝励Ξ繧､繝､繝ｼ繧ｳ繝ｳ繝医Ο繝ｼ繝ｩ繝ｼ
    /// </summary>
    private GameObject playerController;

    /// <summary>
    /// 霆贋ｽ薙が繝悶ず繧ｧ縺ｮ菴咲ｽｮ諠��ｱ
    /// </summary>
    private Transform visualTransform;

    /// <summary>
    /// 謫堺ｽ懊さ繝ｳ繝医Ο繝ｼ繝ｩ繝ｼ
    /// </summary>
    private GameObject inputController;

    /// <summary>
    /// 繝帙う繝ｼ繝ｫ縺ｮ隗貞ｺｦ蜿門ｾ礼畑
    /// </summary>
    private Transform wheelAngle;

    /// <summary>
    /// 繝励Ξ繧､繝､繝ｼ縺ｮ繧ｿ繝ｼ繝懊ヱ繝ｼ繝��ぅ繧ｯ繝ｫ
    /// </summary>
    private ParticleSystem playerTurboParticle;

    /// <summary>
    /// 繝励Ξ繧､繝､繝ｼ縺ｮ繝峨Μ繝輔ヨ繝代��繝��ぅ繧ｯ繝ｫ
    /// </summary>
    private ParticleSystem playerDriftParticle;

    /// <summary>
    /// 蜿ょ刈鬆�
    /// </summary>
    private int joinOrder = 0;

    /// <summary>
    /// 螟ｧ遐ｲ逋ｺ蟆��ヵ繝ｩ繧ｰ
    /// </summary>
    private bool isCannon = false;

    /// <summary>
    /// 繝��せ繝槭ャ繝√ヵ繝ｩ繧ｰ
    /// </summary>
    private bool isDeathMatch = false;

    [Header("謨ｰ蛟､險ｭ螳�")]

    /// <summary>
    /// 騾壻ｿ｡騾溷ｺｦ
    /// </summary>
    [SerializeField] private float internetSpeed = 0.1f;

    /// <summary>
    /// 蛻ｶ髯先凾髢�
    /// </summary>
    [SerializeField] private int timeLimit = 30;

    /// <summary>
    /// 螟ｧ遐ｲ逋ｺ蟆��俣髫�
    /// </summary>
    [SerializeField] private int shotInterval = 10;

    [Header("蜷��ｨｮObject繧偵い繧ｿ繝��メ")]

    /// <summary>
    /// 繝｢繝舌う繝ｫ繧､繝ｳ繝励ャ繝医せ繧ｯ繝ｪ繝励ヨ
    /// </summary>
    [SerializeField] private TinyCarMobileInput tinyCarMobileInput;

    /// <summary>
    /// 繝｢繝舌う繝ｫ繧､繝ｳ繝励ャ繝�obj
    /// </summary>
    [SerializeField] private GameObject mobileInputObj;

    /// <summary>
    /// 逕滓��縺吶ｋ繝励Ξ繧､繝､繝ｼ縺ｮ繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼ繝励Ξ繝上ヶ
    /// </summary>
    [SerializeField] private GameObject playerPrefab;

    /// <summary>
    /// 逕滓��縺吶ｋ莉悶��繝ｬ繧､繝､繝ｼ縺ｮ繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼ繝励Ξ繝上ヶ
    /// </summary>
    [SerializeField] private GameObject otherPrefab;

    /// <summary>
    /// 蜈･蜉帛��逅�プ繝ｬ繝上ヶ
    /// </summary>
    [SerializeField] private GameObject inputPrefab;

    /// <summary>
    /// 繝励Ξ繧､繝､繝ｼ繧呈�ｼ邏阪☆繧玖ｦｪ繧ｪ繝悶ず繧ｧ繧ｯ繝�
    /// </summary>
    [SerializeField] private GameObject parentObj;

    /// <summary>
    /// 繝｡繧､繝ｳ繧ｫ繝｡繝ｩ
    /// </summary>
    [SerializeField] private GameObject mainCamera;

    /// <summary>
    /// 繝励Ξ繧､繝､繝ｼ縺斐→縺ｮ繝ｪ繧ｹ蝨ｰ轤ｹ
    /// </summary>
    [SerializeField] private Transform[] respownList;

    /// <summary>
    /// 辷��匱繝代��繝��ぅ繧ｯ繝ｫ
    /// </summary>
    [SerializeField] private GameObject explosionPrefab;

    /// <summary>
    /// 繝ｦ繝ｼ繧ｶ繝ｼ蜷崎｡ｨ遉ｺ逕ｨ繧ｪ繝悶ず繧ｧ
    /// </summary>
    [SerializeField] private GameObject[] nameObjs;

    /// <summary>
    /// 螟ｧ遐ｲ繧ｹ繧ｯ繝ｪ繝励ヨ
    /// </summary>
    [SerializeField] private Cannon[] cannons;

    [Space (25)]
    [Header("===== UI髢｢騾｣ =====")]

    [Space(10)]
    [Header("---- Text ----")]

    /// <summary>
    /// 繧ｿ繧､繝槭��
    /// </summary>
    [SerializeField] private Text timerText;

    /// <summary>
    /// 謦��ｴ騾夂衍陦ｨ遉ｺ
    /// </summary>
    [SerializeField] private GameObject crushText;

    /// <summary>
    /// 繝ｬ繝ｼ繝郁｡ｨ遉ｺ逕ｨ繧ｪ繝悶ず繧ｧ
    /// </summary>
    [SerializeField] private GameObject rateObjs;

    /// <summary>
    /// 繝ｬ繝ｼ繝医ユ繧ｭ繧ｹ繝�
    /// </summary>
    [SerializeField] private Text rateText;

    /// <summary>
    /// 隨ｦ蜿ｷ陦ｨ遉ｺ繝��く繧ｹ繝�
    /// </summary>
    [SerializeField] private Text signText;

    /// <summary>
    /// 繝ｬ繝ｼ繝亥｢玲ｸ帙ユ繧ｭ繧ｹ繝�
    /// </summary>
    [SerializeField] private Text changeRateText;

    /// <summary>
    /// 繝ｦ繝ｼ繧ｶ繝ｼ蜷阪ユ繧ｭ繧ｹ繝�
    /// </summary>
    [SerializeField] private Text[] nameTexts;

    [Space(10)]
    [Header("---- Panel ----")]

    /// <summary>
    /// 繝ｪ繧ｶ繝ｫ繝医ヱ繝阪Ν
    /// </summary>
    [SerializeField] private GameObject resultPanel;

    /// <summary>
    /// 蛻��妙陦ｨ遉ｺ繝代ロ繝ｫ
    /// </summary>
    [SerializeField] private GameObject disconnectPanel;

    /// <summary>
    /// 谿九ち繧､繝�陦ｨ遉ｺ繝代ロ繝ｫ
    /// </summary>
    [SerializeField] private GameObject timerPanel;

    [Space(10)]
    [Header("---- Image ----")]

    /// <summary>
    /// 鬆��ｽ崎｡ｨ遉ｺ逕ｻ蜒�
    /// </summary>
    [SerializeField] private GameObject[] rankImages;

    /// <summary>
    /// 繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ逕ｻ蜒上が繝悶ず繧ｧ
    /// </summary>
    [SerializeField] private GameObject countDownImageObj;

    /// <summary>
    /// 邨ゆｺ��｡ｨ遉ｺ逕ｻ蜒�
    /// </summary>
    [SerializeField] private GameObject endImageObj;

    /// <summary>
    /// 繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ逕ｻ蜒�
    /// </summary>
    [SerializeField] private Image countDownImage;

    /// <summary>
    /// 蠑輔″蛻��￠陦ｨ遉ｺ逕ｨ繧ｪ繝悶ず繧ｧ
    /// </summary>
    [SerializeField] private GameObject drawImageObj;

    [Space(10)]
    [Header("---- Sprit ----")]

    /// <summary>
    /// 繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ繧ｹ繝励Λ繧､繝�
    /// </summary>
    [SerializeField] private Sprite[] countSprits;

    //=====================================
    // 繝｡繧ｽ繝��ラ

    /// <summary>
    /// 蛻晄悄蜃ｦ逅�
    /// </summary>
    async void Start()
    {
        // BGM蜀咲函
        BGMManager.Instance.Pause(BGMPath.MAIN_BGM);
        BGMManager.Instance.Play(BGMPath.MULTI_PLAY);

        if(SceneManager.GetActiveScene().name == "3_OnlinePlayScene")
        {   // 螟ｧ遐ｲ繧ｹ繝�ー繧ｸ縺ｮ譎ゅ↓逋ｺ蟆��ヵ繝ｩ繧ｰOn
            isCannon = true;
        }

        // 繝ｫ繝ｼ繝�繝｢繝��Ν縺ｮ蜿門ｾ�
        roomModel = GameObject.Find("RoomModel").GetComponent<RoomModel>();

        // 蜷��夂衍縺悟ｱ翫＞縺滄圀縺ｫ陦後≧蜃ｦ逅��ｒ繝｢繝��Ν縺ｫ逋ｻ骭ｲ縺吶ｋ
        roomModel.OnJoinedUser += OnJoinedUser;         // 蜈･螳､
        roomModel.OnExitedUser += OnExitedUser;         // 騾�螳､
        roomModel.OnMovedUser += OnMovedUser;           // 遘ｻ蜍�
        roomModel.OnInGameUser += OnInGameUser;         // 繧､繝ｳ繧ｲ繝ｼ繝�
        roomModel.OnStartGameUser += OnStartGameUser;   // 繧ｲ繝ｼ繝�繧ｹ繧ｿ繝ｼ繝�
        roomModel.OnEndGameUser += OnEndGameUser;       // 繧ｲ繝ｼ繝�邨ゆｺ�
        roomModel.OnCrushingUser += OnCrushingUser;     // 謦��ｴ
        roomModel.OnTimeCountUser += OnTimeCountUser;   // 繧ｿ繧､繝�繧ｫ繧ｦ繝ｳ繝�
        roomModel.OnTimeUpUser += OnTimeUpUser;         // 繧ｿ繧､繝�繧｢繝�プ
        roomModel.OnShotUser += OnShotUser;             // 螟ｧ遐ｲ逋ｺ蟆�

        // 蛻ｶ髯先凾髢薙��蛻晄悄蛹�
        timerText.text = timeLimit.ToString();

        // 謗･邯�
        await roomModel.ConnectAsync();
        // 蜈･螳､ (繝ｫ繝ｼ繝�蜷阪→繝ｦ繝ｼ繧ｶ繝ｼID繧呈ｸ｡縺励※蜈･螳､縲よ怙邨ら噪縺ｫ縺ｯ繝ｭ繝ｼ繧ｫ繝ｫ繝�ー繧ｿ縺ｮ繝ｦ繝ｼ繧ｶ繝ｼID繧剃ｽｿ逕ｨ縺吶ｋ
        await roomModel.JoinAsync();

        Debug.Log("蜈･螳､");
    }

    /// <summary>
    /// 遘ｻ蜍輔ョ繝ｼ繧ｿ騾∽ｿ｡蜃ｦ逅�
    /// </summary>
    private async void SendMoveData()
    {
        if (gameState == GameState.None) return;

        // 遘ｻ蜍輔ョ繝ｼ繧ｿ縺ｮ菴懈��
        var moveData = new MoveData()
        {
            ConnectionId = roomModel.ConnectionId,
            Position = playerController.transform.position + posCorrection,
            Rotation = visualTransform.eulerAngles,
            WheelAngle = wheelAngle.localEulerAngles.y,
            IsTurbo = playerTurboParticle.isPlaying,
            IsDrift = playerDriftParticle.isPlaying
        };

        await roomModel.MoveAsync(moveData);
    }

    /// <summary>
    /// 蛻��妙蜃ｦ逅�
    /// </summary>
    public async void OnDisconnect()
    {
        CancelInvoke();

        // 騾�蜃ｺ
        await roomModel.DisconnectionAsync();

        // 繝励Ξ繧､繝､繝ｼ繧ｪ繝悶ず繧ｧ繧ｯ繝医��蜑企勁
        foreach (Transform child in parentObj.transform)
        {
            Destroy(child.gameObject);
        }

        gameState = GameState.None;
        Debug.Log("騾�蜃ｺ");
    }

    /// <summary>
    /// 繧ｲ繝ｼ繝�繧ｹ繧ｿ繝ｼ繝磯�夂衍蜃ｦ逅�
    /// </summary>
    public async void OnStart()
    {
        await roomModel.GameStartAsync();
    }

    // --------------------------------------------------------------
    // 繝｢繝��Ν逋ｻ骭ｲ逕ｨ髢｢謨ｰ

    /// <summary>
    /// 蜈･螳､騾夂衍蜿嶺ｿ｡譎ゅ��蜃ｦ逅�
    /// </summary>
    /// <param name="user"></param>
    private void OnJoinedUser(JoinedUser user)
    {
        if (characterList.ContainsKey(user.ConnectionId)) return;

        GameObject characterObj;    // 逕滓��縺輔ｌ繧九が繝悶ず繧ｧ繧ｯ繝�

        // 閾ｪ蛻��°莉悶��繝ｬ繧､繝､繝ｼ縺句愛譁ｭ
        if (user.ConnectionId == roomModel.ConnectionId)
        {
            // 蜿ょ刈鬆�の菫晏ｭ�
            joinOrder = user.JoinOrder;

            // 閾ｪ讖溘��謫堺ｽ懃畑繧ｪ繝悶ず繧ｧ縺ｮ逕滓��
            if(joinOrder == 1 || joinOrder == 3)
            {
                characterObj = Instantiate(playerPrefab, respownList[user.JoinOrder - 1].position, Quaternion.Euler(0, 0, 0));
                inputController = Instantiate(inputPrefab, Vector3.zero, Quaternion.identity);
            }
            else
            {
                characterObj = Instantiate(playerPrefab, respownList[user.JoinOrder - 1].position, Quaternion.Euler(0, 180, 0));
                inputController = Instantiate(inputPrefab, Vector3.zero, Quaternion.identity);
            }

            // 繧ｫ繝ｼ繧ｳ繝ｳ繝医Ο繝ｼ繝ｩ繝ｼ縺ｮ蜿門ｾ励��蜿肴丐
            playerController = characterObj.transform.GetChild(0).gameObject;
            wheelAngle = characterObj.transform.Find("Visuals/WheelFrontLeft").transform;

            // 繝励Ξ繧､繝､繝ｼ縺ｮ菴咲ｽｮ諠��ｱ繧貞叙蠕�
            visualTransform = characterObj.transform.GetChild(1).gameObject.GetComponent<Transform>();

            // 繝代��繝��ぅ繧ｯ繝ｫ縺ｮ蜿門ｾ�
            playerTurboParticle = characterObj.transform.Find("Visuals/ParticlesBoost").GetComponent<ParticleSystem>();
            playerDriftParticle = characterObj.transform.Find("Visuals/ParticlesDrifting").GetComponent<ParticleSystem>();

            // 繝｢繝舌う繝ｫ繧､繝ｳ繝励ャ繝医↓繧ｫ繝ｼ繧ｳ繝ｳ繝医Ο繝ｼ繝ｩ繝ｼ繧定ｨｭ螳�
            tinyCarMobileInput.carController = playerController.GetComponent<TinyCarController>();

            // 繝ｦ繝ｼ繧ｶ繝ｼ蜷攻I縺ｮ霑ｽ蠕楢ｨｭ螳� & 蜷榊燕蜿肴丐
            nameObjs[user.JoinOrder - 1].GetComponent<NameTracker>().SetTarget(playerController.transform, 1);
            nameTexts[user.JoinOrder - 1].text = user.UserData.Name;
            nameObjs[user.JoinOrder - 1].SetActive(true);

            // UI螟画峩
            gameState = GameState.Join;
            Debug.Log("閾ｪ讖溽函謌仙ｮ御ｺ�");

            // 菴咲ｽｮ騾∽ｿ｡髢句ｧ�
            InvokeRepeating("SendMoveData", 0.5f, internetSpeed);
        }
        else
        {
            // 莉悶��繝ｬ繧､繝､繝ｼ縺ｮ逕滓��
            characterObj = Instantiate(otherPrefab, respownList[user.JoinOrder - 1].position, Quaternion.Euler(0, 180, 0));
            characterObj.GetComponent<OtherPlayerManager>().ConnectionID = user.ConnectionId;   // 謗･邯唔D縺ｮ菫晏ｭ�
            characterObj.GetComponent<OtherPlayerManager>().UserName = user.UserData.Name;      // 繝ｦ繝ｼ繧ｶ繝ｼ蜷阪��菫晏ｭ�
            characterObj.GetComponent<OtherPlayerManager>().JoinOrder = user.JoinOrder;         // 蜿ょ刈鬆�の菫晏ｭ�

            // 繝ｦ繝ｼ繧ｶ繝ｼ蜷攻I縺ｮ霑ｽ蠕楢ｨｭ螳� & 蜷榊燕蜿肴丐
            nameObjs[user.JoinOrder - 1].GetComponent<NameTracker>().SetTarget(characterObj.transform, 2);
            nameTexts[user.JoinOrder - 1].text = user.UserData.Name;
            nameObjs[user.JoinOrder - 1].SetActive(true);
        }

        characterObj.transform.parent = parentObj.transform;    // 隕ｪ繧定ｨｭ螳�
        characterList[user.ConnectionId] = characterObj;        // 繝輔ぅ繝ｼ繝ｫ繝峨↓菫晏ｭ�

        Debug.Log(user.JoinOrder + "P蜿ょ刈");
    }

    /// <summary>
    /// 騾�蜃ｺ騾夂衍蜿嶺ｿ｡譎ゅ��蜃ｦ逅�
    /// </summary>
    /// <param name="user"></param>
    private void OnExitedUser(JoinedUser user)
    {
        if (!characterList.ContainsKey(user.ConnectionId)) return;  // 騾�蜃ｺ閠��が繝悶ず繧ｧ縺ｮ蟄伜惠繝√ぉ繝��け

        if(gameState == GameState.Result)
        {
            Destroy(characterList[user.ConnectionId]);   // 繧ｪ繝悶ず繧ｧ繧ｯ繝医��遐ｴ譽�
            characterList.Remove(user.ConnectionId);     // 繝ｪ繧ｹ繝医°繧牙炎髯､
        }
        else
        {
            // 莉悶��繝ｬ繧､繝､繝ｼ縺ｮ蛻��妙陦ｨ遉ｺ繝ｻ謚ｼ荳九〒繝｡繝九Η繝ｼ縺ｫ謌ｻ繧�
            disconnectPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 繝励Ξ繧､繝､繝ｼ縺檎ｧｻ蜍輔＠縺溘→縺阪��蜃ｦ逅�
    /// </summary>
    /// <param name="moveData"></param>
    private void OnMovedUser(MoveData moveData)
    {
        // 菴咲ｽｮ諠��ｱ縺ｮ譖ｴ譁ｰ
        if (!characterList.ContainsKey(moveData.ConnectionId)) return;

        // 譛ｬ菴謎ｽ咲ｽｮ縺ｮ譖ｴ譁ｰ
        characterList[moveData.ConnectionId].transform.DOMove(moveData.Position, internetSpeed).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed, true);
        characterList[moveData.ConnectionId].transform.DORotate(moveData.Rotation, internetSpeed).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed, true);

        // 繧ｿ繧､繝､隗偵��譖ｴ譁ｰ
        Transform wheelR = characterList[moveData.ConnectionId].transform.Find("wheels/wheel front right").transform;
        Transform wheelL = characterList[moveData.ConnectionId].transform.Find("wheels/wheel front left").transform;
        wheelR.localEulerAngles = new Vector3(wheelR.localEulerAngles.x, moveData.WheelAngle, 0);
        wheelL.localEulerAngles = new Vector3(wheelL.localEulerAngles.x, moveData.WheelAngle, 0);

        // 繝代��繝��ぅ繧ｯ繝ｫ縺ｮ蜿門ｾ励��譖ｴ譁ｰ
        characterList[moveData.ConnectionId].GetComponent<OtherPlayerManager>().playDrift(moveData.IsDrift);
        characterList[moveData.ConnectionId].GetComponent<OtherPlayerManager>().playTurbo(moveData.IsTurbo);
    }

    /// <summary>
    /// 繧､繝ｳ繧ｲ繝ｼ繝�騾夂衍蜿嶺ｿ｡蜃ｦ逅�
    /// </summary>
    private void OnInGameUser()
    {
        // 繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ髢句ｧ�
        Debug.Log("繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ");
        StartCoroutine("StartCount");
    }

    /// <summary>
    /// 繧ｲ繝ｼ繝�繧ｹ繧ｿ繝ｼ繝磯�夂衍蜿嶺ｿ｡蜃ｦ逅�
    /// </summary>
    private void OnStartGameUser()
    {
        SEManager.Instance.Play(SEPath.START);

        // 繝��く繧ｹ繝医��螟画峩
        countDownImage.sprite = countSprits[0];
        StartCoroutine("HiddenText");

        // 蛻ｶ髯先凾髢楢｡ｨ遉ｺ繝ｻ繝帙せ繝医��繧ｫ繧ｦ繝ｳ繝磯幕蟋�
        timerPanel.transform.DOLocalMove(new Vector3(820, 450, 0), 0.6f);
        if (joinOrder == 1)
        {
            InvokeRepeating("CountTime", 1, 1);
        }

        // 繧ｫ繝｡繝ｩ繧偵ヨ繝�プ繝�繧ｦ繝ｳ縺ｫ螟画峩
        mainCamera.GetComponent<TinyCarCamera>().whatToFollow = playerController.transform;
        // 謫堺ｽ懷庄閭ｽ縺ｫ縺吶ｋ
        inputController.GetComponent<TinyCarStandardInput>().carController = playerController.GetComponent<TinyCarController>();
        mobileInputObj.SetActive(true);

        gameState = GameState.InGame;
    }

    /// <summary>
    /// 繧ｲ繝ｼ繝�邨ゆｺ��夂衍蜿嶺ｿ｡蜃ｦ逅�
    /// </summary>
    private void OnEndGameUser(List<ResultData> result)
    {
        gameState = GameState.Result;

        // 謫堺ｽ應ｸ崎��縺ｫ縺吶ｋ
        mobileInputObj.SetActive(false);

        // 邨ゆｺ�SE蜀咲函
        SEManager.Instance.Play(SEPath.GOAL);

        // 邨ゆｺ��｡ｨ遉ｺ
        endImageObj.SetActive(true);

        // 繝ｪ繧ｶ繝ｫ繝医ヱ繝阪Ν縺ｫ邨先棡繧貞渚譏� (閾ｪ蛻�の鬆��ｽ阪��繝ｬ繝ｼ繝医ｒ蜿肴丐)
        foreach(ResultData resultData in result)
        {
            if(resultData.UserId == roomModel.UserId)
            {
                rateText.text = resultData.Rate.ToString();                         // 蜿門ｾ励＠縺溘Ξ繝ｼ繝医ｒ陦ｨ遉ｺ
                changeRateText.text = Math.Abs(resultData.ChangeRate).ToString();   // 蠅玲ｸ帙Ξ繝ｼ繝医ｒ陦ｨ遉ｺ
                if(resultData.ChangeRate < 0)
                {   // 貂帷ｮ励��蝣ｴ蜷医�� - 繧定｡ｨ遉ｺ
                    signText.text = "-";
                }

                rankImages[resultData.Rank - 1].SetActive(true);        // 隧ｲ蠖馴���ｽ阪��陦ｨ遉ｺ
            }
        }

        // 1遘貞ｾ後↓繝ｪ繧ｶ繝ｫ繝郁｡ｨ遉ｺ
        StartCoroutine("DisplayResult");
    }

    /// <summary>
    /// 繝励Ξ繧､繝､繝ｼ謦��ｴ騾夂衍蜃ｦ逅�
    /// </summary>
    /// <param name="attackName">謦��ｴ縺励◆莠ｺ縺ｮPL蜷�</param>
    /// <param name="crushName"> 謦��ｴ縺輔ｌ縺滉ｺｺ縺ｮPL蜷�</param>
    /// <param name="crushID">   謦��ｴ縺輔ｌ縺滉ｺｺ縺ｮ謗･邯唔D</param>
    private void OnCrushingUser(string attackName, string crushName, Guid crushID, int deadNo)
    {
        // 謦��ｴ騾夂衍繝��く繧ｹ繝医��蜀��ｮｹ螟画峩繝ｻ陦ｨ遉ｺ縺吶ｋ ()
        if(deadNo == 1)
        {
            crushText.GetComponent<Text>().text = attackName + " 縺� " + crushName + "繧呈茶遐ｴ！";
        }
        else if(deadNo == 2)
        {
            crushText.GetComponent<Text>().text = crushName + "縺檎��遐ｴ！";
        }

        // 騾夂衍陦ｨ遉ｺSequence繧剃ｽ懈��
        var sequence = DOTween.Sequence();
        sequence.Append(crushText.transform.DOLocalMove(new Vector3(0f, 450f, 0f), 2.5f));
        sequence.Append(crushText.transform.DOLocalMove(new Vector3(0f, 830f, 0f), 0.5f));
        sequence.Play();
        
        // 謦��ｴ縺輔ｌ縺溘��繝ｬ繧､繝､繝ｼ縺ｮ蜷榊燕繧帝撼陦ｨ遉ｺ
        foreach(Text name in nameTexts)
        {
            if(name.text == crushName)
            {
                name.text = "";
            }
        }

        // 辷��匱繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ逕滓��
        if (roomModel.ConnectionId == crushID)
        {   // 閾ｪ蛻��′謦��ｴ縺輔ｌ縺溘→縺�
            SEManager.Instance.Play(SEPath.BOOM);
            Instantiate(explosionPrefab, playerController.transform.position, Quaternion.identity); // 辷��匱繧ｨ繝輔ぉ繧ｯ繝�
            mainCamera.GetComponent<TinyCarCamera>().whatToFollow = null;                           // 繧ｫ繝｡繝ｩ繧剃ｿｯ迸ｰ縺ｫ螟画峩
            characterList[crushID].GetComponent<TinyCarExplosiveBody>().explode();                  // 閾ｪ辷�処逅�
            characterList.Remove(crushID);                                                          // PL繝ｪ繧ｹ繝医°繧牙炎髯､
        }
        else
        {
            SEManager.Instance.Play(SEPath.BOOM);
            Instantiate(explosionPrefab, characterList[crushID].transform.position, Quaternion.identity);
            Destroy(characterList[crushID]);
            characterList.Remove(crushID);
        }
    }

    /// <summary>
    /// 谿九ち繧､繝�騾夂衍蜃ｦ逅�
    /// </summary>
    /// <param name="time"></param>
    private void OnTimeCountUser(int time)
    {   // 谿九ち繧､繝�縺ｮ蜿肴丐蜃ｦ逅�
        timerText.text = time.ToString();

        if(time <= DEATH_MATCH_TIME && !isDeathMatch)
        {
            isDeathMatch = true;
        }

        if (3 >= time)
        {
            timerText.color = Color.yellow;
        }
    }

    /// <summary>
    /// 繧ｿ繧､繝�繧｢繝�プ騾夂衍
    /// </summary>
    private void OnTimeUpUser()
    {
        if (gameState == GameState.Result) return;

        // 邨ゆｺ�SE蜀咲函
        SEManager.Instance.Play(SEPath.GOAL);

        // 謫堺ｽ應ｸ崎��縺ｫ縺吶ｋ
        mobileInputObj.SetActive(false);

        // 蜷榊燕髱櫁｡ｨ遉ｺ
        foreach (GameObject obj in nameObjs)
        {
            obj.SetActive(false);
        }

        // 繝ｪ繧ｶ繝ｫ繝郁｡ｨ遉ｺ
        drawImageObj.SetActive(true);
        resultPanel.gameObject.SetActive(true);

        gameState = GameState.Result;
    }

    /// <summary>
    /// 螟ｧ遐ｲ逋ｺ蟆��夂衍
    /// </summary>
    private void OnShotUser(int cannonID)
    {
        if (!isCannon) return;

        // 逋ｺ蟆�処逅�の蜻ｼ縺ｳ蜃ｺ縺�
        if (isDeathMatch)
        {   // 繝��せ繝槭ャ繝∵凾縺ｯ蟇ｾ隗堤ｷ�2縺､縺ｮ螟ｧ遐ｲ繧剃ｽｿ逕ｨ縺吶ｋ
            if(cannonID == 1 || cannonID == 4)
            {
                cannons[0].ShotBullet();
                cannons[3].ShotBullet();
            }
            else
            {
                cannons[1].ShotBullet();
                cannons[2].ShotBullet();
            }
        }
        else
        {
            cannons[cannonID - 1].ShotBullet();
        }
    }

    /// <summary>
    /// 繧ｲ繝ｼ繝�繧ｫ繧ｦ繝ｳ繝�
    /// </summary>
    /// <returns></returns>
    IEnumerator StartCount()
    {
        for (int i = 3; i > 0; i--)
        {
            countDownImage.sprite = countSprits[i];

            SEManager.Instance.Play(SEPath.COUNT);

            // 1遘貞ｾ��▲縺ｦ繧ｳ繝ｫ繝ｼ繝√Φ荳ｭ譁ｭ
            yield return new WaitForSeconds(1.0f);

            if (i == 1)
            {
                OnStart();
            }
        }
    }

    /// <summary>
    /// 谿九ち繧､繝�繧ｫ繧ｦ繝ｳ繝亥��逅�
    /// </summary>
    private async void CountTime()
    {
        timeLimit--;

        if(timeLimit % shotInterval == 0 && joinOrder == 1 && isCannon ==true)
        {   // 逋ｺ蟆��俣髫� && 繝帙せ繝� && 螟ｧ遐ｲ繧ｹ繝�ー繧ｸ縺ｮ譎�
            await roomModel.ShotCannonAsync();
        }

        await roomModel.TimeCountAsync(timeLimit);

        if(timeLimit <= 0)
        {   // 0莉･荳九��譎ゅ��繧ｫ繧ｦ繝ｳ繝育ｵゆｺ�
            CancelInvoke("CountTime");
        }
    } 

    /// <summary>
    /// 繧ｿ繧､繝槭��繝��く繧ｹ繝磯撼陦ｨ遉ｺ蜃ｦ逅�
    /// </summary>
    /// <returns></returns>
    IEnumerator HiddenText()
    {
        // 1遘貞ｾ��▲縺ｦ繧ｳ繝ｫ繝ｼ繝√Φ荳ｭ譁ｭ
        yield return new WaitForSeconds(0.8f);

        countDownImageObj.SetActive(false);
    }

    /// <summary>
    /// 繝ｪ繧ｶ繝ｫ繝郁｡ｨ遉ｺ
    /// </summary>
    /// <returns></returns>
    IEnumerator DisplayResult()
    {
        CancelInvoke("CountTime");

        // 蜷榊燕陦ｨ遉ｺ繧偵☆縺ｹ縺ｦOFF
        foreach (GameObject obj in nameObjs)
        {
            obj.SetActive(false);
        }

        // 1遘貞ｾ��▲縺ｦ繧ｳ繝ｫ繝ｼ繝√Φ荳ｭ譁ｭ
        yield return new WaitForSeconds(1.0f);

        mobileInputObj.SetActive(false);
        endImageObj.SetActive(false);
        rateObjs.SetActive(true);               // 繝ｬ繝ｼ繝郁｡ｨ遉ｺ
        resultPanel.gameObject.SetActive(true); // 繝ｪ繧ｶ繝ｫ繝医ヱ繝阪Ν陦ｨ遉ｺ
    }

    /// <summary>
    /// 繧ｿ繧､繝医Ν繝懊ち繝ｳ謚ｼ荳区凾
    /// </summary>
    public async void OnTitleButton()
    {
        CancelInvoke();

        // SE蜀咲函
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // 騾�蜃ｺ
        await roomModel.ExitAsync();

        // 繝励Ξ繧､繝､繝ｼ繧ｪ繝悶ず繧ｧ繧ｯ繝医��蜑企勁
        foreach (Transform child in parentObj.transform)
        {
            Destroy(child.gameObject);
        }

        gameState = GameState.None;
        Debug.Log("騾�蜃ｺ");

        // 繝ｫ繝ｼ繝�繝｢繝��Ν縺ｮ遐ｴ譽�
        Destroy(GameObject.Find("RoomModel"));

        // 繧ｿ繧､繝医Ν縺ｫ謌ｻ繧�
        Initiate.DoneFading();
        Initiate.Fade("2_MenuScene", Color.white, 2.5f);
    }
}
