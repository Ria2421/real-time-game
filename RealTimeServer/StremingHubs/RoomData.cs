using Shared.Interfaces.StreamingHubs;

namespace RealTimeServer.StremingHubs
{
    // ルーム内に保存するデータクラス (全ユーザーで共有するデータをここに保存する)
    public class RoomData
    {
        /// <summary>
        /// 参加情報
        /// </summary>
        public JoinedUser JoinedUser {  get; set; }

        /// <summary>
        /// 動作情報
        /// </summary>
        public MoveData MoveData { get; set; }

        /// <summary>
        /// ゲーム状態 [0.入室 1.準備完了 2.インゲーム]
        /// </summary>
        public int GameState { get; set; }
    }
}
