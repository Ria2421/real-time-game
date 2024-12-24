<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration {
    public function up(): void
    {
        Schema::create('multi_play_logs', function (Blueprint $table) {
            $table->id();
            $table->string('room_name', 128);
            $table->unsignedTinyInteger('matching_type');
            $table->unsignedInteger('stage_id')->index();
            $table->timestamps();
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('multi_play_logs');
    }
};
