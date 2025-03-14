//---------------------------------------------------------------
// ゴーストデータクラス [ GhostData.cs ]
// Author:Kenta Nakamoto
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostData
{
    /// <summary>
    /// 位置
    /// </summary>
    public Vector3 Pos { get; set; }

    /// <summary>
    /// 角度
    /// </summary>
    public Vector3 Rot { get; set; }

    /// <summary>
    /// タイヤ角
    /// </summary>
    public float WRot { get; set; }
}
