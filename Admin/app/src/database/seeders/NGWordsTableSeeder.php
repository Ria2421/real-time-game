<?php

namespace Database\Seeders;

use App\Models\NgWord;
use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\Storage;

class NGWordsTableSeeder extends Seeder
{
    public function run(): void
    {
        // テキストファイルのパス
        $filePath = 'txt/NGWord.txt';

        // Storageファザードを使って読み込む
        $content = Storage::get($filePath);

        // 区切り文字で分割する(改行指定)
        $lines = explode(PHP_EOL, $content);

        foreach ($lines as $line) {
            // 改行を削除して登録する
            NgWord::create([
                'word' => str_replace([PHP_EOL, " "], "", $line)
            ]);
        }
    }
}
