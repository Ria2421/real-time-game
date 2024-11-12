<?php
//-------------------------------------------------
// クリエイトステージモデル [CreateStage.php]
// Author:Kenta Nakamoto
// Data:2024/08/08
//-------------------------------------------------

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class CreateStage extends Model
{
    // 更新しないカラムを指定 (idはauto_incrementの為)
    protected $guarded = [
        'id',
    ];
}
