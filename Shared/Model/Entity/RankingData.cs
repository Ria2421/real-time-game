//---------------------------------------------------------------
// ランキングデータ [ RankingData.cs ]
// Author:Kenta Nakamoto
//---------------------------------------------------------------
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Model.Entity
{
    [MessagePackObject]
    public class RankingData
    {
        /// <summary>
        /// ログID
        /// </summary>
        [Key(0)]
        public int Id { get; set; }

        /// <summary>
        /// ユーザーID
        /// </summary>
        [Key(1)]
        public int UserId { get; set; }

        /// <summary>
        /// ユーザー名
        /// </summary>
        [Key(2)]
        public string UserName { get; set; }

        /// <summary>
        /// クリアタイム(m/sec)
        /// </summary>
        [Key(3)]
        public int ClearTime { get; set; }

        /// <summary>
        /// ゴーストデータ
        /// </summary>
        [Key(4)]
        public string GhostData { get; set; }
    }
}
