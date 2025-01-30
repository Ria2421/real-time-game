//---------------------------------------------------------------
// 繝ｦ繝ｼ繧ｶ繝ｼ蜷崎ｿｽ蠕� [ NameTracker.cs ]
// Author:Kenta Nakamoto
// Data:2025/01/16
// Update:2025/01/16
// 蜿り�ザRL:https://tech.pjin.jp/blog/2017/07/14/unity_ugui_sync_rendermode/
//---------------------------------------------------------------
using DavidJalbert;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameTracker : MonoBehaviour
{
    //=====================================
    // 繝輔ぅ繝ｼ繝ｫ繝�

    /// <summary>
    /// 霑ｽ蠕灘ｯｾ雎｡
    /// </summary>
    private Transform targetTfm;

    /// <summary>
    /// 陦ｨ遉ｺUI縺ｮRectTransform
    /// </summary>
    private RectTransform myRectTfm;

    /// <summary>
    /// 陦ｨ遉ｺ繧ｪ繝輔そ繝��ヨ
    /// </summary>
    private Vector3 offset;

    /// <summary>
    /// 蜷榊燕繝輔か繝ｳ繝医し繧､繧ｺ
    /// </summary>
    private int fontSize;

    /// <summary>
    /// 菫ｯ迸ｰ隕也せ譎ゅ��繝輔か繝ｳ繝医し繧､繧ｺ
    /// </summary>
    private const int topFontSize = 120;

    /// <summary>
    /// 隨ｬ荳芽���ｦ也せ譎ゅ��繝輔か繝ｳ繝医し繧､繧ｺ
    /// </summary>
    private const int thirdFontSize = 85;

    /// <summary>
    /// 繝｡繧､繝ｳ繧ｫ繝｡繝ｩ
    /// </summary>
    [SerializeField] private Transform cameraTrs;

    /// <summary>
    /// 繧ｫ繝｡繝ｩ繧ｹ繧ｯ繝ｪ繝励ヨ
    /// </summary>
    [SerializeField] private TinyCarCamera tinyCarCamera;

    /// <summary>
    /// name繝��く繧ｹ繝�
    /// </summary>
    [SerializeField] private Text nameText;

    //=====================================
    // 繝｡繧ｽ繝��ラ

    /// <summary>
    /// 蛻晄悄蜃ｦ逅�
    /// </summary>
    void Start()
    {
        myRectTfm = GetComponent<RectTransform>();
    }

    /// <summary>
    /// 螳壽悄譖ｴ譁ｰ蜃ｦ逅�
    /// </summary>
    void FixedUpdate()
    {
        if (targetTfm == null) return;

        //++ 陦ｨ遉ｺ菴咲ｽｮ繧定ｿｽ蠕灘ｯｾ雎｡+ 繧ｪ繝輔そ繝��ヨ縺ｮ蠎ｧ讓吶↓遘ｻ蜍輔＠邯壹￠繧�
        myRectTfm.position = targetTfm.position + offset;

        myRectTfm.rotation = cameraTrs.rotation; 

        // 繧ｫ繝｡繝ｩ繝｢繝ｼ繝峨↓繧医▲縺ｦ繝輔か繝ｳ繝医し繧､繧ｺ繧貞､画峩縺吶ｋ
        if(tinyCarCamera.viewMode == TinyCarCamera.CAMERA_MODE.ThirdPerson)
        {
            fontSize = thirdFontSize;
        }
        else if (tinyCarCamera.viewMode == TinyCarCamera.CAMERA_MODE.TopDown)
        {
            fontSize = topFontSize;
        }

        nameText.fontSize = fontSize;
    }

    /// <summary>
    /// 霑ｽ蠕灘ｯｾ雎｡險ｭ螳壼��逅�
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target,int no)
    {
        targetTfm = target;

        // 繧ｪ繝輔そ繝��ヨ霍晞屬縺ｮ螟画峩
        if(no == 1)
        {
            offset = new Vector3(0, 2.3f, 0);
        }else if(no == 2)
        {
            offset = new Vector3(0, 3.5f, 0);
        }
    }
}
