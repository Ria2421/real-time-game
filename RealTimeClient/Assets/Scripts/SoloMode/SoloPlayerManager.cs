//---------------------------------------------------------------
// 繧ｽ繝ｭ繝励Ξ繧､繝､繝ｼ繝槭ロ繝ｼ繧ｸ繝｣繝ｼ [ SoloPlayerManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/18
// Update:2024/12/18
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloPlayerManager : MonoBehaviour
{
    //=====================================
    // 繝輔ぅ繝ｼ繝ｫ繝�

    /// <summary>
    /// 迴ｾ繝√ぉ繝��け繝昴う繝ｳ繝域焚
    /// </summary>
    private int currentCheckPointCnt = 1;

    /// <summary>
    /// 繝√ぉ繝��け繝昴う繝ｳ繝医が繝悶ず繧ｧ譬ｼ邏咲畑
    /// </summary>
    [SerializeField] private GameObject[] checkPoints;

    /// <summary>
    /// 繧ｲ繝ｼ繝�繝槭ロ繝ｼ繧ｸ繝｣繝ｼ
    /// </summary>
    [SerializeField] private SoloManager soloManager;

    //=====================================
    // 繝｡繧ｽ繝��ラ

    /// <summary>
    /// 蛻晄悄蜃ｦ逅�
    /// </summary>
    void Start()
    {
        
    }

    /// <summary>
    /// 譖ｴ譁ｰ蜃ｦ逅�
    /// </summary>
    void Update()
    {
        
    }

    /// <summary>
    /// 繧ｳ繝ｩ繧､繝�繝ｼ謗･隗ｦ譎ょ��逅�
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "CheckPoint" + currentCheckPointCnt.ToString())
        {
            Debug.Log(currentCheckPointCnt.ToString() + "逡ｪ繧帝�夐℃");

            if (currentCheckPointCnt == checkPoints.Length)
            {   // 譛�蠕後��繝√ぉ繝��け繝昴う繝ｳ繝医ｒ騾夐℃縺励◆繧�
                currentCheckPointCnt = 1;   // 繝√ぉ繝��け繝昴う繝ｳ繝医き繧ｦ繝ｳ繝医��繝ｪ繧ｻ繝��ヨ

                // 繝ｩ繝�プ謨ｰ譖ｴ譁ｰ蜃ｦ逅�の蜻ｼ縺ｳ蜃ｺ縺�
                soloManager.AddRapCnt();
            }
            else
            {
                currentCheckPointCnt++;
            }
        }
    }
}
