using MagicOnion;
using System;
using System.Threading.Tasks;

namespace Shared.Interfaces.StreamingHubs
{
    public interface IRoomHub:IStreamingHub<IRoomHub,IRoomHubReceiver>
    {
        // [ここにクライアント側からサーバー側を呼び出す関数を定義する]

        /// <summary>
        /// ロビー入室処理
        /// </summary>
        /// <param name="UserId">参加するUserID</param>
        /// <returns></returns>
        Task JoinLobbyAsync(int userId);

        /// <summary>
        /// ユーザー入室処理 [retuns : 参加者情報]
        /// </summary>
        /// <param name="roomName">参加するroom名</param>
        /// <param name="UserId">  参加するUserID</param>
        /// <returns></returns>
        Task<JoinedUser[]> JoinAsync(string roomName, int userId);

        /// <summary>
        /// ユーザー退出処理
        /// </summary>
        /// <returns></returns>
        Task ExitAsync();

        /// <summary>
        /// 位置同期処理
        /// </summary>
        /// <returns></returns>
        Task MoveAsync(MoveData moveData);

        /// <summary>
        /// ゲームスタート処理
        /// </summary>
        /// <returns></returns>
        Task GameStartAsync();

        /// <summary>
        /// ゲーム終了処理
        /// </summary>
        /// <returns></returns>
        Task GameEndAsync();

        /// <summary>
        /// プレイヤー撃破処理
        /// </summary>
        /// <returns></returns>
        Task CrushingPlayerAsync(string attackName,string cruchName,Guid crushID);

        /// <summary>
        /// 残タイム同期処理
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        Task TimeCountAsync(int time);
    }
}
