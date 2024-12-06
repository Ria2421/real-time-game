<?php

namespace Database\Seeders;

// use Illuminate\Database\Console\Seeds\WithoutModelEvents;
use Illuminate\Database\Seeder;

class DatabaseSeeder extends Seeder
{
    /**
     * Seed the application's database.
     */
    public function run(): void
    {
        // マスターデータ(初期データ)挿入
        $this->call(AccountsTableSeeder::class);    // アカウント
        $this->call(UsersTableSeeder::class);       // ユーザー
    }
}
