<?php
//-------------------------------------------------
// メールコントローラー [AccountController.php]
// Author:Kenta Nakamoto
// Data:2024/06/27
//-------------------------------------------------

namespace App\Http\Controllers;

use App\Models\Item;
use App\Models\Mail;
use App\Models\MailLogs;
use App\Models\ReceiveMails;
use App\Models\SendItem;
use App\Models\User;
use Illuminate\Http\Request;

class MailController extends Controller
{
    // 定型メール一覧の表示
    public function index()
    {
        if (isset($request->id)) {
            // id指定有
            $data = Mail::where('id', '=', $request->id)->paginate(10);
            $data->onEachSide(2);;

        } else {
            // id指定無
            $data = Mail::paginate(10);
            $data->onEachSide(2);;
        }

        return view('mails/index', ['mails' => $data]);
    }

    // メール添付アイテム一覧の表示
    public function showSendItems()
    {
        // クエリによる情報の取得
        $data = SendItem::select([
            'send_items.id as id',
            'items.name as name',
            'send_items.quantity as quantity',
            'send_items.created_at as created_at',
            'send_items.updated_at as updated_at',
        ])
            ->join('items', function ($join) {
                $join->on('send_items.item_id', '=', 'items.id');
            })->paginate(10);

        $data->onEachSide(2);

        return view('mails.sendItem', ['items' => $data]);
    }

    // ユーザー受信メール一覧の表示
    public function showReceiveMails()
    {
        // クエリによる情報の取得
        $data = ReceiveMails::select([
            'receive_mails.id as id',
            'users.name as user_name',
            'mails.title as mail_title',
            'receive_mails.send_item_id as send_item_id',
            'receive_mails.unsealed_flag as unsealed_flag',
            'receive_mails.created_at as created_at',
            'receive_mails.updated_at as updated_at',
        ])
            ->join('users', function ($join) {
                $join->on('receive_mails.user_id', '=', 'users.id');
            })
            ->join('mails', function ($join) {
                $join->on('receive_mails.mail_id', '=', 'mails.id');
            })
            ->paginate(10);

        $data->onEachSide(2);

        // 取得したデータをviewに渡して表示
        return view('mails.receive', ['mails' => $data]);
    }

    // メール送信フォームの表示
    public function showSendMail()
    {
        // セレクトボックスの表示用のデータを取得
        $mails = Mail::All();
        $items = SendItem::select([
            'send_items.id as id',
            'items.name as name',
            'send_items.quantity as quantity',
        ])
            ->join('items', function ($join) {
                $join->on('send_items.item_id', '=', 'items.id');
            })
            ->get();

        // 取得したデータをviewに渡して表示
        return view('mails.sendMail', ['mails' => $mails, 'items' => $items]);
    }

    // 送信処理
    public function sendMail(Request $request)
    {
        if ($request->user_id == 0) {
            /* 全ユーザー指定の場合 */

            // 全ユーザーデータを取得
            $users = User::All();

            // 送信処理(受信テーブルに登録)
            foreach ($users as $user) {
                ReceiveMails::create([
                    'user_id' => $user->id,
                    'mail_id' => $request->mail_id,
                    'send_item_id' => $request->send_item_id,
                    'unsealed_flag' => false
                ]);

                // 受信ログを生成
                MailLogs::create([
                    'user_id' => $user->id,
                    'mail_id' => $request->mail_id,
                    'send_item_id' => $request->send_item_id,
                    'action' => 1
                ]);
            }

            // 送信IDをsendComp()に渡す
            return redirect()->route('mails.showSendComp', ['id' => 0]);
        } else {
            /* ユーザー指定の場合 */

            // 入力されたユーザーIDが存在するか確認
            $users = User::find($request->user_id);

            if (isset($users)) {
                // 指定IDが存在した時

                // 送信処理(受信テーブルに登録)
                ReceiveMails::create([
                    'user_id' => $request->user_id,
                    'mail_id' => $request->mail_id,
                    'send_item_id' => $request->send_item_id,
                    'unsealed_flag' => false
                ]);

                // 受信ログを生成
                MailLogs::create([
                    'user_id' => $request->user_id,
                    'mail_id' => $request->mail_id,
                    'send_item_id' => $request->send_item_id,
                    'action' => 1
                ]);

                // 送信IDをsendComp()に渡す
                return redirect()->route('mails.showSendComp', ['id' => $request->user_id]);
            } else {
                // エラー表示
                return redirect()->route('mails.showSendMail', ['error' => 'invalid']);
            }
        }
    }

    // 送信完了画面
    public function showSendComp(Request $request)
    {
        // 渡された送信IDごとに名前を取得
        if ($request->id == 0) {
            $name = '全ユーザー';
        } else {
            $data = User::find($request->id);
            $name = $data->name;
        }

        // 取得した名前をviewに渡して表示
        return view('mails.sendComp', ['name' => $name]);
    }

    // 指定IDのアイテム操作のログを取得
    public function showMailLogs(Request $request)
    {
        // 指定ユーザーIDが存在するかチェック
        $user = User::find($request->user_id);

        if (!empty($user)) {
            // 指定されたIDのユーザーデータが存在した時

            // 指定ユーザーIDのレコードを取得
            $mailLogs = MailLogs::where('user_id', $request->user_id)->get();
        }

        // 取得したログを渡してviewを表示
        return view('mails.showMailLogs', ['mailLogs' => $mailLogs ?? null]);
    }
}
