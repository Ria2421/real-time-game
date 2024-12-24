<?php

namespace Database\Seeders;

use App\Models\HaveCar;
use Illuminate\Database\Seeder;

class HaveCarsSeeder extends Seeder
{
    public function run(): void
    {
        HaveCar::create([
            'user_id' => 1,
            'car_id' => 1,
        ]);

        HaveCar::create([
            'user_id' => 2,
            'car_id' => 1,
        ]);

        HaveCar::create([
            'user_id' => 3,
            'car_id' => 1,
        ]);

        HaveCar::create([
            'user_id' => 4,
            'car_id' => 1,
        ]);

        HaveCar::create([
            'user_id' => 1,
            'car_id' => 2,
        ]);

        HaveCar::create([
            'user_id' => 2,
            'car_id' => 3,
        ]);

        HaveCar::create([
            'user_id' => 3,
            'car_id' => 4,
        ]);

        HaveCar::create([
            'user_id' => 4,
            'car_id' => 5,
        ]);
    }
}
