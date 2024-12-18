//---------------------------------------------------------------
//
// セーブデータクラス [ SaveData.cs ]
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
    /// ユーザーID
    /// </summary>
    public int UserID { get; set; }

    /// <summary>
    /// トークン
    /// </summary>
    public string Token { get; set; }
}
