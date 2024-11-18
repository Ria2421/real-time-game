using MagicOnion.Server.Hubs;
using MagicOnionServer.Model.Context;
using RealTimeServer.StremingHubs;
using Shared.Interfaces.StreamingHubs;

namespace StreamingHubs;

public class RoomHub:StreamingHubBase<IRoomHub,IRoomHubReceiver>,IRoomHub
{
    private IGroup room;    // ルーム情報

    /// <summary>
    /// ユーザー入室処理 [retuns : 参加者情報]
    /// </summary>
    /// <param name="roomName">参加するroom名</param>
    /// <param name="UserId">  参加するUserID</param>
    /// <returns></returns>
    public async Task<JoinedUser[]> JoinAsync(string roomName, int UserId)
    {
        // ルームに参加 & ルームを保持
        this.room = await this.Group.AddAsync(roomName);
        
        // DBからユーザー情報を取得
        GameDbContext context = new GameDbContext();
        var user = context.Users.Where(user => user.Id == UserId).First();

        // グループストレージにユーザーデータを格納
        var roomStrage = this.room.GetInMemoryStorage<RoomData>();      // ルームには参加者全員が参照可能な共有の保存領域がある(メモリ)
        var joinedUser = new JoinedUser() { ConnectionId = this.ConnectionId, UserData = user,JoinOrder = roomStrage.AllValues.ToArray().Length + 1 };
        var roomData = new RoomData() { JoinedUser = joinedUser };
        roomStrage.Set(this.ConnectionId, roomData);                    // 接続IDをキーにデータを格納

        // ルーム参加者全員に、ユーザーの入室通知を送信
        this.BroadcastExceptSelf(room).OnJoin(joinedUser);  // 自分以外の参加者のOnJoinを呼び出す。※BroadCast()は自分を含む
        RoomData[] roomDataList = roomStrage.AllValues.ToArray<RoomData>();

        // 参加中のユーザー情報を渡す
        JoinedUser[] joinedUserList = new JoinedUser[roomDataList.Length];
        for (int i = 0; i < roomDataList.Length; i++)
        {
            joinedUserList[i] = roomDataList[i].JoinedUser;
        }
        return joinedUserList;
    }
}
