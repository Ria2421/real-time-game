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

        SoloPlayData::create([
            'stage_id' => 1,
            'user_id' => 1,
            'car_type_id' => 1,
            'clear_time_msec' => 40000,
            'ghost_data' => "",
        ]);

        SoloPlayData::create([
            'stage_id' => 1,
            'user_id' => 2,
            'car_type_id' => 1,
            'clear_time_msec' => 38000,
            'ghost_data' => "",
        ]);

        SoloPlayData::create([
            'stage_id' => 1,
            'user_id' => 3,
            'car_type_id' => 1,
            'clear_time_msec' => 42000,
            'ghost_data' => "",
        ]);

        SoloPlayData::create([
            'stage_id' => 1,
            'user_id' => 4,
            'car_type_id' => 1,
            'clear_time_msec' => 37500,
            'ghost_data' => "",
        ]);

        // ---------------------------
        // ステージ2

        SoloPlayData::create([
            'stage_id' => 2,
            'user_id' => 1,
            'car_type_id' => 1,
            'clear_time_msec' => 91000,
            'ghost_data' => "",
        ]);

        SoloPlayData::create([
            'stage_id' => 2,
            'user_id' => 2,
            'car_type_id' => 1,
            'clear_time_msec' => 92000,
            'ghost_data' => "",
        ]);

        SoloPlayData::create([
            'stage_id' => 2,
            'user_id' => 3,
            'car_type_id' => 1,
            'clear_time_msec' => 93000,
            'ghost_data' => "",
        ]);

        SoloPlayData::create([
            'stage_id' => 2,
            'user_id' => 4,
            'car_type_id' => 1,
            'clear_time_msec' => 94000,
            'ghost_data' => "",
        ]);

        // ---------------------------
        // ステージ3

        SoloPlayData::create([
            'stage_id' => 3,
            'user_id' => 1,
            'car_type_id' => 1,
            'clear_time_msec' => 101000,
            'ghost_data' => "",
        ]);

        SoloPlayData::create([
            'stage_id' => 3,
            'user_id' => 2,
            'car_type_id' => 1,
            'clear_time_msec' => 102000,
            'ghost_data' => "",
        ]);

        SoloPlayData::create([
            'stage_id' => 3,
            'user_id' => 3,
            'car_type_id' => 1,
            'clear_time_msec' => 103000,
            'ghost_data' => "",
        ]);

        SoloPlayData::create([
            'stage_id' => 3,
            'user_id' => 4,
            'car_type_id' => 1,
            'clear_time_msec' => 104000,
            'ghost_data' => "",
        ]);
    }
}
