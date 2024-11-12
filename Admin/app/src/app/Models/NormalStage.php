<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class NormalStage extends Model
{
    protected function casts()
    {
        return [
            'gimick_pos' => 'array',
        ];
    }
}
