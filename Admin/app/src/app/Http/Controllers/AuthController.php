<?php
//-------------------------------------------------
// 認証用コントローラー [AuthController.php]
// Author:Kenta Nakamoto
// Data:2024/06/18
//-------------------------------------------------

namespace App\Http\Controllers;

use App\Models\Account;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Hash;
use Illuminate\Support\Facades\Validator;

class AuthController extends Controller
{
    // ログイン画面を表示する
    public function index(Request $request)
    {
        // ログインしてるかチェック
        return view('authentications/index', ['error' => $request['error'] ?? null]);
    }

    // ログイン処理
    public function login(Request $request)
    {
        // バリデーションチェック
        $validator = Validator::make($request->all(), [
            'name' => ['required', 'min:4'],
            'password' => 'required',
        ]);

        if ($validator->fails()) {
            return redirect("/")
                ->withErrors($validator)
                ->withInput();
        }

        // DBから指定のアカウント情報を取得
        $account = Account::where('name', '=', $request->name)->get();

        if ($account->count() == 0) {
            // エラー表示
            return redirect()->route('auth.index', ['error' => 'invalid']);
        }

        if (Hash::check($request->password, $account[0]->password)) {
            // 一致した時

            // セッションにログイン情報を登録
            $request->session()->put('login', $account[0]->id);
            // 一覧表示
            return redirect()->route('accounts.index');
        } else {
            // 一致しなかった時

            // エラー表示
            return redirect()->route('accounts.index', ['error' => 'invalid']);
        }
    }

    // ログアウト処理
    public function logout(Request $request)
    {
        // 指定したデータをセッションから削除
        $request->session()->forget('login');

        // ログイン画面にリダイレクト
        return redirect()->route('auth.index');
    }
}
