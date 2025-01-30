//---------------------------------------------------------------
// 繧ｿ繝�プ繧ｨ繝輔ぉ繧ｯ繝医��繝阪��繧ｸ繝｣繝ｼ [ TapEffectManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/17
// Update:2024/12/17
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapEffectManager : MonoBehaviour
{
    //=====================================
    // 繝輔ぅ繝ｼ繝ｫ繝�

    /// <summary>
    /// 繝｡繧､繝ｳ繧ｫ繝｡繝ｩ
    /// </summary>
    [SerializeField] private Camera mainCamera;

    /// <summary>
    /// 逕滓��繧ｨ繝輔ぉ繧ｯ繝�
    /// </summary>
    [SerializeField] private GameObject tapEffect;

    //=====================================
    // 繝｡繧ｽ繝��ラ

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 逕ｻ髱｢繧偵ち繝�プ縺励◆縺ｨ縺阪��蜃ｦ逅�
            Vector2 tapPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition + mainCamera.transform.forward * 10);

            // 繧ｨ繝輔ぉ繧ｯ繝医ｒ逕滓��
            GameObject effect = Instantiate(tapEffect, tapPosition, Quaternion.identity, this.transform);
        }
    }
}
