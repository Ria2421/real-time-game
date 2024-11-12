<!--------------------------------------------
// 更新画面 [update.blade.php]
// Author:Kenta Nakamoto
// Data:2024/06/25
//-------------------------------------------->

@extends('layouts.signInApp')

@section('title','パスワード更新')

@section('body')
    <form class="form-signin" method="POST" action="{{route('accounts.update',['id' => $account['id']])}}">
        @csrf
        <h1 class="h3 mb-3 font-weight-normal">パスワード更新</h1>

        @if($errors->any())
            <ul>
                @foreach($errors->all() as $error)
                    <li>{{$error}}</li>
                @endforeach
            </ul>
        @endif

        <label for="inputEmail" class="sr-only">アカウント名</label>
        <input type="text" id="inputEmail" name="name" class="form-control" value="{{$account['name']}}" disabled>
        <label for="inputPassword" class="sr-only">パスワード</label>
        <input type="password" id="inputPassword" name="password" class="form-control" placeholder="パスワード"
               required>
        <label for="inputPassword" class="sr-only">パスワード再入力</label>
        <input type="password" id="inputPassword" name="password_confirmation" class="form-control"
               placeholder="パスワード再入力"
               required>
        <div class="checkbox mb-3">
        </div>
        <button class="btn btn-lg btn-primary btn-block" name="register_btn" type="submit">更新</button>
        <p class="mt-5 mb-3 text-muted">&copy; 2024</p>
    </form>
@endsection
