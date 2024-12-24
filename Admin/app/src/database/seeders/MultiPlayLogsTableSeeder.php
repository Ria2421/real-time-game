<?php

namespace Database\Seeders;

use App\Models\MultiPlayLog;
use Illuminate\Database\Seeder;

class MultiPlayLogsTableSeeder extends Seeder
{
    public function run(): void
    {
        MultiPlayLog::create([
            'room_name' => 'sample1',
            'matching_type' => 1,
            'stage_id' => 1,
        ]);

        MultiPlayLog::create([
            'room_name' => 'sample2',
            'matching_type' => 1,
            'stage_id' => 1,
        ]);

        MultiPlayLog::create([
            'room_name' => 'sample3',
            'matching_type' => 1,
            'stage_id' => 1,
        ]);
    }
}
