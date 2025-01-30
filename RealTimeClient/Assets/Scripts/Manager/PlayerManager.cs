//---------------------------------------------------------------
// 繝励Ξ繧､繝､繝ｼ繝槭ロ繝ｼ繧ｸ繝｣繝ｼ [ PlayerManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/05
// Update:2025/01/23
//---------------------------------------------------------------
using DavidJalbert;
using Shared.Interfaces.StreamingHubs;
using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //=====================================
    // 繝輔ぅ繝ｼ繝ｫ繝�

    /// <summary>
    /// 繝ｫ繝ｼ繝�繝｢繝��Ν譬ｼ邏咲畑
    /// </summary>
    private RoomModel roomModel;

    /// <summary>
    /// 繧ｲ繝ｼ繝�邨ゆｺ��ヵ繝ｩ繧ｰ
    /// </summary>
    private bool isDead = false;

    //=====================================
    // 繝｡繧ｽ繝��ラ

    // 蛻晄悄蜃ｦ逅�
    void Start()
    {
        // 繝ｫ繝ｼ繝�繝｢繝��Ν縺ｮ蜿門ｾ�
        roomModel = GameObject.Find("RoomModel").GetComponent<RoomModel>();
    }

    // 繧ｲ繝ｼ繝�邨ゆｺ��夂衍騾∽ｿ｡蜃ｦ逅�
    private async void SendEndGame()
    {
        await roomModel.GameEndAsync();
    }

    // 謦��ｴ騾夂衍蜃ｦ逅�
    private async void CrushingPlayerAsync(string attackName, string cruchName, Guid crushID, int deadNo)
    {
        await roomModel.CrushingPlayerAsync(attackName, cruchName, crushID, deadNo);
    }

    // 繝医Μ繧ｬ繝ｼ繧ｳ繝ｩ繧､繝�繝ｼ謗･隗ｦ譎ょ��逅�
    private void OnTriggerEnter(Collider collision)
    {
        if(isDead) return;

        // Tag豈弱��謗･隗ｦ譎ょ��逅�

        if(collision.gameObject.tag == "Trap")
        {   // 關ｽ荳区ｭｻ
            // 謦��ｴ騾夂衍 (蛟偵＠縺蘖L蜷�,蛟偵＆繧後◆PL蜷�,蛟偵＆繧後◆莠ｺ縺ｮ謗･邯唔D,豁ｻ莠｡隕∝屏ID)
            isDead = true;
            CrushingPlayerAsync("", roomModel.UserName, roomModel.ConnectionId, 2);
        }

        if(collision.gameObject.tag == "OtherPlayer")
        {   // 莉悶��繝ｬ繧､繝､繝ｼ繧呈茶遐ｴ
            CrushingPlayerAsync(roomModel.UserName,collision.GetComponent<OtherPlayerManager>().UserName,collision.GetComponent<OtherPlayerManager>().ConnectionID, 1);
        }
    }
}