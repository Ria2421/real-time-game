<!--------------------------------------------
// webページレイアウト [app.blade.php]
// Author:Kenta Nakamoto
// Data:2024/07/01
//-------------------------------------------->
<!DOCTYPE html>
<html lang="ja">
<head>
    <meta charset="UTF-8">
    <title>@yield('title')</title>
    <link href="/css/bootstrap.min.css" rel="stylesheet"
          integrity="sha384-9ndCyUaIbzAi2FUVXJi0CjmCapSmO7SnpJef0486qhLnuZ2cdeRhO02iuK6FUUVM" crossorigin="anonymous">
</head>
<body>

<!-- ヘッダー -->
<div class="container">
    <header
        class="d-flex flex-wrap align-items-center justify-content-center justify-content-md-between py-3 mb-4 border-bottom">

        <ul class="nav col-12 col-md-auto mb-2 justify-content-center mb-md-0">
            <li>
                <form method="GET" action="{{route('accounts.create')}}">
                    @csrf
                    <button type="submit" class="btn btn-outline-primary me-2">登録</button>
                </form>
            </li>
            <li>
                <form method="GET" action="{{route('mails.showSendMail')}}">
                    @csrf
                    <button type="submit" class="btn btn-outline-primary me-2">メール送信</button>
                </form>
            </li>
            <li><a href="{{route('accounts.index')}}" class="nav-link px-2 @yield('account')">アカウント</a></li>
            <li><a href="{{route('users.index')}}" class="nav-link px-2 @yield('user')">ユーザー</a></li>
            <li><a href="{{route('items.index')}}" class="nav-link px-2 @yield('item')">アイテム</a></li>
            <li><a href="{{route('users.showItems')}}" class="nav-link px-2 @yield('haveItem')">持ち物リスト</a></li>
            <li><a href="{{route('mails.index')}}" class="nav-link px-2 @yield('mail')">定型メール</a></li>
            <li><a href="{{route('mails.showSendItems')}}"
                   class="nav-link px-2 @yield('sendItem')">添付アイテムリスト</a></li>
            <li><a href="{{route('mails.showReceiveMails')}}"
                   class="nav-link px-2 @yield('receive')">ユーザー受信メール</a></li>
            <li><a href="{{route('users.findFollows')}}"
                   class="nav-link px-2 @yield('follow')">フォロー情報</a></li>
            <li><a href="{{route('users.findFollowLogs')}}"
                   class="nav-link px-2 @yield('followLogs')">フォローログ</a></li>
            <li><a href="{{route('users.findItemLogs')}}"
                   class="nav-link px-2 @yield('itemLogs')">アイテムログ</a></li>
            <li><a href="{{route('mails.findMailLogs')}}"
                   class="nav-link px-2 @yield('mailLogs')">メールログ</a></li>
        </ul>

        <div class="col-md-3 text-end">
            <form method="POST" action="{{route('auth.logout')}}">
                @csrf
                <button type="submit" class="btn btn-outline-primary me-2">ログアウト</button>
            </form>
        </div>

    </header>
</div>

@yield('body')

<script src="/js/bootstrap.bundle.min.js"
        integrity="sha384-geWF76RCwLtnZ8qwWowPQNguL3RmwHVBC9FhGdlKrxdiJJigb/j/68SIy3Te4Bkz"
        crossorigin="anonymous"></script>

</body>
</html>
