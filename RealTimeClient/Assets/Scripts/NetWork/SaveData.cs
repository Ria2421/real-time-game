//---------------------------------------------------------------
//
// 繧ｻ繝ｼ繝悶ョ繝ｼ繧ｿ繧ｯ繝ｩ繧ｹ [ SaveData.cs ]
// Author:Kenta Nakamoto
// Data:2024/08/26
// Update:2024/08/26
//
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    /// <summary>
    /// 繝ｦ繝ｼ繧ｶ繝ｼID
    /// </summary>
    public int UserID { get; set; }

    /// <summary>
    /// 繝医��繧ｯ繝ｳ
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// 繝√Η繝ｼ繝医Μ繧｢繝ｫ繝輔Λ繧ｰ
    /// </summary>
    public bool TutorialFlag { get; set; }
}
