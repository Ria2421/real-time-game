<!--------------------------------------------
// 所持アイテム一覧画面 [haveItem.blade.php]
// Author:Kenta Nakamoto
// Data:2024/06/11
//-------------------------------------------->
@extends('layouts.side')

@section('title','所持アイテム一覧')

@section('user','true')
@section('showUser','show')

@section('body')
    <!-- 表示内容 -->
    <div class="container text-center bg-dark-subtle" style="width: 500px">
        <h3 class="display-6">▼ 所持アイテム一覧 ▼</h3>
    </div>

    <!--検索-->
    <div class="text-center">
        <form method="POST" action="{{route('users.showItem')}}">
            @csrf
            <input type="text" name="user_id" placeholder="IDを入力">
            <input type="submit" value="検索">
        </form>
    </div>

    <br>

    <!-- ページネーション -->
    <div class="d-flex justify-content-center">
        @if(isset($items))
            {{$items->links('vendor.pagination.bootstrap-5')}}
        @endif

    </div>

    <table class="table table-hover mx-auto p-2" style="width: 60%">
        <tr>
            <th>ユーザーID</th>
            <th>ユーザー名</th>
            <th>アイテム名</th>
            <th>所持個数</th>
        </tr>

        @if(isset($items))
            @foreach($items as $item)
                <tr>
                    <td>{{$user->id}}</td>
                    <td>{{$user->name}}</td>
                    <td>{{$item->name}}</td>
                    <td>{{$item->pivot->quantity}}</td>
                </tr>
            @endforeach
        @endif

    </table>
@endsection
