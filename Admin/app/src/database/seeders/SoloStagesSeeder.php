<?php

namespace Database\Seeders;

use App\Models\SoloStage;
use Illuminate\Database\Seeder;

class SoloStagesSeeder extends Seeder
{
    public function run(): void
    {
        SoloStage::create([
            'name' => 'スタンダード'
        ]);
    }
}
