//---------------------------------------------------------------
// 繧ｿ繧､繝医Ν繧ｪ繝悶ず繧ｧ逕ｨ繧ｹ繧ｯ繝ｪ繝励ヨ [ TitleObj.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/16
// Update:2024/12/16
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitlrObj : MonoBehaviour
{
    //-------------------------------------------------------
    // 繝輔ぅ繝ｼ繝ｫ繝�

    /// <summary>
    /// 辷��匱繧ｨ繝輔ぉ繧ｯ繝�
    /// </summary>
    [SerializeField] GameObject boomEffect;

    //-------------------------------------------------------
    // 繝｡繧ｽ繝��ラ

    /// <summary>
    /// 繝医Μ繧ｬ繝ｼ繧ｳ繝ｩ繧､繝�繝ｼ謗･隗ｦ譎ょ��逅�
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "OtherPlayer")
        {
            // 辷��匱繧ｨ繝輔ぉ繧ｯ繝�
            Instantiate(boomEffect, collision.gameObject.transform.position,Quaternion.identity);
        }
    }
}
