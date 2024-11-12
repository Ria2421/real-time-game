using MagicOnion;
using RealTimeServer.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Interfaces.Services
{
    public interface IUserService:IService<IUserService>
    {
        /// <summary>
        /// ユーザー登録処理
        /// [return : ユーザーID]
        /// </summary>
        /// <param name="name">ユーザー名</param>
        /// <returns></returns>
        UnaryResult<int> RegistUserAsync(string name);

        /// <summary>
        /// ユーザーをID指定で検索
        /// [return : ユーザー情報]
        /// </summary>
        /// <param name="id">ユーザーID</param>
        /// <returns></returns>
        UnaryResult<User> SearchUserID(int id);

        /// <summary>
        /// ユーザー一覧を取得
        /// [return : ユーザー一覧情報]
        /// </summary>
        /// <returns></returns>
        UnaryResult<User[]> GetAllUser();

        /// <summary>
        /// 指定IDのユーザー名更新
        /// [return : 真偽]
        /// </summary>
        /// <param name="id">  ユーザーID</param>
        /// <param name="name">ユーザー名</param>
        /// <returns></returns>
        UnaryResult<bool> UpdateUserName(int id, string name);
    }
}
