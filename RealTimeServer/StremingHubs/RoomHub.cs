using MagicOnion.Server.Hubs;
using MagicOnionServer.Model.Context;
using RealTimeServer.StremingHubs;
using Shared.Interfaces.StreamingHubs;

namespace StreamingHubs;

public class RoomHub:StreamingHubBase<IRoomHub,IRoomHubReceiver>,IRoomHub
{
    //------------------------------------------------------
    // フィールド

    /// <summary>
    /// ルーム情報
    /// </summary>
    private IGroup room;

    /// <summary>
    /// プレイヤーNoリスト
    /// </summary>
    private int[] noList = { 1, 2, 3, 4 };

    //------------------------------------------------------
    // メソッド

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

        // ユーザーデータの作成・追加
        var nowRoomDataList = roomStrage.AllValues.ToArray<RoomData>();
        JoinedUser joinedUser;

        if (nowRoomDataList.Length == 0)
        {   // 誰もいない時
            joinedUser = new JoinedUser() { ConnectionId = this.ConnectionId, UserData = user, JoinOrder = 1 };
        }
        else
        {
            // 存在するプレイヤーNoを取得
            int[] numbers = new int[nowRoomDataList.Length];
            for(int i = 0;i < nowRoomDataList.Length;i++)
            {
                numbers[i] = nowRoomDataList[i].JoinedUser.JoinOrder;
            }

            // 空いているPLNoを取得
            IEnumerable<int> result = noList.Except(numbers);

            // 最小値のPLNoを適用したユーザーデータを作成
            joinedUser = new JoinedUser() { ConnectionId = this.ConnectionId, UserData = user, JoinOrder = result.Min() };
        }
        
        var roomData = new RoomData() { JoinedUser = joinedUser };
        roomStrage.Set(this.ConnectionId, roomData);  // 接続IDをキーにデータを格納

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

    /// <summary>
    /// ユーザー退出処理
    /// </summary>
    /// <returns></returns>
    public async Task ExitAsync()
    {
        // グループデータから削除
        this.room.GetInMemoryStorage<RoomData>().Remove(this.ConnectionId);

        // ルーム内のメンバーから自分を削除
        await room.RemoveAsync(this.Context);

        // ルーム参加者全員に、ユーザーの退出通知を送信
        this.BroadcastExceptSelf(room).OnExit(this.ConnectionId);
    }

    /// <summary>
    /// 位置同期処理
    /// </summary>
    /// <returns></returns>
    public async Task MoveAsync(MoveData moveData)
    {
        // ルーム参加者全員に、ユーザーの移動情報を送信
        this.BroadcastExceptSelf(room).OnMove(moveData);
    }
}
