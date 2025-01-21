using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Model.Entity
{
    [MessagePackObject]
    public class SoloPlayData
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key(0)]
        public int Id { get; set; }

        /// <summary>
        /// ステージID
        /// </summary>
        [Key(1)]
        public int Stage_Id { get; set; }

        /// <summary>
        /// ユーザーID
        /// </summary>
        [Key(2)]
        public int User_Id { get; set; }

        /// <summary>
        /// 車種ID
        /// </summary>
        [Key(3)]
        public int Car_Type_Id { get; set; }

        /// <summary>
        /// クリアタイム (m/sec)
        /// </summary>
        [Key(4)]
        public int Clear_Time_Msec { get; set; }

        /// <summary>
        /// ゴーストデータ
        /// </summary>
        [Key(5)]
        public string Ghost_Data { get; set; }

        /// <summary>
        /// 生成日時
        /// </summary>
        [Key(6)]
        public DateTime Created_at { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        [Key(7)]
        public DateTime Updated_at { get; set; }
    }
}
