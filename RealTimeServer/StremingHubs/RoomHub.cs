using MagicOnion.Server.Hubs;
using MagicOnionServer.Model.Context;
using RealTimeServer.Model.Entity;
using RealTimeServer.StremingHubs;
using Shared.Interfaces.StreamingHubs;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    /// <summary>
    /// ロビー名
    /// </summary>
    private const string LOBBY_NAME = "Lobby";

    //------------------------------------------------------
    // メソッド

    /// <summary>
    /// ユーザー入室処理 [retuns : 参加者情報]
    /// </summary>
    /// <param name="roomName">参加するroom名</param>
    /// <param name="UserId">  参加するUserID</param>
    /// <returns></returns>
    public async Task JoinLobbyAsync(int userId)
    {
        JoinedUser[] joinedUserList = await JoinAsync(LOBBY_NAME, userId);   // ロビーに参加

        Console.WriteLine(joinedUserList.Length + "人目参加");

        // マッチングが完了していたらクライアント側のマッチング処理を呼び出す
        if(joinedUserList.Length == MAX_PLAYER)
        {   // とりあえず人数揃い次第マッチング完了
            Random rand = new Random();
            int stageID = rand.Next(1, 4);
            Console.WriteLine("マッチング完了");
            this.Broadcast(room).OnMatching(Guid.NewGuid().ToString("N"),stageID);
        }
    }

    /// <summary>
    /// ユーザー入室処理 [retuns : 参加者情報]
    /// </summary>
    /// <param name="roomName">参加するroom名</param>
    /// <param name="userId">  参加するUserID</param>
    /// <returns></returns>
    public async Task<JoinedUser[]> JoinAsync(string roomName, int userId)
    {
        // ルームに参加 & ルームを保持
        this.room = await this.Group.AddAsync(roomName);
        
        // DBからユーザー情報を取得
        GameDbContext context = new GameDbContext();
        var user = context.Users.Where(user => user.Id == userId).First();

        // グループストレージにユーザーデータを格納
        var roomStrage = this.room.GetInMemoryStorage<RoomData>();      // ルームには参加者全員が参照可能な共有の保存領域がある(メモリ)

        lock (roomStrage)
        {
            // ユーザーデータの作成・追加
            var nowRoomDataList = roomStrage.AllValues.ToArray<RoomData>();
            JoinedUser joinedUser;

            if (nowRoomDataList.Length == 0)
            {   // 誰もいない時
                joinedUser = new JoinedUser() { ConnectionId = this.ConnectionId, UserData = user, JoinOrder = 1, GameState = 1,Ranking = 1 };
            }
            else
            {
                // 存在するプレイヤーNoを取得
                int[] numbers = new int[nowRoomDataList.Length];
                for (int i = 0; i < nowRoomDataList.Length; i++)
                {
                    numbers[i] = nowRoomDataList[i].JoinedUser.JoinOrder;
                }

                // 空いているPLNoを取得
                IEnumerable<int> result = noList.Except(numbers);

                // 最小値のPLNoを適用したユーザーデータを作成
                joinedUser = new JoinedUser() { ConnectionId = this.ConnectionId, UserData = user, JoinOrder = result.Min(), GameState = 1,Ranking = 1 };
            }

            var roomData = new RoomData() { JoinedUser = joinedUser, GameState = 0 };
            roomStrage.Set(this.ConnectionId, roomData);  // 接続IDをキーにデータを格納

            // ルーム参加者全員に、ユーザーの入室通知を送信
            if(roomName != LOBBY_NAME) 
            {   // マッチング時は送らない
                this.BroadcastExceptSelf(room).OnJoin(joinedUser);  // 自分以外の参加者のOnJoinを呼び出す。※BroadCast()は自分を含む
                Console.WriteLine(userId + "参加通知");
            }
            
            RoomData[] roomDataList = roomStrage.AllValues.ToArray<RoomData>();

            // 参加中のユーザー情報を渡す
            JoinedUser[] joinedUserList = new JoinedUser[roomDataList.Length];
            for (int i = 0; i < roomDataList.Length; i++)
            {
                joinedUserList[i] = roomDataList[i].JoinedUser;
            }

            // 4人揃ったら開始の合図を送る
            if (roomDataList.Length == MAX_PLAYER && roomName != LOBBY_NAME)
            {
                Console.WriteLine("ゲームカウント開始 (ルーム名:" + roomName + ")");
                this.Broadcast(room).OnInGame();    // inGame通知処理
            }

            return joinedUserList;
        }
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

        // lockを使った排他制御 (room参加者全員のデータを参照するときに使用)
        // 処理が並行して同時に行われることで不具合が発生するのを防ぐ。
        lock (roomStrage)   
        {
            var roomData = roomStrage.Get(this.ConnectionId);

            // GameStateの変更
            roomData.GameState = 2;

            // 全員がスタート状態か調べる
            int startCnt = 0;
            RoomData[] roomDataList = roomStrage.AllValues.ToArray<RoomData>();
            foreach (RoomData data in roomDataList)
            {
                if (data.GameState == 2) { startCnt++; }
            }

            if (startCnt == MAX_PLAYER)
            {   // 揃っている場合はゲーム開始通知
                Console.WriteLine("ゲームスタート");
                this.Broadcast(room).OnStartGame();
            }
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

        // ゲーム終了通知 (順位と名前を渡す)
        Console.WriteLine("ゲーム終了\n\n");
    }

    /// <summary>
    /// 切断時の退室処理
    /// </summary>
    /// <returns></returns>
    protected override async ValueTask OnDisconnected()
    {
        await ExitAsync();
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

    /// <summary>
    /// 撃破処理
    /// </summary>
    /// <param name="attackName">撃破した人のPL名</param>
    /// <param name="cruchName"> 撃破された人のPL名</param>
    /// <param name="crushID">   撃破された人の接続ID</param>
    /// <returns></returns>
    public async Task CrushingPlayerAsync(string attackName, string cruchName, Guid crushID, int deadNo)
    {
        // 撃破された人のデータを取得する
        var roomStrage = this.room.GetInMemoryStorage<RoomData>();
        var roomData = roomStrage.Get(crushID);

        // 全員に撃破情報通知を送る
        this.Broadcast(room).OnCrushing(attackName, cruchName, crushID, deadNo);

        // 順位の計算
        RoomData[] roomDataList = roomStrage.AllValues.ToArray<RoomData>();
        int rankCnt = 0;
        foreach(RoomData data in roomDataList)
        {
            if(data.JoinedUser.Ranking == 1) rankCnt++;
        }

        roomData.JoinedUser.Ranking = rankCnt;  // 順位の保存

        Console.WriteLine(cruchName + ":" + rankCnt + "位");

        if (rankCnt == 2)
        {
            using var context = new GameDbContext();
            int[] rates = new int[MAX_PLAYER];

            foreach(RoomData data in roomDataList)
            {   // ランク順にレートを取得
                switch(data.JoinedUser.Ranking)
                {
                    case 1:
                        rates[0] = data.JoinedUser.UserData.Rate;
                        break;

                    case 2:
                        rates[1] = data.JoinedUser.UserData.Rate;
                        break;

                    case 3:
                        rates[2] = data.JoinedUser.UserData.Rate;
                        break;

                    case 4:
                        rates[3] = data.JoinedUser.UserData.Rate;
                        break;

                    default:
                        break;
                }
            }

            // リザルトデータの作成
            List<ResultData> rank = new List<ResultData>();
            foreach(var data2 in roomDataList)
            {
                ResultData resultData = new ResultData() { UserId = data2.JoinedUser.UserData.Id, Rank = data2.JoinedUser.Ranking, Rate = data2.JoinedUser.UserData.Rate };

                // 増減レートポイントの反映
                switch (data2.JoinedUser.Ranking)
                {
                    case 1:
                        data2.JoinedUser.UserData.Rate += CalcRate(rates[0], rates[3]);
                        resultData.ChangeRate = CalcRate(rates[0], rates[3]);
                        break;

                    case 2:
                        data2.JoinedUser.UserData.Rate += CalcRate(rates[1], rates[3]) / 2;
                        resultData.ChangeRate = CalcRate(rates[1], rates[3]) / 2;
                        break;

                    case 3:
                        data2.JoinedUser.UserData.Rate -= CalcRate(rates[0], rates[2]) / 2;

                        if(data2.JoinedUser.UserData.Rate <= 0)
                        {
                            data2.JoinedUser.UserData.Rate = 0;
                        }

                        resultData.ChangeRate = -CalcRate(rates[0], rates[2]) / 2;
                        break;

                    case 4:
                        data2.JoinedUser.UserData.Rate -= CalcRate(rates[0], rates[3]);

                        if (data2.JoinedUser.UserData.Rate <= 0)
                        {
                            data2.JoinedUser.UserData.Rate = 0;
                        }

                        resultData.ChangeRate = -CalcRate(rates[0], rates[3]);
                        break;

                    default:
                        break;
                }

                // ユーザー情報の更新
                User user = context.Users.Where(user => user.Id == data2.JoinedUser.UserData.Id).First();
                user.Rate = data2.JoinedUser.UserData.Rate;
                user.Updated_at = DateTime.Now;
                await context.SaveChangesAsync();   // テーブルに保存

                rank.Add(resultData);
            }

            Console.WriteLine("ゲーム終了");
            this.Broadcast(room).OnEndGame(rank);
        }
    }

    /// <summary>
    /// 残タイム共有処理
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public async Task TimeCountAsync(int time)
    {
        // 全員に残り時間を通知
        this.Broadcast(room).OnTimeCount(time);

        if(time <= 0)
        {   // タイムアップ通知 (引き分け表示)
            this.Broadcast(room).OnTimeUp();
        }
    }

    /// <summary>
    /// 大砲発射処理
    /// </summary>
    /// <returns></returns>
    public async Task ShotCannonAsync()
    {
        Random rand = new Random();
        int cannonID = rand.Next(1, 5);

        // 発射通知 (発射する大砲ID)
        this.Broadcast(room).OnShot(cannonID);
    }

    /// <summary>
    /// レート計算処理
    /// </summary>
    /// <param name="winRate"> 勝者レート</param>
    /// <param name="loseRate">敗者レート</param>
    /// <returns></returns>
    private int CalcRate(int winRate,int loseRate)
    {
        int result = (int)(50.0f / (MathF.Pow(10, ((float)(winRate - loseRate) / 400.0f)) + 1.0f));

        if (result <= 2) result = 2;

        return result;
    }
}
