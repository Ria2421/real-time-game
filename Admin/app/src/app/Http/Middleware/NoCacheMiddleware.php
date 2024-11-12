<?php
//--------------------------------------------------
// ノーキャッシュミドルウェア [NoCacheMiddleware.php]
// Author:Kenta Nakamoto
//  Data :2024/07/01
// Update:2024/08/22
//---------------------------------------------------

namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;
use Symfony\Component\HttpFoundation\Response;

class NoCacheMiddleware
{
    /**
     * Handle an incoming request.
     *
     * @param \Closure(\Illuminate\Http\Request): (\Symfony\Component\HttpFoundation\Response) $next
     */
    public function handle(Request $request, Closure $next): Response
    {
        $response = $next($request);
        $response->withHeaders(['Cache-Control' => 'no-store', 'Max-Age' => 0]); // キャッシュを保存しないように設定
        return $response;
    }
}
