<?php
//-------------------------------------------------
// アチーブメント進捗モデル [AchievementProgress.php]
// Author:Kenta Nakamoto
// Data:2024/08/08
//-------------------------------------------------

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class AchievementProgress extends Model
{
    use HasFactory;

    // 更新しないカラムを指定 (idはauto_incrementの為)
    protected $guarded = [
        'id',
    ];
}
