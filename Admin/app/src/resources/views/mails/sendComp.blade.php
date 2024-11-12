<!--------------------------------------------
// 送信完了画面 [sendComp.blade.php]
// Author:Kenta Nakamoto
// Data:2024/06/28
//-------------------------------------------->
@extends('layouts.side')

@section('title','送信完了')

@section('action','true')
@section('showAction','show')

@section('body')

    <div class="container text-center bg-dark-subtle" style="width: 500px">
        <h3 class="display-6"> 送信確認 </h3>
    </div>
    <div class="text-center">
        <br>
        {{$name}}にメールの送信が完了しました。
    </div>

@endsection
