//---------------------------------------------------------------
//
// 郢ｧ�｢郢ｧ�ｹ郢晏｣ｹ縺醍ｹ晏沺�ｯ豈費ｽｿ譎��亜 [ AspectKeeper.cs ]
// Author:Kenta Nakamoto
// Data:2024/07/17
// Update:2024/07/17
//
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways] // 陷�蜥ｲ蜃ｽ隴弱ｆ�ｻ�･陞滓じ縲堤ｹｧ繧��劒闖ｴ諛岩��郢ｧ�
public class AspectKeeper : MonoBehaviour
{
    //-------------------------------------------
    // 郢晁ｼ斐≦郢晢ｽｼ郢晢ｽｫ郢揄

    /// <summary>
    /// 陝��ｽｾ髮趣ｽ｡邵ｺ�ｨ邵ｺ蜷ｶ�狗ｹｧ�ｫ郢晢ｽ｡郢晢ｽｩ
    /// </summary>
    [SerializeField]
    private Camera targetCamera;

    /// <summary>
    /// 騾ｶ�ｮ騾ｧ���ｧ�｣陷剃ｸ橸ｽｺ�ｦ
    /// </summary>
    [SerializeField]
    private Vector2 aspectVec;

    //--------------------------------------------
    // 郢晢ｽ｡郢ｧ�ｽ郢揀聽Λ

    /// <summary>
    /// 隴厄ｽｴ隴��ｽｰ陷��ｽｦ騾��
    /// </summary>
    void Update()
    {
        var screenAspect = Screen.width / (float)Screen.height; // 騾包ｽｻ鬮ｱ�｢邵ｺ�ｮ郢ｧ�｢郢ｧ�ｹ郢晏｣ｹ縺醍ｹ晏沺�ｯ�
        var targetAspect = aspectVec.x / aspectVec.y; // 騾ｶ�ｮ騾ｧ�の郢ｧ�｢郢ｧ�ｹ郢晏｣ｹ縺醍ｹ晏沺�ｯ�

        var magRate = targetAspect / screenAspect; // 騾ｶ�ｮ騾ｧ��縺��ｹｧ�ｹ郢晏｣ｹ縺醍ｹ晏沺�ｯ譁絶��邵ｺ蜷ｶ�狗ｸｺ貅假ｽ∫ｸｺ�ｮ陋溷調邏ｫ

        var viewportRect = new Rect(0, 0, 1, 1); // Viewport陋ｻ譎��ｄ陋滂ｽ､邵ｺ�ｧRect郢ｧ蜑��ｽｽ諛���

        if (magRate < 1)
        {
            viewportRect.width = magRate; // 闖ｴ�ｿ騾包ｽｨ邵ｺ蜷ｶ�玖ｮ難ｽｪ陝ｷ���定棔逕ｻ蟲ｩ
            viewportRect.x = 0.5f - viewportRect.width * 0.5f;// 闕ｳ�ｭ陞滂ｽｮ陝℡聹雷
        }
        else
        {
            viewportRect.height = 1 / magRate; // 闖ｴ�ｿ騾包ｽｨ邵ｺ蜷ｶ�矩し�ｦ陝ｷ���定棔逕ｻ蟲ｩ
            viewportRect.y = 0.5f - viewportRect.height * 0.5f;// 闕ｳ�ｭ陞滂ｽｮ陝℡聹雷
        }

        targetCamera.rect = viewportRect; // 郢ｧ�ｫ郢晢ｽ｡郢晢ｽｩ邵ｺ�ｮViewport邵ｺ�ｫ鬩包ｽｩ騾包ｽｨ
    }
}