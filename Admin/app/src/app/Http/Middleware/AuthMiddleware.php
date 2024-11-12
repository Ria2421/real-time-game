<?php
//--------------------------------------------------
// 認証ミドルウェア [AuthMiddleware.php]
// Author:Kenta Nakamoto
//  Data :2024/06/25
// Update:2024/08/22
//---------------------------------------------------

namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;
use Symfony\Component\HttpFoundation\Response;

class AuthMiddleware
{
    /**
     * Handle an incoming request.
     *
     * @param \Closure(\Illuminate\Http\Request): (\Symfony\Component\HttpFoundation\Response) $next
     */
    public function handle(Request $request, Closure $next): Response
    {
        // ログインしているかチェック
        if (!$request->session()->exists('login')) {
            // ログイン画面にリダイレクト
            return redirect()->route('auth.index');
        }

        $response = $next($request);

        return $response;
    }
}
