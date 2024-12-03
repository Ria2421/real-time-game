using MessagePack;
using RealTimeServer.Model.Entity;
using System;
using UnityEngine;

namespace Shared.Interfaces.StreamingHubs
{
    [MessagePackObject]
    public class MoveData
    {
        /// <summary>
        /// 接続ID
        /// </summary>
        [Key(0)]
        public Guid ConnectionId { get; set; }

        /// <summary>
        /// 位置情報
        /// </summary>
        [Key(1)]
        public Vector3 Position { get; set; }

        /// <summary>
        /// 車体角度情報
        /// </summary>
        [Key(2)]
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// タイヤ角
        /// </summary>
        [Key(3)]
        public float WheelAngle { get; set; }
    }
}
