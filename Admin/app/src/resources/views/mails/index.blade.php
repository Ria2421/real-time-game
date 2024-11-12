<!--------------------------------------------
// メール定型一覧画面 [index.blade.php]
// Author:Kenta Nakamoto
// Data:2024/06/11
//-------------------------------------------->

@extends('layouts.side')

@section('title','メール定型文')

@section('master','true')
@section('showMaster','show')

@section('body')

    <!-- 表示内容 -->
    <div class="container text-center bg-dark-subtle" style="width: 500px">
        <h3 class="display-6"> メール定型文 </h3>
    </div>

    <!--検索-->
    <div class="text-center">
        <form method="POST" action="{{route('mails.show')}}">
            @csrf
            <input type="text" name="id" placeholder="メールIDを入力">
            <input type="submit" value="検索">
        </form>
    </div>

    <br>

    <!-- ページネーション -->
    <div class="d-flex justify-content-center">
        {{$mails->links('vendor.pagination.bootstrap-5')}}
    </div>

    <table class="table table-hover mx-auto p-2" style="width: 60%">
        <tr>
            <th>ID</th>
            <th>タイトル</th>
            <th>本文</th>
            <th>生成日時</th>
            <th>更新日時</th>
        </tr>

        @foreach($mails as $mail)
            <tr>
                <td>{{$mail['id']}}</td>
                <td>{{$mail['title']}}</td>
                <td>{{$mail['content']}}</td>
                <td>{{$mail['created_at']}}</td>
                <td>{{$mail['updated_at']}}</td>
            </tr>
        @endforeach

    </table>

@endsection
