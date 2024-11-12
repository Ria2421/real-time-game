<!--------------------------------------------
// 削除完了画面 [compRegister.blade.php]
// Author:Kenta Nakamoto
// Data:2024/06/24
//-------------------------------------------->

@extends('layouts.side')

@section('title','削除完了')

@section('master','true')
@section('showMaster','show')

@section('body')
    <div class="container text-center bg-dark-subtle" style="width: 500px">
        <h3 class="display-6"> アカウント登録 </h3>
    </div>
    <div class="text-center">
        <br>
        [ {{$name}} ]の削除が完了しました。
    </div>
@endsection
