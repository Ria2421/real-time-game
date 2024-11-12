<?php
//-------------------------------------------------
// アカウントコントローラー [AccountController.php]
// Author:Kenta Nakamoto
// Data:2024/06/11
//-------------------------------------------------

namespace App\Http\Controllers;

use App\Models\Account;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Hash;
use Illuminate\Support\Facades\Validator;

class AccountController extends Controller
{
    // アカウント一覧表示
    public function index(Request $request)
    {
        if (isset($request->id)) {
            // id指定有
            $data = Account::where('id', '=', $request->id)->get();

        } else {
            // id指定無
            $data = Account::simplePaginate(10);
        }

        // 表示処理・ログインセッションの開始
        return view('accounts/index', ['accounts' => $data, 'login' => $request->session()->get('login')]);
    }

    // 登録画面の表示
    public function create(Request $request)
    {
        return view('accounts/create', ['error' => $request['error'] ?? null]);
    }

    // アカウント登録処理
    public function store(Request $request)
    {
        // バリデーションチェック
        $validator = Validator::make($request->all(), [
            'name' => ['required', 'min:4'],
            'password' => ['required', 'confirmed']
        ]);

        if ($validator->fails()) {
            return redirect()->route('accounts.create')
                ->withErrors($validator)
                ->withInput();
        }

        // DBから指定のアカウント情報を取得
        $account = Account::where('name', '=', $request->name)->get();

        if ($account->count() == 0) {
            // 被りが無ければ作成
            Account::create(['name' => $request->name, 'password' => Hash::make($request->password)]);
        } else {
            // エラー表示
            return redirect()->route('accounts.create', ['error' => 'invalid']);
        }

        // 登録完了画面にリダイレクト
        return redirect()->route('accounts.storeComp', ['name' => $request->name]);
    }

    // 登録完了画面の表示
    public function storeComplete(Request $request)
    {
        return view('accounts/compRegister', ['name' => $request['name'] ?? null]);
    }

    // 削除確認画面の表示
    public function destroyConf(Request $request)
    {
        // 送られてきたIDからユーザーデータを取得
        $data = Account::where('id', '=', $request->id)->first();
        return view('accounts.destroyConf', ['accounts' => $data]);
    }

    // 削除処理
    public function destroy(Request $request)
    {
        // データの取得・削除処理
        $account = Account::findOrFail($request->id);
        $account->delete();

        // 完了表示・名前を渡す
        return redirect()->route('accounts.destroyComp', ['name' => $account->name]);
    }

    // 削除完了画面の表示
    public function destroyComp(Request $request)
    {
        return view('accounts.destroyComp', ['name' => $request->name]);
    }

    // 更新画面の表示
    public function showUpdate(Request $request)
    {
        // 送られてきたIDからアカウントデータを取得
        $data = Account::where('id', '=', $request->id)->first();
        return view('accounts.update', ['account' => $data]);
    }

    // 更新処理
    public function update(Request $request)
    {
        // バリデーションチェック
        $validator = Validator::make($request->all(), [
            'password' => ['required', 'confirmed']
        ]);

        if ($validator->fails()) {
            return redirect()->route('accounts.showUpdate')
                ->withErrors($validator)
                ->withInput();
        }

        // データの取得・更新処理
        $account = Account::findOrFail($request->id);
        $account->password = Hash::make($request->password);
        $account->save();

        // 更新完了表示・名前を渡す
        return redirect()->route('accounts.updateComp', ['name' => $account->name]);
    }

    // 更新完了表示
    public function UpdateComp(Request $request)
    {
        return view('accounts.updateComp', ['name' => $request->name]);
    }
}

//- デバック -//
//dd関数
//dd($request->account_id);

//Laravel DebugBar
// use Barryvdh\Debugbar\Facades\Debugbar;
//Debugbar::info('あいうえお');
//Debugbar::error('えらーだよ');

/* セッションに指定のキーで値を保存
$request->session()->put('name', 'hoge');
// セッションから指定のキーの値を取得
$value = $request->session()->get('name');
// 指定したデータをセッションから削除
$request->session()->forget('name');
// セッションのデータをすべて削除
$request->session()->flush();
// セッションに指定してキーが存在するか
if ($request->session()->exists('name')) {

}
dd($value);*/
