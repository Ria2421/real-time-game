﻿using MagicOnion.Server.Hubs;
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

    /// <summary>
    /// プレイヤー最大数
    /// </summary>
    private const int MAX_PLAYER = 4;

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
            joinedUser = new JoinedUser() { ConnectionId = this.ConnectionId, UserData = user, JoinOrder = 1, GameState = 1 };
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
            joinedUser = new JoinedUser() { ConnectionId = this.ConnectionId, UserData = user, JoinOrder = result.Min(), GameState = 1 };
        }
        
        var roomData = new RoomData() { JoinedUser = joinedUser, GameState = 0 };
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

        // 4人揃ったら開始の合図を送る
        if(roomDataList.Length == MAX_PLAYER)
        {
            Console.WriteLine("全員揃いました");
            this.Broadcast(room).OnInGame();    // inGame通知処理
        }

        return joinedUserList;
    }

    /// <summary>
    /// ユーザー退出処理
    /// </summary>
    /// <returns></returns>
    public async Task ExitAsync()
    {
        // ルーム参加者全員に、ユーザーの退出通知を送信
        var roomStrage = this.room.GetInMemoryStorage<RoomData>();
        var roomData = roomStrage.Get(this.ConnectionId);

        if (roomData == null) return;   // ユーザーデータが存在するかnullチェック
        
        this.BroadcastExceptSelf(room).OnExit(roomData.JoinedUser);

        // グループデータから削除
        this.room.GetInMemoryStorage<RoomData>().Remove(this.ConnectionId);

        // ルーム内のメンバーから自分を削除
        await room.RemoveAsync(this.Context);
    }

    /// <summary>
    /// ゲーム開始処理
    /// </summary>
    /// <returns></returns>
    public async Task GameStartAsync()
    {
        // 該当するルームデータを取得
        var roomStrage = this.room.GetInMemoryStorage<RoomData>();
        var roomData = roomStrage.Get(this.ConnectionId);

        // GameStateの変更
        roomData.GameState = 2;

        // 全員がスタート状態か調べる
        int startCnt = 0;
        RoomData[] roomDataList = roomStrage.AllValues.ToArray<RoomData>();
        foreach(RoomData data in roomDataList)
        {
            if (data.GameState == 2) { startCnt++; }
        }

        if (startCnt == MAX_PLAYER)
        {   // 揃っている場合はゲーム開始通知
            Console.WriteLine("ゲームスタート");
            this.Broadcast(room).OnStartGame();
        }
    }

    /// <summary>
    /// ゲーム終了処理
    /// </summary>
    /// <returns></returns>
    public async Task GameEndAsync()
    {
        // 該当するルームデータを取得
        var roomStrage = this.room.GetInMemoryStorage<RoomData>();
        var roomData = roomStrage.Get(this.ConnectionId);

        // ゲーム終了通知 (勝者のPLNoと名前を渡す)
        Console.WriteLine("ゲーム終了");
        this.Broadcast(room).OnEndGame(roomData.JoinedUser.JoinOrder,roomData.JoinedUser.UserData.Name);
    }

    /// <summary>
    /// 切断時の退室処理
    /// </summary>
    /// <returns></returns>
    protected override ValueTask OnDisconnected()
    {
        // ルームデータを削除
        this.room.GetInMemoryStorage<RoomData>().Remove(this.ConnectionId);
        // ルーム内のメンバーから削除
        room.RemoveAsync(this.Context);
        return CompletedTask;
    }

    /// <summary>
    /// 位置同期処理
    /// </summary>
    /// <returns></returns>
    public async Task MoveAsync(MoveData moveData)
    {
        // 呼び出した接続IDのルームデータに動作情報を保存
        var roomStrage = this.room.GetInMemoryStorage<RoomData>();
        var roomData = roomStrage.Get(this.ConnectionId);
        roomData.MoveData = moveData;

        // ルーム参加者全員に、ユーザーの移動情報を送信
        this.BroadcastExceptSelf(room).OnMove(moveData);
    }
}