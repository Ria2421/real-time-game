<!--------------------------------------------
// ログイン画面 [haveItems.blade.php]
// Author:Kenta Nakamoto
// Data:2024/06/10
//-------------------------------------------->

<!doctype html>
<html lang="ja">
<head>
    <title>Signin</title>
    <link rel="canonical" href="https://getbootstrap.jp/docs/5.3/examples/sign-in/">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css">
    <link href="/signin.css" rel="stylesheet">
</head>
<body class="text-center">

<form class="form-signin" method="POST" action="{{route('auth.login')}}">
    @csrf
    <h1 class="h3 mb-3 font-weight-normal">サインイン</h1>

    @if($errors->any())
        <ul>
            @foreach($errors->all() as $error)
                <li>{{$error}}</li>
            @endforeach
        </ul>
    @endif

    @if(isset($error))
        <ul>
            <li>入力された内容が正しくありません。</li>
        </ul>
    @endif

    <label for="inputEmail" class="sr-only">アカウント名</label>
    <input type="text" id="inputEmail" name="name" class="form-control" placeholder="アカウント名" required autofocus>
    <label for="inputPassword" class="sr-only">パスワード</label>
    <input type="password" id="inputPassword" name="password" class="form-control" placeholder="パスワード" required>
    <div class="checkbox mb-3">
    </div>
    <button class="btn btn-lg btn-primary btn-block" name="login_btn" type="submit">Sign in</button>
    <input type="hidden" name="action" value="doLogin">
    <p class="mt-5 mb-3 text-muted">&copy; 2024</p>
</form>

<script src="https://code.jquery.com/jquery-3.3.1.slim.min.js"
        integrity="sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo"
        crossorigin="anonymous"></script>
</body>
</html>
