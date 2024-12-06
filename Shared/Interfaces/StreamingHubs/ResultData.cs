//---------------------------------------------------------------
// リザルトデータ [ ResultData.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/06
// Update:2024/12/06
//---------------------------------------------------------------
using MessagePack;
using RealTimeServer.Model.Entity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shared.Interfaces.StreamingHubs
{
    [MessagePackObject]
    public class ResultData
    {
        /// <summary>
        /// 順位データ
        /// </summary>
        [Key(0)]
        public List<Dictionary<int, string>> Rank { get; set; }
    }
}
