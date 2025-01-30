using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Model.Entity
{
    public class NGWord
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key(0)]
        public int Id { get; set; }

        /// <summary>
        /// NGワード
        /// </summary>
        [Key(1)]
        public string Word { get; set; }
    }
}
