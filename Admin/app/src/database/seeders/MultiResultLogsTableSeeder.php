<?php

namespace Database\Seeders;

use App\Models\MultiResultLog;
use Illuminate\Database\Seeder;

class MultiResultLogsTableSeeder extends Seeder
{
    public function run(): void
    {
        MultiResultLog::create([
            'play_log_id' => 1,
            'user_id' => 1,
            'rank' => 1,
            'car_id' => 1
        ]);

        MultiResultLog::create([
            'play_log_id' => 1,
            'user_id' => 2,
            'rank' => 2,
            'car_id' => 1
        ]);

        MultiResultLog::create([
            'play_log_id' => 1,
            'user_id' => 3,
            'rank' => 3,
            'car_id' => 1
        ]);

        MultiResultLog::create([
            'play_log_id' => 1,
            'user_id' => 4,
            'rank' => 4,
            'car_id' => 1
        ]);
    }
}
