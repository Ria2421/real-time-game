using MagicOnion;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Interfaces.Services
{
    public interface IMyFirstService:IService<IMyFirstService>
    {
        // [ここにどのようなAPIを作るのか、関数形式で定義を作成する]

        /// <summary>
        /// 「足し算API」 二つの整数を引数で受け取り合計値を返す。
        /// return:x,yの合計値
        /// </summary>
        /// <param name="x">足す数</param>
        /// <param name="y">足される数</param>
        /// <returns>x,yの合計値</returns>
        UnaryResult<int> SumAsync(int x, int y);

        // 「引き算API」 二つの整数を引数で受け取り減算値を返す
        UnaryResult<int> SubAsync(int x, int y);

        // 足し算API」 int[]を引数で受け取り合計値を返す
        UnaryResult<int> SumAllAsync(int[] numList);

        // [0] x+y , [1] x-y , [2] x*y , [3] x/y の配列を返す
        UnaryResult<int[]> CalcForOperationAsync(int x, int y);

        // xとyの小数をフィールドに持つNumberクラスを渡して、x + yの結果を返す
        UnaryResult<float> SumAllNumberAsync (Number numArray);

        [MessagePackObject]
        public class Number
        {
            [Key(0)]
            public float x;
            [Key(1)]
            public float y;
        }
    }
}
