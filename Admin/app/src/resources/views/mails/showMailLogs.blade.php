<!--------------------------------------------
// メールログ画面 [showMailLogs.blade.php]
// Author:Kenta Nakamoto
// Data:2024/07/23
//-------------------------------------------->
@extends('layouts.side')

@section('title','メールログ')

@section('log','true')
@section('showLog','show')

@section('body')
    <!-- 表示内容 -->
    <div class="container text-center bg-dark-subtle" style="width: 500px">
        <h3 class="display-6"> メールログ </h3>
    </div>

    <!--検索-->
    <div class="text-center">
        <form method="POST" action="{{route('mails.showMailLogs')}}">
            @csrf
            <input type="text" name="user_id" placeholder="ユーザーIDを入力">
            <input type="submit" value="検索">
        </form>
    </div>

    <br>

    @if(isset($mailLogs))

        <table class="table table-hover mx-auto p-2" style="width: 60%">
            <tr>
                <th>ログID</th>
                <th>ユーザーID</th>
                <th>メールID</th>
                <th>添付アイテムID</th>
                <th>操作</th>
                <th>生成日時</th>
            </tr>

            @foreach($mailLogs as $mailLog)
                <tr>
                    <td>{{$mailLog['id']}}</td>
                    <td>{{$mailLog['user_id']}}</td>
                    <td>{{$mailLog['mail_id']}}</td>
                    <td>{{$mailLog['send_item_id']}}</td>

                    @if($mailLog['action'] === 1)
                        <td>受信</td>
                    @elseif($mailLog['action'] === 2)
                        <td>開封</td>
                    @endif

                    <td>{{$mailLog['created_at']}}</td>
                </tr>
            @endforeach

        </table>

    @endif

@endsection
