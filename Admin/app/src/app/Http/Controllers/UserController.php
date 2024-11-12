<?php
//-------------------------------------------------
// ユーザーコントローラー [UserController.php]
// Author:Kenta Nakamoto
// Data:2024/06/18
//-------------------------------------------------

namespace App\Http\Controllers;

use App\Models\FollowLogs;
use App\Models\HaveItem;
use App\Models\ItemLogs;
use App\Models\User;
use Illuminate\Http\Request;

class UserController extends Controller
{
    // ユーザー一覧を表示する
    public function index(Request $request)
    {
        // アイテムをページネーションに対応した形式で取得
        $data = User::paginate(10);
        // ページネーションの表示幅を設定
        $data->onEachSide(2);

        // 取得データをviewに渡して表示
        return view('users.index', ['users' => $data]);
    }

    // 所持アイテム一覧の表示
    public function showItem(Request $request)
    {
        // 指定されたIDのデータを取得
        $user = User::find($request->user_id);
        if (!empty($user)) {
            $items = $user->items()->paginate(10);
            $items->appends(['id' => $request->user_id]);
        }

        return view('users/haveItems', ['user' => $user, 'items' => $items ?? null]);
    }

    // 指定IDのフォロー情報を取得する処理
    public function showFollow(Request $request)
    {
        // 指定ユーザーIDのユーザーデータを取得
        $user = User::find($request->user_id);

        if (!empty($user)) {
            // 指定されたIDのユーザーデータが存在した時

            // フォロー・フォロワーのユーザーデータを取得
            $follows = $user->follows()->get();
            $followers = $user->followers()->get();

            // フォロー・フォロワーのID情報をピックアップ
            $follows_id = $follows->pluck('id')->toArray();
            $followers_id = $followers->pluck('id')->toArray();
            // 上記の2つのIDで一致するもののみ取得
            $mutualUsersID = array_intersect($follows_id, $followers_id);
            // 一致したIDのユーザーデータを取得
            $mutualUsers = User::whereIn('id', $mutualUsersID)->get();
        }

        // フォロー・フォロワー・相互フォロワーのユーザーデータを渡してviewを表示
        return view('users.showFollows',
            ['follows' => $follows ?? null, 'followers' => $followers ?? null, 'mutualUsers' => $mutualUsers ?? null]);
    }

    // 指定IDのフォロー操作のログを取得
    public function showFollowLogs(Request $request)
    {
        // 指定ユーザーIDが存在するかチェック
        $user = User::find($request->user_id);

        if (!empty($user)) {
            // 指定されたIDのユーザーデータが存在した時

            // 指定ユーザーIDのレコードを取得
            $followLogs = FollowLogs::where('user_id', $request->user_id)->get();
        }

        // 取得したログを渡してviewを表示
        return view('users.showFollowLogs', ['followLogs' => $followLogs ?? null]);
    }

    // 指定IDのアイテム操作のログを取得
    public function showItemLogs(Request $request)
    {
        // 指定ユーザーIDが存在するかチェック
        $user = User::find($request->user_id);

        if (!empty($user)) {
            // 指定されたIDのユーザーデータが存在した時

            // 指定ユーザーIDのレコードを取得
            $itemLogs = ItemLogs::where('user_id', $request->user_id)->get();
        }

        // 取得したログを渡してviewを表示
        return view('users.showItemLogs', ['itemLogs' => $itemLogs ?? null]);
    }
}
