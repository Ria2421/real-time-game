<?php
//--------------------------------------------------
// アカウントテーブルシーダー [AccountsTableSeeder.php]
// Author:Kenta Nakamoto
//  Data :2024/06/17
// Update:2024/08/22
//---------------------------------------------------

namespace Database\Seeders;

use App\Models\Account;
use Illuminate\Database\Console\Seeds\WithoutModelEvents;
use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\Hash;

class AccountsTableSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {
        Account::create([   // シーダーを使った初期データの登録
            'name' => 'jobi',
            'password' => Hash::make('jobi')
        ]);
    }
}
