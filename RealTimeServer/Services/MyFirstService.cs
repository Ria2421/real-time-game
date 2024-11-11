using MagicOnion;
using MagicOnion.Server;
using Shared.Interfaces.Services;

namespace RealTimeServer.Services
{
    public class MyFirstService:ServiceBase<IMyFirstService>, IMyFirstService
    {
        // 「足し算API」二つの整数を引数で受け取り合計値を返す
        public async UnaryResult<int> SumAsync(int x,int y)
        {
            Console.WriteLine("Received:" + x + y);
            return x + y;
        }

        // 「引き算API」二つの整数を引数で受け取り減算値を返す
        public async UnaryResult<int> SubAsync(int x, int y)
        {
            Console.WriteLine("Received:" + (x - y));
            return x - y;
        }
    }
}
