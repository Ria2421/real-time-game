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
        $this->call(AccountsTableSeeder::class);        // アカウント
        $this->call(UsersTableSeeder::class);           // ユーザー
        $this->call(CarTypeTableSeeder::class);         // 車種 (マスター)
        $this->call(CarTableSeeder::class);             // 車 (マスター)
        $this->call(SoloStagesSeeder::class);           // ソロステージ (マスター)
        $this->call(MultiStagesTableSeeder::class);     // マルチステージ (マスター)
        $this->call(HaveCarsSeeder::class);             // 所持車
        $this->call(SoloPlayLogsTableSeeder::class);    // ソロプレイログ
        $this->call(MultiPlayLogsTableSeeder::class);   // マルチプレイログ
        $this->call(MultiResultLogsTableSeeder::class); // マルチリザルトログ
    }
}
