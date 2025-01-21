using MagicOnion;
using MagicOnion.Server;
using MagicOnionServer.Model.Context;
using RealTimeServer.Model.Entity;
using Shared.Interfaces.Services;
using Shared.Model.Entity;
using System.Diagnostics;
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
        public async UnaryResult<RegistResult> RegistClearTimeAsync(int stageID, int userID,int time,string ghostData)
        {
            using var context = new GameDbContext();
            RegistResult registResult = new RegistResult();

            //================================
            // クリアタイム登録・更新処理

            // ランキング一位のプレイデータを取得
            var topData = context.Solo_Play_Data.Where(ranking => ranking.Stage_Id == stageID)
                                                         .OrderBy(ranking => ranking.Clear_Time_Msec).FirstOrDefault();

            // 該当データが無いか検索
            var soloPlayLog = context.Solo_Play_Data.Where(soloPlayLog => soloPlayLog.Stage_Id == stageID && soloPlayLog.User_Id == userID).FirstOrDefault();

            if(soloPlayLog == null)
            {   // 初回登録
                SoloPlayData solo = new SoloPlayData();
                solo.Stage_Id = stageID;
                solo.User_Id = userID;
                solo.Car_Type_Id = 1;               // とりあえずスタンダード固定(1)
                solo.Clear_Time_Msec = time;
                solo.Created_at = DateTime.Now;
                solo.Updated_at = DateTime.Now;

                // 自分の一番早いゴーストデータを表示できる仕様にするときは条件なくゴーストデータを登録する
                // ゴーストデータ登録処理
                if (topData.Clear_Time_Msec > time)
                {
                    solo.Ghost_Data = ghostData;
                    registResult.ghostRegistFlag = true;
                    Console.WriteLine("ゴーストデータ登録");
                }
                else
                {   
                    solo.Ghost_Data = "";
                    registResult.ghostRegistFlag = false;
                    Console.WriteLine("ゴーストデータ未登録");
                }

                context.Solo_Play_Data.Add(solo);   // ソロプレイ情報の追加
                Console.WriteLine("タイム初登録");

                registResult.timeRegistFlag = true;
            }
            else
            {
                if(soloPlayLog.Clear_Time_Msec > time)
                {
                    // ベストタイム更新処理
                    soloPlayLog.Clear_Time_Msec = time;     // タイム更新
                    soloPlayLog.Updated_at = DateTime.Now;
                    Console.WriteLine("タイム更新");

                    // ゴーストデータ登録処理
                    if (topData.Clear_Time_Msec > time)
                    {
                        soloPlayLog.Ghost_Data = ghostData;
                        registResult.ghostRegistFlag = true;
                        Console.WriteLine("ゴーストデータ登録");
                    }
                    else
                    {
                        soloPlayLog.Ghost_Data = "";                // 自分の一番早いゴーストデータを表示できる仕様にするときは""にしない
                        registResult.ghostRegistFlag = false;
                        Console.WriteLine("ゴーストデータ未登録");
                    }

                    registResult.timeRegistFlag = true;
                }
                else
                {   // 更新不要
                    Console.WriteLine("更新不要");

                    registResult.timeRegistFlag = false;
                }
            }

            await context.SaveChangesAsync();   // テーブルに保存
            return registResult;
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
            SoloPlayData[] ranking = context.Solo_Play_Data.Where(ranking => ranking.Stage_Id == stageID)
                .OrderBy(ranking => ranking.Clear_Time_Msec).Take(10).ToArray();

            // ユーザーIDの抽出
            List<int> userIds = new List<int>();
            foreach (SoloPlayData soloPlayLog in ranking)
            {   
                userIds.Add(soloPlayLog.User_Id);
            }

            // ユーザー情報の取得
            User[] users = context.Users.Where(users => userIds.Contains(users.Id)).ToArray();

            // 送信用ランキング情報に整形
            List<RankingData> rankingDatas = new List<RankingData>();
            foreach (SoloPlayData playData in ranking)
            {
                RankingData rankingData = new RankingData();
                rankingData.Id = playData.Id;                    // ログID格納
                rankingData.UserId = playData.Id;                // ユーザーID格納

                // ユーザー名格納
                foreach (var user in users)
                {
                    if(playData.User_Id == user.Id)
                    {
                        rankingData.UserName = user.Name;
                    }
                }

                rankingData.ClearTime = playData.Clear_Time_Msec;// クリアタイム格納
                rankingData.GhostData = playData.Ghost_Data;     // ゴーストデータ格納

                rankingDatas.Add(rankingData);
            }

            return rankingDatas;
        }
    }
}
