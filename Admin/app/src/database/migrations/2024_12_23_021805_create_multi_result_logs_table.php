<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration {
    public function up(): void
    {
        Schema::create('multi_result_logs', function (Blueprint $table) {
            $table->id();
            $table->unsignedInteger('play_log_id')->index();
            $table->unsignedInteger('user_id')->index();
            $table->string('rank');
            $table->unsignedInteger('car_id');
            $table->timestamps();
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('multi_result_logs');
    }
};
