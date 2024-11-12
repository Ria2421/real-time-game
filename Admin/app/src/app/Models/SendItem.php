<?php
//-------------------------------------------------
// メール添付アイテムモデル [SendItem.php]
// Author:Kenta Nakamoto
// Data:2024/06/27
//-------------------------------------------------

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class SendItem extends Model
{
    use HasFactory;

    // 更新しないカラムを指定 (idはauto_incrementの為)
    protected $guarded = [
        'id',
    ];
}
