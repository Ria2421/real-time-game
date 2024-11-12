<!--------------------------------------------
// メール送信画面 [sendMail.blade.php]
// Author:Kenta Nakamoto
// Data:2024/06/27
//-------------------------------------------->
@extends('layouts.side')

@section('title','送信')

@section('action','true')
@section('showAction','show')

@section('body')
    <!-- 表示内容 -->
    <div class="container text-center bg-dark-subtle" style="width: 500px">
        <h3 class="display-6"> メール送信 </h3>
    </div>
    <br>

    <!-- 送信フォーム -->
    <div class="mx-auto p-2" style="width: 400px;">
        @if(!empty(request()->get('error')))
            <div class="text-danger text-center">指定したユーザーIDは存在しません。</div>
            <br>
        @endisset

        <form method="POST" action="{{route('mails.sendMail')}}">
            @csrf
            <div>送信するユーザーIDを入力してください (0:全ユーザー)</div>
            <input type="text" name="user_id" class="form-control" required>

            <br>

            <div>送信するメール内容を選択してください</div>
            <select name="mail_id" class="form-select">
                @foreach($mails as $mail)
                    <option value="{{$mail['id']}}">{{$mail['id']}} : {{$mail['title']}}</option>
                @endforeach
            </select>

            <br>

            <div>添付するアイテムを選択してください</div>
            <select name="send_item_id" class="form-select">
                @foreach($items as $item)
                    <option value="{{$item['id']}}">{{$item['id']}} : {{$item['name']}} * {{$item['quantity']}}</option>
                @endforeach
            </select>

            <br>

            <button type="submit" class="btn btn-outline-primary me-2">送信</button>
        </form>
    </div>
@endsection
