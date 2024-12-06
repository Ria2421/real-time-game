<?php

namespace Database\Seeders;

use App\Models\User;
use Illuminate\Database\Seeder;

class UsersTableSeeder extends Seeder
{
    public function run(): void
    {
        User::create([
            'name' => 'jobi1',
            'token' => "test"
        ]);
        User::create([
            'name' => 'jobi2',
            'token' => "test"
        ]);
        User::create([
            'name' => 'jobi3',
            'token' => "test"
        ]);
        User::create([
            'name' => 'jobi4',
            'token' => "test"
        ]);
    }
}
