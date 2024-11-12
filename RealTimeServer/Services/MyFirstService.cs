using MagicOnion;
using MagicOnion.Server;
using Shared.Interfaces.Services;

namespace RealTimeServer.Services
{
    public class MyFirstService:ServiceBase<IMyFirstService>, IMyFirstService
    {
        /// <summary>
        /// 「足し算API」 二つの整数を引数で受け取り合計値を返す。
        /// return:x,yの合計値
        /// </summary>
        /// <param name="x">足す数</param>
        /// <param name="y">足される数</param>
        /// <returns>x,yの合計値</returns>
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

        // 「足し算API」int配列を引数で受け取り合計値を返す
        public async UnaryResult<int> SumAllAsync(int[] numList)
        {
            int result = 0;
            for (int i= 0; i < numList.Length; i++)
            {
                result += numList[i];
            }

            return result;
        }

        // [0] x+y , [1] x-y , [2] x*y , [3] x/y の配列を返す
        public async UnaryResult<int[]> CalcForOperationAsync(int x,int y)
        {
            int[] result = new int[4];
            result[0] = x + y;
            result[1] = x - y;
            result[2] = x * y;
            result[3] = x / y;
            return result;
        }

        // xとyの小数をフィールドに持つNumberクラスを渡して、x + yの結果を返す
        public async UnaryResult<float> SumAllNumberAsync(IMyFirstService.Number numArray)
        {
            float result = numArray.x + numArray.y;

            return result;
        }
    }
}
