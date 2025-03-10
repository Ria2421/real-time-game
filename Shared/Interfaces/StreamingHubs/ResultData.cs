//---------------------------------------------------------------
// リザルトデータ [ ResultData.cs ]
// Author:Kenta Nakamoto
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
        /// ユーザーID
        /// </summary>
        [Key(0)]
        public int UserId { get; set; }

        /// <summary>
        /// 順位
        /// </summary>
        [Key(1)]
        public int Rank { get; set; }

        /// <summary>
        /// 現レートポイント
        /// </summary>
        [Key(2)]
        public int Rate { get; set; }

        /// <summary>
        /// 増減レートポイント
        /// </summary>
        [Key(3)]
        public int ChangeRate { get; set; }
    }
}
