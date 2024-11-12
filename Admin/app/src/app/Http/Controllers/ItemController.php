<?php
//-------------------------------------------------
// アイテムコントローラー [ItemController.php]
// Author:Kenta Nakamoto
// Data:2024/06/18
//-------------------------------------------------

namespace App\Http\Controllers;

use App\Models\Item;
use Illuminate\Http\Request;

class ItemController extends Controller
{
    // アイテム一覧の表示
    public function index(Request $request)
    {
        // アイテムをページネーションに対応した形式で取得
        $data = Item::paginate(10);
        // ページネーションの表示幅を設定
        $data->onEachSide(2);
        // 取得データをviewに渡して表示
        return view('items/index', ['items' => $data]);
    }
}
