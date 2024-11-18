using MagicOnion;
using System.Threading.Tasks;

namespace Shared.Interfaces.StreamingHubs
{
    public interface IRoomHub:IStreamingHub<IRoomHub,IRoomHubReceiver>
    {
        // [ここにクライアント側からサーバー側を呼び出す関数を定義する]

        /// <summary>
        /// ユーザー入室処理 [retuns : 参加者情報]
        /// </summary>
        /// <param name="roomName">参加するroom名</param>
        /// <param name="UserId">  参加するUserID</param>
        /// <returns></returns>
        Task<JoinedUser[]> JoinAsync(string roomName, int UserId);
    }
}
