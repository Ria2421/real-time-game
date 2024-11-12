using MessagePack;
using System;

namespace RealTimeServer.Model.Entity
{
    [MessagePackObject]
    public class User
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key(0)]
        public int Id { get; set; }

        /// <summary>
        /// ユーザー名
        /// </summary>
        [Key(1)]
        public string Name { get; set; }

        /// <summary>
        /// トークン
        /// </summary>
        [Key(2)]
        public string Token { get; set; }

        /// <summary>
        /// 生成日時
        /// </summary>
        [Key(3)]
        public DateTime Created_at { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        [Key(4)]
        public DateTime Updated_at { get; set; }
    }
}
