<!--------------------------------------------
// 配布アイテム一覧画面 [sendItem.blade.php]
// Author:Kenta Nakamoto
// Data:2024/06/11
//-------------------------------------------->
@extends('layouts.side')

@section('title','送信完了')

@section('master','true')
@section('showMaster','show')

@section('body')
    <!-- 表示内容 -->
    <div class="container text-center bg-dark-subtle" style="width: 500px">
        <h3 class="display-6"> 添付アイテムリスト </h3>
    </div>

    <br>

    <!-- ページネーション -->
    <div class="d-flex justify-content-center">
        {{$items->links('vendor.pagination.bootstrap-5')}}
    </div>

    <table class="table table-hover mx-auto p-2" style="width: 60%">
        <tr>
            <th>ID</th>
            <th>アイテム名</th>
            <th>個数</th>
            <th>生成日時</th>
            <th>更新日時</th>
        </tr>

        @foreach($items as $item)
            <tr>
                <td>{{$item['id']}}</td>
                <td>{{$item['name']}}</td>
                <td>{{$item['quantity']}}</td>
                <td>{{$item['created_at']}}</td>
                <td>{{$item['updated_at']}}</td>
            </tr>
        @endforeach

    </table>

@endsection
