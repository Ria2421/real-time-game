using Shared.Interfaces.StreamingHubs;

namespace RealTimeServer.StremingHubs
{
    // ルーム内に保存するデータクラス (全ユーザーで共有するデータをここに保存する)
    public class RoomData
    {
        public JoinedUser JoinedUser {  get; set; }
    }
}
