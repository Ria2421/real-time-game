using MagicOnion;
using System;

namespace Shared.Interfaces.StreamingHubs
{
    public interface IRoomHubReceiver
    {
        // [ここにサーバー側からクライアント側を呼び出す関数を定義する]

        // ユーザーの入室通知
        void OnJoin(JoinedUser user);

        // ユーザーの準備完了通知 [引数はそのうちプレイヤーデータに変える(車種やスキン等の情報)]
        void OnReady(JoinedUser user);

        // ユーザーの準備キャンセル通知
        void OnNonReady(JoinedUser user);

        // ユーザーの退出通知
        void OnExit(Guid connectionId);

        // ユーザーの移動通知
        void OnMove(MoveData moveData);
    }
}
