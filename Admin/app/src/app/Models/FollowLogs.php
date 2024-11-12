<?php
//-------------------------------------------------
// フォローログモデル [follow.php]
// Author:Kenta Nakamoto
// Data:2024/07/25
//-------------------------------------------------

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class FollowLogs extends Model
{
    use HasFactory;

    // 更新しないカラムを指定 (idはauto_incrementの為)
    protected $guarded = [
        'id',
    ];
}
