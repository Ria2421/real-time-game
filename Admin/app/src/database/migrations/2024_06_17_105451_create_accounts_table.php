<?php
//-------------------------------------------------------------------------
// アカウントマイグレーション [2024_06_17_105451_create_accounts_table.php]
// Author:Kenta Nakamoto
//  Data :2024/06/17
// Update:2024/08/22
//-------------------------------------------------------------------------

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration {
    /**
     * Run the migrations.
     */
    public function up(): void
    {
        Schema::create('accounts', function (Blueprint $table) {
            $table->id();   // idカラム
            $table->string('name', 20);     // nameカラム
            $table->string('password', 128);    // passwordカラム
            $table->timestamps();   // created_at,update_at

            $table->unique('name');     // nameにユニーク制約設定
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('accounts');
    }
};
