//---------------------------------------------------------------
// 繧ｿ繧､繝医Ν繝槭ロ繝ｼ繧ｸ繝｣繝ｼ [ MatchingManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/10
// Update:2025/01/30
//---------------------------------------------------------------
using KanKikuchi.AudioManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchingManager : MonoBehaviour
{
    //-------------------------------------------------------
    // 繝輔ぅ繝ｼ繝ｫ繝�

    /// <summary>
    /// 繝励Ξ繧､繧ｹ繝�ー繧ｸID
    /// </summary>
    private int playStageID = 0;

    /// <summary>
    /// 繝槭ャ繝√Φ繧ｰ繝��く繧ｹ繝医が繝悶ず繧ｧ
    /// </summary>
    [SerializeField] private GameObject matchingTextObj;

    /// <summary>
    /// 繝槭ャ繝√Φ繧ｰ螳御ｺ��ユ繧ｭ繧ｹ繝医が繝悶ず繧ｧ
    /// </summary>
    [SerializeField] private GameObject completeTextObj;

    /// <summary>
    /// 繝ｫ繝ｼ繝�繝｢繝��Ν譬ｼ邏咲畑
    /// </summary>
    [SerializeField] private RoomModel roomModel;

    /// <summary>
    /// 繧ｭ繝｣繝ｳ繧ｻ繝ｫ繝懊ち繝ｳ繧ｪ繝悶ず繧ｧ
    /// </summary>
    [SerializeField] private GameObject cancelObj;

    /// <summary>
    /// 繧ｭ繝｣繝ｳ繧ｻ繝ｫ繝懊ち繝ｳ
    /// </summary>
    [SerializeField] private Button cancelButton;

    /// <summary>
    /// 霆願レ譎ｯ
    /// </summary>
    [SerializeField] private Transform carBG;

    //-------------------------------------------------------
    // 繝｡繧ｽ繝��ラ

    /// <summary>
    /// 蛻晄悄蜃ｦ逅�
    /// </summary>
    async void Start()
    {
        roomModel.OnMatchingUser += OnMatchingUser;     // 繝槭ャ繝√Φ繧ｰ螳御ｺ��夂衍

        Invoke("StartMatching",2.0f);
    }

    /// <summary>
    /// 螳壽悄譖ｴ譁ｰ蜃ｦ逅�
    /// </summary>
    private void FixedUpdate()
    {
        // 霆顔判蜒上ｒ蝗槭☆
        carBG.localEulerAngles += new Vector3(0,0,1.0f);
    }

    /// <summary>
    /// 繧ｭ繝｣繝ｳ繧ｻ繝ｫ繝懊ち繝ｳ
    /// </summary>
    public async void OnCancelButton()
    {
        // SE蜀咲函
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        cancelButton.interactable = false;

        // 蛻��妙 (魃悶°繧牙ｸｰ縺｣縺ｦ縺阪◆繧峨す繝ｼ繝ｳ遘ｻ蜍輔↓縺吶ｋ)
        await roomModel.DisconnectionAsync();

        // 繝｡繝九Η繝ｼ繧ｷ繝ｼ繝ｳ縺ｫ驕ｷ遘ｻ
        Initiate.DoneFading();
        Initiate.Fade("2_MenuScene", Color.white, 2.5f);

        Debug.Log("繝槭ャ繝√Φ繧ｰ荳ｭ豁｢");
    }

    /// <summary>
    /// 繝槭ャ繝√Φ繧ｰ螳御ｺ��夂衍蜿嶺ｿ｡譎ゅ��蜃ｦ逅�
    /// </summary>
    /// <param name="roomName"></param>
    private async void OnMatchingUser(string roomName,int stageID)
    {
        roomModel.RoomName = roomName;  // 逋ｺ陦後＆繧後◆繝ｫ繝ｼ繝�蜷阪ｒ菫晏ｭ�
        playStageID = stageID;          // 繧ｹ繝�ー繧ｸID繧剃ｿ晏ｭ�

        // SE蜀咲函
        SEManager.Instance.Play(SEPath.MATCHING_COMPLETE);
        // 陦ｨ遉ｺ蛻��崛
        cancelObj.SetActive(false);
        matchingTextObj.SetActive(false);
        completeTextObj.SetActive(true);

        // 騾�蜃ｺ
        await roomModel.ExitAsync();

        StartCoroutine("TransGmaeScene");

        Debug.Log("繝槭ャ繝√Φ繧ｰ螳御ｺ�");
    }

    /// <summary>
    /// 繧ｲ繝ｼ繝�繧ｷ繝ｼ繝ｳ驕ｷ遘ｻ
    /// </summary>
    /// <returns></returns>
    private IEnumerator TransGmaeScene()
    {
        // 1遘貞ｾ��▲縺ｦ繧ｳ繝ｫ繝ｼ繝√Φ荳ｭ譁ｭ
        yield return new WaitForSeconds(1.2f);

        // playStageID縺ｫ蠢懊§縺ｦ蟇ｾ蠢懊☆繧九ご繝ｼ繝�繧ｷ繝ｼ繝ｳ縺ｫ驕ｷ遘ｻ
        Initiate.DoneFading();
        Initiate.Fade(playStageID.ToString() + "_OnlinePlayScene", Color.white, 2.5f);
    }

    /// <summary>
    /// 繝槭ャ繝√Φ繧ｰ髢句ｧ句��逅�
    /// </summary>
    private async void StartMatching()
    {
        // 謗･邯�
        await roomModel.ConnectAsync();
        // 繝槭ャ繝√Φ繧ｰ
        await roomModel.JoinLobbyAsync(UserModel.Instance.UserId);
        // 繧ｭ繝｣繝ｳ繧ｻ繝ｫ繝懊ち繝ｳ譛牙柑蛹�
        cancelButton.interactable = true;

        Debug.Log("繝槭ャ繝√Φ繧ｰ髢句ｧ�");
    }
}
