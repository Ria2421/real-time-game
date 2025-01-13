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
        /// ユーザーID
        /// </summary>
        [Key(0)]
        public int UserId { get; set; }

        /// <summary>
        /// ユーザー名
        /// </summary>
        [Key(1)]
        public string UserName { get; set; }

        /// <summary>
        /// クリアタイム(m/sec)
        /// </summary>
        [Key(2)]
        public int ClearTime { get; set; }


    }
}
