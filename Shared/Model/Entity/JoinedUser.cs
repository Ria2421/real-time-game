//---------------------------------------------------------------
// 参加者データ [ JoinedUser.cs ]
// Author:Kenta Nakamoto
// Data:2024/11/18
// Update:2024/12/06
//---------------------------------------------------------------
using MessagePack;
using RealTimeServer.Model.Entity;
using System;
using UnityEngine;

namespace Shared.Interfaces.StreamingHubs
{
    [MessagePackObject]
    public class JoinedUser
    {
        /// <summary>
        /// 接続ID
        /// </summary>
        [Key(0)]
        public Guid ConnectionId { get; set; }

        /// <summary>
        /// ユーザーデータ
        /// </summary>
        [Key(1)]
        public User UserData { get; set; }

        /// <summary>
        /// 参加順
        /// </summary>
        [Key(2)]
        public int JoinOrder { get; set; }

        /// <summary>
        /// ゲーム状態 [0:None 1:参加 2:インゲーム]
        /// </summary>
        [Key(3)]
        public int GameState { get; set; }

        /// <summary>
        /// ゲーム順位
        /// </summary>
        [Key(4)]
        public int Ranking { get; set; } = 0;
    }
}
