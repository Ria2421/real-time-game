﻿using MagicOnion;
using System;
using System.Collections.Generic;

namespace Shared.Interfaces.StreamingHubs
{
    public interface IRoomHubReceiver
    {
        // [ここにサーバー側からクライアント側を呼び出す関数を定義する]

        // マッチング完了通知
        void OnMatching(string roomName);

        // ユーザーの入室通知
        void OnJoin(JoinedUser user);

        // ユーザーの退出通知
        void OnExit(JoinedUser user);

        // ユーザーの移動通知
        void OnMove(MoveData moveData);

        // インゲーム通知
        void OnInGame();

        // ゲーム開始通知
        void OnStartGame();

        // ゲーム終了通知
        void OnEndGame(Dictionary<int, string> result);

        // 撃破情報通知
        void OnCrushing(string attackName, string cruchName, Guid crushID);
    }
}
