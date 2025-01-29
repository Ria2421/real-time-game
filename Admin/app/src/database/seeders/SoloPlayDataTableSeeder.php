<?php

namespace Database\Seeders;

use App\Models\SoloPlayData;
use Illuminate\Database\Seeder;

class SoloPlayDataTableSeeder extends Seeder
{
    public function run(): void
    {
        // ---------------------------
        // ステージ1

        for ($i = 0; $i < 10; $i++) {
            SoloPlayData::create([
                'stage_id' => 1,
                'user_id' => $i + 1,
                'car_type_id' => 1,
                'clear_time_msec' => 50000 + $i * 1000,
                'ghost_data' => "",
            ]);
        }

        // ---------------------------
        // ステージ2
        for ($i = 0; $i < 10; $i++) {
            SoloPlayData::create([
                'stage_id' => 2,
                'user_id' => $i + 1,
                'car_type_id' => 1,
                'clear_time_msec' => 65000 + $i * 1000,
                'ghost_data' => "",
            ]);
        }
    }
}
