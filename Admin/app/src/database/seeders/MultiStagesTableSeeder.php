<?php

namespace Database\Seeders;

use App\Models\MultiStage;
use Illuminate\Database\Seeder;

class MultiStagesTableSeeder extends Seeder
{
    public function run(): void
    {
        MultiStage::create([
            'name' => 'スタンダード'
        ]);
    }
}
