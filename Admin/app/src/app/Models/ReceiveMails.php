<?php
//-------------------------------------------------
// 受信メールモデル [ReceiveMails.php]
// Author:Kenta Nakamoto
// Data:2024/07/08
//-------------------------------------------------

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class ReceiveMails extends Model
{
    // 更新しないカラムを指定 (idはauto_incrementの為)
    protected $guarded = [
        'id',
    ];
}
