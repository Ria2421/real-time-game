//---------------------------------------------------------------
// 登録フラグデータ [ RegistResult.cs ]
// Author:Kenta Nakamoto
//---------------------------------------------------------------
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Model.Entity
{
    [MessagePackObject]
    public class RegistResult
    {
        /// <summary>
        /// タイム登録・更新フラグ
        /// </summary>
        [Key(0)]
        public bool timeRegistFlag { get; set; } = false;

        /// <summary>
        /// ゴースト登録フラグ
        /// </summary>
        [Key(1)]
        public bool ghostRegistFlag { get; set; } = false;
    }
}
