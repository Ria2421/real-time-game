<?php
//-------------------------------------------------
// 所持アイテムモデル [HaveItem.php]
// Author:Kenta Nakamoto
// Data:2024/07/18
//-------------------------------------------------

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class HaveItem extends Model
{
    use HasFactory;

    // 更新しないカラムを指定 (idはauto_incrementの為)
    protected $guarded = [
        'id',
    ];
}
