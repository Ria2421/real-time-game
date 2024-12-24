<?php

namespace Database\Seeders;

use App\Models\Car;
use Illuminate\Database\Seeder;

class CarTableSeeder extends Seeder
{
    public function run(): void
    {
        Car::create([
            'type_id' => 1,
            'name' => 'N1',
            'price' => 200
        ]);

        Car::create([
            'type_id' => 1,
            'name' => 'N2',
            'price' => 200
        ]);

        Car::create([
            'type_id' => 1,
            'name' => 'N3',
            'price' => 200
        ]);

        Car::create([
            'type_id' => 2,
            'name' => 'S1',
            'price' => 200
        ]);

        Car::create([
            'type_id' => 2,
            'name' => 'S2',
            'price' => 200
        ]);

        Car::create([
            'type_id' => 2,
            'name' => 'S3',
            'price' => 200
        ]);

        Car::create([
            'type_id' => 3,
            'name' => 'D1',
            'price' => 200
        ]);

        Car::create([
            'type_id' => 3,
            'name' => 'D2',
            'price' => 200
        ]);

        Car::create([
            'type_id' => 3,
            'name' => 'D3',
            'price' => 200
        ]);
    }
}
