<!--------------------------------------------
// 登録画面 [create.blade.php]
// Author:Kenta Nakamoto
// Data:2024/06/10
//-------------------------------------------->
@extends('layouts.signInApp')

@section('title','アカウント作成')

@section('body')

    <form class="form-signin" method="POST" action="{{route('accounts.store')}}">
        @csrf
        <h1 class="h3 mb-3 font-weight-normal">アカウント登録</h1>

        @if($errors->any())
            <ul>
                @foreach($errors->all() as $error)
                    <li>{{$error}}</li>
                @endforeach
            </ul>
        @endif

        @if($error)
            <ul>
                <li>入力された名前は既に登録されています。</li>
            </ul>
        @endif

        <label for="inputEmail" class="sr-only">アカウント名</label>
        <input type="text" id="inputEmail" name="name" class="form-control" placeholder="アカウント名" required
               autofocus>
        <label for="inputPassword" class="sr-only">パスワード</label>
        <input type="password" id="inputPassword" name="password" class="form-control" placeholder="パスワード"
               required>
        <label for="inputPassword" class="sr-only">パスワード再入力</label>
        <input type="password" id="inputPassword" name="password_confirmation" class="form-control"
               placeholder="パスワード再入力"
               required>
        <div class="checkbox mb-3">
        </div>
        <button class="btn btn-lg btn-primary btn-block" name="register_btn" type="submit">登録</button>
        <input type="hidden" name="action" value="doRegister">
        <p class="mt-5 mb-3 text-muted">&copy; 2024</p>
    </form>

@endsection
