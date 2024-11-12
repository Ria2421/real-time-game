<!--------------------------------------------
// アイテム一覧画面 [item.blade.php]
// Author:Kenta Nakamoto
// Data:2024/06/11
//-------------------------------------------->
@extends('layouts.side')

@section('title','アカウント一覧')

@section('master','true')
@section('showMaster','show')

@section('body')
    <div class="container text-center bg-dark-subtle" style="width: 500px">
        <h3 class="display-6"> アイテム一覧 </h3>
    </div>

    <!-- ページネーション -->
    <div class="d-flex justify-content-center">
        {{$items->links('vendor.pagination.bootstrap-5')}}
    </div>

    <table class="table table-hover mx-auto p-2" style="width: 60%">
        <tr>
            <th>ID</th>
            <th>名前</th>
            <th>種別</th>
            <th>効果値</th>
            <th>説明</th>
        </tr>

        @foreach($items as $item)
            <tr>
                <td>{{$item['id']}}</td>
                <td>{{$item['name']}}</td>

                @if($item['type'] === 1)
                    <td>消耗品</td>
                @elseif($item['type'] === 2)
                    <td>装備品</td>
                @endif

                <td>{{$item['effect_value']}}</td>
                <td>{{$item['text']}}</td>
            </tr>
        @endforeach

    </table>
@endsection
