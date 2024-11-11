using MagicOnion;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Interfaces.Services
{
    public interface IMyFirstService:IService<IMyFirstService>
    {
        // [ここにどのようなAPIを作るのか、関数形式で定義を作成する]

        // 「足し算API」 二つの整数を引数で受け取り合計値を返す
        UnaryResult<int> SumAsync(int x, int y);

        // 「引き算API」 二つの整数を引数で受け取り減算値を返す
        UnaryResult<int> SubAsync(int x, int y);
    }
}
