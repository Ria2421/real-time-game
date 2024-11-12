<!--------------------------------------------
// サインインレイアウト [signInApp.blade.php]
// Author:Kenta Nakamoto
// Data:2024/07/01
//-------------------------------------------->
<!doctype html>
<html lang="ja">
<head>
    <title>@yield('title')</title>
    <link rel="canonical" href="https://getbootstrap.jp/docs/5.3/examples/sign-in/">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css">
    <link href="/signin.css" rel="stylesheet">
</head>
<body class="text-center d-flex flex-column">

<!-- ヘッダー -->
<div class="container">
    <header
        class="d-flex flex-wrap align-items-center justify-content-center justify-content-md-between py-3 mb-4 border-bottom">

        <ul class="nav col-12 col-md-auto mb-2 justify-content-center mb-md-0">
            <a href="{{route('accounts.index')}}"
               class="link-body-emphasis d-inline-flex text-decoration-none rounded">
                <img src="/images/home.png" alt="ホーム" width="45px" height="45px">
                ホーム
            </a>
        </ul>

        <div>
            <a href="{{route('auth.logout')}}"
               class="link-body-emphasis d-inline-flex text-decoration-none rounded">
                <img src="/images/logout.png" alt="ログアウト" width="45px" height="45px">
                ログアウト
            </a>
        </div>

    </header>
</div>

@yield('body')

<script src="https://code.jquery.com/jquery-3.3.1.slim.min.js"
        integrity="sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo"
        crossorigin="anonymous"></script>
</body>
</html>
