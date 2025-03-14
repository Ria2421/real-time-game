﻿//---------------------------------------------------------------
// ユーザーデータ操作API [ UserService.cs ]
// Author:Kenta Nakamoto
//---------------------------------------------------------------
using MagicOnion;
using MagicOnion.Server;
using MagicOnionServer.Model.Context;
using Microsoft.EntityFrameworkCore;
using RealTimeServer.Model.Entity;
using Shared.Interfaces.Services;
using Shared.Model.Entity;
using System.Xml.Linq;

namespace RealTimeServer.Services
{
    public class UserService:ServiceBase<IUserService>,IUserService
    {
        /// <summary>
        /// ユーザー登録処理
        /// [return : ユーザーID]
        /// </summary>
        /// <param name="name">ユーザー名</param>
        /// <returns></returns>
        public async UnaryResult<int> RegistUserAsync(string name,string token)
        {
            using var context = new GameDbContext();

            var ngWords = context.Ng_Words.ToArray();   // NGワード取得

            // バリデーションチェック
            if (context.Users.Where(user => user.Name == name).Count() > 0)
            {   // 重複チェック
                // 例外スロー
                throw new ReturnStatusException(Grpc.Core.StatusCode.InvalidArgument, "SameName");
            }

            // NGワードチェック
            foreach(NGWord ng in ngWords)
            {
                if(ng.Word == name)
                {
                    throw new ReturnStatusException(Grpc.Core.StatusCode.InvalidArgument, "NGWord");
                }
            }

            User user = new User();
            user.Name = name;
            user.Token = token;
            user.Skin_No = 1;
            user.Rate = 1500;
            user.Money = 200;
            user.Created_at = DateTime.Now;
            user.Updated_at = DateTime.Now;
            context.Users.Add(user);            // ユーザー情報の追加
            await context.SaveChangesAsync();   // テーブルに保存

            Console.WriteLine(name + " : 登録完了");
            return user.Id;
        }

        /// <summary>
        /// ユーザーをID指定で検索
        /// [return : ユーザー情報]
        /// </summary>
        /// <param name="id">ユーザーID</param>
        /// <returns></returns>
        public async UnaryResult<User> SearchUserID(int id)
        {
            using var context = new GameDbContext();

            var user = context.Users.Where(user => user.Id == id).FirstOrDefault();

            return user;
        }

        /// <summary>
        /// ユーザー一覧を取得
        /// [return : ユーザー一覧情報]
        /// </summary>
        /// <returns></returns>
        public async UnaryResult<User[]> GetAllUser()
        {
            using var context = new GameDbContext();

            User[] users = context.Users.ToArray();

            return users;
        }

        /// <summary>
        /// 指定IDのユーザー名更新
        /// [return : 真偽]
        /// </summary>
        /// <param name="id">  ユーザーID</param>
        /// <param name="name">ユーザー名</param>
        /// <returns></returns>
        public async UnaryResult<bool> UpdateUserName(int id, string name)
        {
            using var context = new GameDbContext();

            var ngWords = context.Ng_Words.ToArray();   // NGワード取得

            // バリデーションチェック
            if (context.Users.Where(user => user.Name == name).Count() > 0)
            {
                // 例外スロー
                throw new ReturnStatusException(Grpc.Core.StatusCode.InvalidArgument, "SameName");
            }

            // NGワードチェック
            foreach (NGWord ng in ngWords)
            {
                if (ng.Word == name)
                {
                    throw new ReturnStatusException(Grpc.Core.StatusCode.InvalidArgument, "NGWord");
                }
            }

            // ユーザー情報の更新
            User user = context.Users.Where(user=>user.Id == id).First();
            user.Name = name;
            user.Updated_at = DateTime.Now;
            await context.SaveChangesAsync();   // テーブルに保存
            return true;
        }
    }
}
