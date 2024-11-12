<!--------------------------------------------
// アイテムログ画面 [showItemLogs.blade.php]
// Author:Kenta Nakamoto
// Data:2024/07/23
//-------------------------------------------->
@extends('layouts.side')

@section('title','アイテムログ')

@section('log','true')
@section('showLog','show')

@section('body')
    <!-- 表示内容 -->
    <div class="container text-center bg-dark-subtle" style="width: 500px">
        <h3 class="display-6"> アイテムログ </h3>
    </div>

    <!--検索-->
    <div class="text-center">
        <form method="POST" action="{{route('users.showItemLogs')}}">
            @csrf
            <input type="text" name="user_id" placeholder="ユーザーIDを入力">
            <input type="submit" value="検索">
        </form>
    </div>

    <br>

    @if(isset($itemLogs))

        <table class="table table-hover mx-auto p-2" style="width: 60%">
            <tr>
                <th>ログID</th>
                <th>ユーザーID</th>
                <th>アイテムID</th>
                <th>操作</th>
                <th>個数</th>
                <th>生成日時</th>
            </tr>

            @foreach($itemLogs as $itemLog)
                <tr>
                    <td>{{$itemLog['id']}}</td>
                    <td>{{$itemLog['user_id']}}</td>
                    <td>{{$itemLog['target_item_id']}}</td>

                    @if($itemLog['action'] === 1)
                        <td>消費</td>
                    @elseif($itemLog['action'] === 2)
                        <td>入手</td>
                    @endif

                    <td>{{$itemLog['quantity']}}</td>
                    <td>{{$itemLog['created_at']}}</td>
                </tr>
            @endforeach

        </table>

    @endif

@endsection
