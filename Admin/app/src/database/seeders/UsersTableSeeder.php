<?php

namespace Database\Seeders;

use App\Models\User;
use Illuminate\Database\Seeder;

class UsersTableSeeder extends Seeder
{
    public function run(): void
    {
        for ($i = 0; $i < 10; $i++) {
            User::create([
                'name' => 'Jobi' . ($i + 1),
                'token' => "test"
            ]);
        }
    }
}
