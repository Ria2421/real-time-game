<!--------------------------------------------
// フォロー一覧画面 [showFollow.blade.php]
// Author:Kenta Nakamoto
// Data:2024/07/04
//-------------------------------------------->
@extends('layouts.side')

@section('title','フォロー一覧')

@section('user','true')
@section('showUser','show')

@section('body')
    <!-- 表示内容 -->
    <div class="container text-center bg-dark-subtle" style="width: 500px">
        <h3 class="display-6"> フォロー情報 </h3>
    </div>

    <!--検索-->
    <div class="text-center">
        <form method="POST" action="{{route('users.showFollows')}}">
            @csrf
            <input type="text" name="user_id" placeholder="IDを入力">
            <input type="submit" value="検索">
        </form>
    </div>

    <br>

    <div class="container text-center">
        <div class="row">
            <div class="col">
                @if(isset($follows))
                    <!--フォロー-->
                    <div>フォロー</div>
                    <table class="table table-hover mx-auto p-2">
                        <tr>
                            <th>ID</th>
                            <th>ユーザー名</th>
                        </tr>


                        @foreach($follows as $follow)
                            <tr>
                                <td>{{$follow->id}}</td>
                                <td>{{$follow->name}}</td>
                            </tr>
                        @endforeach
                    </table>
                @endif
            </div>
            <div class="col">
                @if(isset($followers))
                    <!--フォロワー-->
                    <div>フォロワー</div>
                    <table class="table table-hover mx-auto p-2" style="width: 60%">
                        <tr>
                            <th>ID</th>
                            <th>ユーザー名</th>
                        </tr>

                        @foreach($followers as $follower)
                            <tr>
                                <td>{{$follower->id}}</td>
                                <td>{{$follower->name}}</td>
                            </tr>
                        @endforeach
                    </table>
                @endif
            </div>
            <div class="col">
                @if(isset($mutualUsers))
                    <!--相互フォロー-->
                    <div>相互フォロー</div>
                    <table class="table table-hover mx-auto p-2" style="width: 60%">
                        <tr>
                            <th>ID</th>
                            <th>ユーザー名</th>
                        </tr>

                        @foreach($mutualUsers as $mutualUser)
                            <tr>
                                <td>{{$mutualUser->id}}</td>
                                <td>{{$mutualUser->name}}</td>
                            </tr>
                        @endforeach
                    </table>
                @endif
            </div>
        </div>
    </div>

@endsection
