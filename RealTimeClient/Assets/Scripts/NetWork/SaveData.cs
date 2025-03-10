//---------------------------------------------------------------
// セーブデータクラス [ SaveData.cs ]
// Author:Kenta Nakamoto
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    /// <summary>
    /// ユーザーID
    /// </summary>
    public int UserID { get; set; }

    /// <summary>
    /// トークン
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// チュートリアルフラグ
    /// </summary>
    public bool TutorialFlag { get; set; }
}
