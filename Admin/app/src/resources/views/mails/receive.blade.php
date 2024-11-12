<!--------------------------------------------
// ユーザー受信メール一覧 [receive.blade.php]
// Author:Kenta Nakamoto
// Data:2024/06/27
//-------------------------------------------->
@extends('layouts.side')

@section('title','ユーザー受信メール一覧')

@section('user','true')
@section('showUser','show')

@section('body')
    <!-- 表示内容 -->
    <div class="container text-center bg-dark-subtle" style="width: 500px">
        <h3 class="display-6"> ユーザー受信メール一覧 </h3>
    </div>
    <br>

    <!-- ページネーション -->
    <div class="d-flex justify-content-center">
        {{$mails->links('vendor.pagination.bootstrap-5')}}
    </div>

    <table class="table table-hover mx-auto p-2" style="width: 60%">
        <tr>
            <th>ID</th>
            <th>ユーザ名</th>
            <th>メールタイトル</th>
            <th>添付アイテムID</th>
            <th>開封状態</th>
            <th>作成日時</th>
            <th>更新日時</th>
        </tr>

        @foreach($mails as $mail)
            <tr>
                <td>{{$mail['id']}}</td>
                <td>{{$mail['user_name']}}</td>
                <td>{{$mail['mail_title']}}</td>
                <td>{{$mail['send_item_id']}}</td>
                @if($mail['unsealed_flag'])
                    <td>済</td>
                @else
                    <td>未</td>
                @endif

                <td>{{$mail['created_at']}}</td>
                <td>{{$mail['updated_at']}}</td>
            </tr>
        @endforeach

    </table>

@endsection
