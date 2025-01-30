<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class NgWord extends Model
{
    protected $table = 'ng_words';

    // $guardedには更新しないカラムを指定する
    protected $guarded = [
        'id',
    ];
}
