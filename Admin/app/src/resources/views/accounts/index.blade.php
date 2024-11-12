<!--------------------------------------------
// アカウント一覧画面 [user.blade.php]
// Author:Kenta Nakamoto
// Data:2024/06/11
//-------------------------------------------->
@extends('layouts.side')

@section('title','アカウント一覧')

@section('master','true')
@section('showMaster','show')

@section('body')
    <!-- 表示内容 -->
    <div class="container text-center bg-dark-subtle" style="width: 500px">
        <h3 class="display-6"> アカウント一覧 </h3>
    </div>

    <!--検索-->
    <div class="text-center">
        <form method="POST" action="{{route('accounts.show')}}">
            @csrf
            <input type="text" name="id" placeholder="IDを入力">
            <input type="submit" value="検索">
        </form>
    </div>

    <br>

    <!-- ページネーション -->
    <div class="d-flex justify-content-center">
        {{$accounts->links('vendor.pagination.bootstrap-5')}}
    </div>

    <table class="table table-hover mx-auto p-2" style="width: 80%">
        <tr>
            <th>ID</th>
            <th>名前</th>
            <th>パスワード</th>
            <th>生成日時</th>
            <th>更新日時</th>
            <th>操作</th>
        </tr>

        @foreach($accounts as $account)
            <tr>
                <td>{{$account['id']}}</td>
                <td>{{$account['name']}}</td>
                <td>{{$account['password']}}</td>
                <td>{{$account['created_at']}}</td>
                <td>{{$account['updated_at']}}</td>
                <td width="140px">
                    @if($login == $account['id'])

                    @else
                        <a href="{{ route('accounts.destroyConf', ['id'=>$account['id']]) }}"
                           class="btn btn-danger">削除</a>
                    @endif

                    <a href="{{ route('accounts.showUpdate', ['id'=>$account['id']]) }}"
                       class="btn btn-success">更新</a>
                </td>
            </tr>
        @endforeach

    </table>
@endsection
