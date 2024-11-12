<?php
//-------------------------------------------------
// ルート [Web.php]
// Author:Kenta Nakamoto
// Data:2024/06/11
//-------------------------------------------------
use App\Http\Controllers\AccountController;
use App\Http\Controllers\AuthController;
use App\Http\Controllers\ItemController;
use App\Http\Controllers\MailController;
use App\Http\Controllers\UserController;
use App\Http\Middleware\AuthMiddleware;
use App\Http\Middleware\NoCacheMiddleware;
use Illuminate\Support\Facades\Route;

Route::middleware(NoCacheMiddleware::class)->group(function () {
    // ログイン画面の表示
    Route::get('/', [AuthController::class, 'index'])->name('auth.index');

    // ログイン画面の表示
    Route::get('authentications/index', [AuthController::class, 'index'])->name('auth.index');

    // ログイン処理
    Route::post('authentications/login', [AuthController::class, 'login'])->name('auth.login');

    // ログアウト処理
    Route::get('authentications/logout', [AuthController::class, 'logout'])->name('auth.logout');

    // アカウントルート
    Route::prefix('accounts')->name('accounts.')->controller(AccountController::class)
        ->middleware(AuthMiddleware::class)->group(function () {
            Route::get('index/{account_id?}', 'index')->name('index');             // アカウントの表示
            Route::post('show', 'index')->name('show');                            // 指定アカウントの表示
            Route::get('create', 'create')->name('create');                        // 登録画面の表示
            Route::post('store', 'store')->name('store');                          // 登録処理
            Route::get('storeComp', 'storeComplete')->name('storeComp');           // 登録完了画面の表示
            Route::get('{id}/destroyConf', 'destroyConf')->name('destroyConf');    // 削除確認画面
            Route::get('{id}/destroy', 'destroy')->name('destroy');                // 削除処理
            Route::get('{name}/destroyComp', 'destroyComp')->name('destroyComp');  // 削除完了画面の表示
            Route::get('showUpdate', 'showUpdate')->name('showUpdate');            // 更新画面の表示
            Route::post('{id}/update', 'update')->name('update');                  // 更新処理
            Route::get('{name}/updateComp', 'updateComp')->name('updateComp');     // 更新完了表示
        });

    Route::middleware(AuthMiddleware::class)->group(function () {
        //==========================================
        // ユーザー関連 =====

        // ユーザーの表示
        Route::get('users/index/{user_id?}', [UserController::class, 'index'])->name('users.index');

        // 所持アイテムの表示
        Route::get('users/haveItems', [UserController::class, 'showItem'])->name('users.showItems');
        Route::post('users/haveItems/{user_id?}', [UserController::class, 'showItem'])->name('users.showItem');

        // 指定IDのフォロー情報表示
        Route::get('users/findFollows', [UserController::class, 'showFollow'])->name('users.findFollows');
        Route::post('users/findFollows/{user_id?}', [UserController::class, 'showFollow'])->name('users.showFollows');

        // 指定IDのフォロー操作ログ表示
        Route::get('users/findFollowLogs', [UserController::class, 'showFollowLogs'])->name('users.findFollowLogs');
        Route::post('users/findFollowLogs/{user_id?}',
            [UserController::class, 'showFollowLogs'])->name('users.showFollowLogs');

        // 指定IDのアイテム操作ログ表示
        Route::get('users/findItemLogs', [UserController::class, 'showItemLogs'])->name('users.findItemLogs');
        Route::post('users/findItemLogs/{user_id?}',
            [UserController::class, 'showItemLogs'])->name('users.showItemLogs');

        //==========================================
        // アイテム関連 =====

        // アイテムの表示
        Route::get('items/index/{item_id?}', [ItemController::class, 'index'])->name('items.index');

        //==========================================
        // メール関連 =====

        // メール一覧の表示
        Route::get('mails/index/{mail_id?}', [MailController::class, 'index'])->name('mails.index');
        // 指定メールIDの表示
        Route::post('mails/show', [MailController::class, 'index'])->name('mails.show');

        // メール添付アイテム一覧の表示
        Route::get('mails/showSendItems', [MailController::class, 'showSendItems'])->name('mails.showSendItems');

        // ユーザー受信メール表示
        Route::get('mails/showReceiveMails',
            [MailController::class, 'showReceiveMails'])->name('mails.showReceiveMails');

        // メール送信画面表示
        Route::get('mails/showSendMail', [MailController::class, 'showSendMail'])->name('mails.showSendMail');

        // 送信処理
        Route::post('mails/sendMail', [MailController::class, 'sendMail'])->name('mails.sendMail');

        // 送信完了画面表示
        Route::get('mails/showSendComp', [MailController::class, 'showSendComp'])->name('mails.showSendComp');

        // 指定IDのメール操作ログ表示
        Route::get('mails/findMailLogs', [MailController::class, 'showMailLogs'])->name('mails.findMailLogs');
        Route::post('mails/findMailLogs/{user_id?}',
            [MailController::class, 'showMailLogs'])->name('mails.showMailLogs');
    });
});


