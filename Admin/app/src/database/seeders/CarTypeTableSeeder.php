<?php

namespace Database\Seeders;

use App\Models\CarType;
use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\Hash;

class CarTypeTableSeeder extends Seeder
{
    public function run(): void
    {
        CarType::create([
            'type' => 'normal'
        ]);
        CarType::create([
            'type' => 'speed'
        ]);
        CarType::create([
            'type' => 'dirt'
        ]);
    }
}
