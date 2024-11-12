<!--------------------------------------------
// 更新完了画面 [updateComp.blade.php]
// Author:Kenta Nakamoto
// Data:2024/06/24
//-------------------------------------------->

@extends('layouts.side')

@section('title','更新完了')

@section('master','true')
@section('showMaster','show')

@section('body')
    <!-- 表示内容 -->
    <div class="container text-center bg-dark-subtle" style="width: 500px">
        <h3 class="display-6"> パスワード更新 </h3>
    </div>
    <div class="text-center">
        <br>
        [ {{$name}} ]の更新が完了しました。
    </div>
@endsection
