<?php
//-------------------------------------------------------------------------
// ユーザーマイグレーション [2024_06_17_105451_create_have_items_table.php]
// Author:Kenta Nakamoto
//  Data :2024/06/17
// Update:2024/08/22
//-------------------------------------------------------------------------

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration {
    public function up(): void
    {
        Schema::create('users', function (Blueprint $table) {
            $table->id();
            $table->string('name', 128)->unique();  // ユーザー名
            $table->string('token', 128);           // トークン
            $table->timestamps();
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('users');
    }
};
