<!--------------------------------------------
// ユーザー覧画面 [user.blade.php]
// Author:Kenta Nakamoto
// Data:2024/06/11
//-------------------------------------------->
@extends('layouts.side')

@section('title','ユーザー一覧')

@section('user','true')
@section('showUser','show')

@section('body')
    <!-- 表示内容 -->
    <div class="container text-center bg-dark-subtle" style="width: 500px">
        <h3 class="display-6"> ユーザー一覧 </h3>
    </div>
    <br>

    <div class="d-flex justify-content-center">
        {{$users->links('vendor.pagination.bootstrap-5')}}
    </div>

    <table class="table table-hover mx-auto p-2" style="width: 60%">
        <tr>
            <th>ID</th>
            <th>名前</th>
            <th>アイコンID</th>
        </tr>

        @foreach($users as $user)
            <tr>
                <td>{{$user['id']}}</td>
                <td>{{$user['name']}}</td>
                <td>{{$user['icon_id']}}</td>
            </tr>
        @endforeach
    </table>
@endsection
