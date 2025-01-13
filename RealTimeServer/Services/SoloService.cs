using MagicOnion;
using MagicOnion.Server;
using MagicOnionServer.Model.Context;
using RealTimeServer.Model.Entity;
using Shared.Interfaces.Services;
using Shared.Model.Entity;
using System.Xml.Linq;

namespace RealTimeServer.Services
{
    public class SoloService : ServiceBase<ISoloService>, ISoloService
    {
        /// <summary>
        /// タイム登録
        /// [return:成否]
        /// </summary>
        /// <param name="stageID">プレイしたステージNo</param>
        /// <param name="userID"> ユーザーID</param>
        /// <param name="time">   登録タイム</param>
        /// <returns></returns>
        public async UnaryResult<bool> RegistClearTimeAsync(int stageID, int userID,int time)
        {
            using var context = new GameDbContext();

            // 該当データが無いか検索
            SoloPlayLog soloPlayLog = context.Solo_Play_Logs.Where(soloPlayLog => soloPlayLog.Stage_Id == stageID && soloPlayLog.User_Id == userID).First();

            if(soloPlayLog == null)
            {
                SoloPlayLog solo = new SoloPlayLog();
                solo.Stage_Id = stageID;
                solo.User_Id = userID;
                solo.Car_Type_Id = 1;               // とりあえずスタンダード固定(1)
                solo.Clear_Time_Msec = time;
                solo.Created_at = DateTime.Now;
                solo.Updated_at = DateTime.Now;
                context.Solo_Play_Logs.Add(solo);   // ソロプレイ情報の追加
                await context.SaveChangesAsync();   // テーブルに保存

                Console.WriteLine("タイム初登録完了");

                return true;
            }
            else
            {
                if(soloPlayLog.Clear_Time_Msec > time)
                {
                    // ベストタイム更新処理
                    soloPlayLog.Clear_Time_Msec = time;     // タイム更新
                    soloPlayLog.Updated_at = DateTime.Now;
                    await context.SaveChangesAsync();       // テーブルに保存
                    Console.WriteLine("タイム更新完了");

                    return true;
                }
                else
                {   // 更新不要
                    Console.WriteLine("更新不要");

                    return false;
                }
            }
        }

        /// <summary>
        /// ランキング取得
        /// </summary>
        /// <param name="stageID">ステージID</param>
        /// <returns>ランキング情報</returns>
        public async UnaryResult<List<RankingData>> GetRankingAsync(int stageID)
        {
            using var context = new GameDbContext();

            // ランキング情報の取得
            SoloPlayLog[] ranking = context.Solo_Play_Logs.Where(ranking => ranking.Stage_Id == stageID)
                .OrderBy(ranking => ranking.Clear_Time_Msec).Take(10).ToArray();

            // ユーザーIDの抽出
            List<int> userIds = new List<int>();
            foreach (SoloPlayLog soloPlayLog in ranking)
            {   
                userIds.Add(soloPlayLog.User_Id);
            }

            // ユーザー情報の取得
            User[] users = context.Users.Where(users => userIds.Contains(users.Id)).ToArray();

            // 送信用ランキング情報に整形
            List<RankingData> rankingDatas = new List<RankingData>();
            foreach (SoloPlayLog playLog in ranking)
            {
                RankingData rankingData = new RankingData();
                rankingData.UserId = playLog.Id;                // ユーザーID格納

                // ユーザー名格納
                foreach (var user in users)
                {
                    if(playLog.User_Id == user.Id)
                    {
                        rankingData.UserName = user.Name;
                    }
                }

                rankingData.ClearTime = playLog.Clear_Time_Msec;    // クリアタイム格納

                rankingDatas.Add(rankingData);
            }

            return rankingDatas;
        }
    }
}
