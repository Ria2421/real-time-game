<!--------------------------------------------
// 削除完了画面 [destroyConf.blade.php]
// Author:Kenta Nakamoto
// Data:2024/06/25
//-------------------------------------------->
@extends('layouts.side')

@section('title','削除確認')

@section('master','true')
@section('showMaster','show')

@section('body')
    <!-- 表示内容 -->
    <div class="container text-center bg-dark-subtle" style="width: 500px">
        <h3 class="display-6"> アカウント登録 </h3>
    </div>
    <br>
    <div class="text-center">
        [ {{$accounts['name']}} ]を削除しますか？
    </div>
    <br>
    <div class="text-center">
        <a href="{{ route('accounts.destroy', ['id'=>$accounts['id']]) }}" class="btn btn-info">削除</a>
    </div>
@endsection
