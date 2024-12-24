<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration {
    public function up(): void
    {
        Schema::create('solo_play_logs', function (Blueprint $table) {
            $table->id();
            $table->unsignedInteger('stage_id')->index();
            $table->unsignedInteger('user_id')->index();
            $table->unsignedInteger('car_type_id');
            $table->unsignedInteger('clear_time_msec');
            $table->timestamps();
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('solo_play_logs');
    }
};
