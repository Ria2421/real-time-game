<?php
//-------------------------------------------------
// アカウントモデル [Account.php]
// Author:Kenta Nakamoto
// Data:2024/06/17
//-------------------------------------------------

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Account extends Model
{
    // 更新しないカラムを指定 (idはauto_incrementの為)
    protected $guarded = [
        'id',
    ];
}
