<!--------------------------------------------
// フォローログ画面 [showFollowLogs.blade.php]
// Author:Kenta Nakamoto
// Data:2024/07/16
//-------------------------------------------->
@extends('layouts.side')

@section('title','フォローログ')

@section('log','true')
@section('showLog','show')

@section('body')
    <!-- 表示内容 -->
    <div class="container text-center bg-dark-subtle" style="width: 500px">
        <h3 class="display-6"> フォローログ </h3>
    </div>

    <!--検索-->
    <div class="text-center">
        <form method="POST" action="{{route('users.showFollowLogs')}}">
            @csrf
            <input type="text" name="user_id" placeholder="IDを入力">
            <input type="submit" value="検索">
        </form>
    </div>

    <br>

    @if(isset($followLogs))

        <table class="table table-hover mx-auto p-2" style="width: 60%">
            <tr>
                <th>ログID</th>
                <th>ユーザーID</th>
                <th>ターゲットID</th>
                <th>操作</th>
                <th>生成日時</th>
            </tr>

            @foreach($followLogs as $followLog)
                <tr>
                    <td>{{$followLog['id']}}</td>
                    <td>{{$followLog['user_id']}}</td>
                    <td>{{$followLog['target_user_id']}}</td>

                    @if($followLog['action'] === 1)
                        <td>登録</td>
                    @elseif($followLog['action'] === 2)
                        <td>削除</td>
                    @endif

                    <td>{{$followLog['created_at']}}</td>
                </tr>
            @endforeach

        </table>

    @endif

@endsection
