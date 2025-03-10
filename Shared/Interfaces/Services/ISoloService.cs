//---------------------------------------------------------------
// タイムアタック用APIインターフェース [ ISoloService.cs ]
// Author:Kenta Nakamoto
//---------------------------------------------------------------
using MagicOnion;
using RealTimeServer.Model.Entity;
using Shared.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Interfaces.Services
{
    public interface ISoloService: IService<ISoloService>
    {
        /// <summary>
        /// タイム登録
        /// [return:成否]
        /// </summary>
        /// <param name="stageID">プレイしたステージNo</param>
        /// <param name="userID"> ユーザーID</param>
        /// <param name="time">   登録タイム</param>
        /// <returns></returns>
        UnaryResult<RegistResult> RegistClearTimeAsync(int stageID, int userID, int time,string ghostData);

        /// <summary>
        /// ステージ毎のランキングを取得
        /// [return:ランキングデータ]
        /// </summary>
        /// <param name="stageID"> ステージID</param>
        /// <returns></returns>
        UnaryResult<List<RankingData>> GetRankingAsync(int stageID);
    }
}
