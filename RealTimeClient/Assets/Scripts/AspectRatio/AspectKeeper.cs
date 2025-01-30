//---------------------------------------------------------------
//
// 繧｢繧ｹ繝壹け繝域ｯ比ｿ晄戟 [ AspectKeeper.cs ]
// Author:Kenta Nakamoto
// Data:2024/07/17
// Update:2024/07/17
//
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways] // 蜀咲函譎ゆｻ･螟悶〒繧ょ虚菴懊☆繧�
public class AspectKeeper : MonoBehaviour
{
    //-------------------------------------------
    // 繝輔ぅ繝ｼ繝ｫ繝�

    /// <summary>
    /// 蟇ｾ雎｡縺ｨ縺吶ｋ繧ｫ繝｡繝ｩ
    /// </summary>
    [SerializeField]
    private Camera targetCamera;

    /// <summary>
    /// 逶ｮ逧��ｧ｣蜒丞ｺｦ
    /// </summary>
    [SerializeField]
    private Vector2 aspectVec;

    //--------------------------------------------
    // 繝｡繧ｽ繝��ラ

    /// <summary>
    /// 譖ｴ譁ｰ蜃ｦ逅�
    /// </summary>
    void Update()
    {
        var screenAspect = Screen.width / (float)Screen.height; // 逕ｻ髱｢縺ｮ繧｢繧ｹ繝壹け繝域ｯ�
        var targetAspect = aspectVec.x / aspectVec.y; // 逶ｮ逧�の繧｢繧ｹ繝壹け繝域ｯ�

        var magRate = targetAspect / screenAspect; // 逶ｮ逧��い繧ｹ繝壹け繝域ｯ斐↓縺吶ｋ縺溘ａ縺ｮ蛟咲紫

        var viewportRect = new Rect(0, 0, 1, 1); // Viewport蛻晄悄蛟､縺ｧRect繧剃ｽ懈��

        if (magRate < 1)
        {
            viewportRect.width = magRate; // 菴ｿ逕ｨ縺吶ｋ讓ｪ蟷��ｒ螟画峩
            viewportRect.x = 0.5f - viewportRect.width * 0.5f;// 荳ｭ螟ｮ蟇��○
        }
        else
        {
            viewportRect.height = 1 / magRate; // 菴ｿ逕ｨ縺吶ｋ邵ｦ蟷��ｒ螟画峩
            viewportRect.y = 0.5f - viewportRect.height * 0.5f;// 荳ｭ螟ｮ蟇��○
        }

        targetCamera.rect = viewportRect; // 繧ｫ繝｡繝ｩ縺ｮViewport縺ｫ驕ｩ逕ｨ
    }
}