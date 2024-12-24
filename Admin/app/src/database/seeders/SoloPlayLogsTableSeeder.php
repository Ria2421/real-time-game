<?php

namespace Database\Seeders;

use App\Models\SoloPlayLog;
use Illuminate\Database\Seeder;

class SoloPlayLogsTableSeeder extends Seeder
{
    public function run(): void
    {
        SoloPlayLog::create([
            'stage_id' => 1,
            'user_id' => 1,
            'car_type_id' => 1,
            'clear_time_msec' => 40000
        ]);

        SoloPlayLog::create([
            'stage_id' => 1,
            'user_id' => 2,
            'car_type_id' => 1,
            'clear_time_msec' => 38000
        ]);

        SoloPlayLog::create([
            'stage_id' => 1,
            'user_id' => 3,
            'car_type_id' => 1,
            'clear_time_msec' => 42000
        ]);

        SoloPlayLog::create([
            'stage_id' => 1,
            'user_id' => 4,
            'car_type_id' => 1,
            'clear_time_msec' => 37500
        ]);
    }
}
