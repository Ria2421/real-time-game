# CarBoomCrash!!
![構成図](https://lessoniaasstrage.blob.core.windows.net/images/Title.png?raw=true)
<p>スマホ向けのカーレースバトルゲームです。</p>
<p>タイムアタックモードとオンライン対戦モードを実装しました。</p>

+ タイムアタックモード
  + 1人用のモードです。コースを選択し、世界最速を目指します。
  + オンラインランキング・ゴースト機能を実装しました。

+ オンライン対戦モード
  + 4人で遊ぶモードです。他プレイヤーの後ろからぶつかって撃破することで1位を目指します。
  + ステージはサーバー側で抽選を行い、全3種のステージで遊ぶことが出来ます。

# アピールポイント
カーレースゲームには欠かせないゴースト機能の実装・マルチ対戦のリアルタイム通信対応を行いました。

# 開発期間
2024/10～2025/02（5カ月程）

# 使用言語・フレームワーク
+ C# (MagicOnion)
+ PHP (Laravel)

# 構築場所
Azure

# サーバー構成図
![構成図](https://lessoniaasstrage.blob.core.windows.net/images/%E3%82%B7%E3%82%B9%E3%83%86%E3%83%A0%E5%85%A8%E4%BD%93%E6%A7%8B%E6%88%90%E5%9B%B3.png?raw=true)

# フォルダ構成
<pre>
├─Admin
│   └─app          #管理ツールソースフォルダ(Laravel)
│
├─Document         #通信フロー図
│
├─RealTimeClient   #クライアントフォルダ(Unity)
│   ├─Assets
│   │ └─Scripts    #自作スクリプトフォルダ
│   └─Build        #実行フォルダ
│
├─RealTimeServer   #サーバーフォルダ(MagicOnion)
│   ├─Services     #API処理フォルダ
│   └─StremingHubs #リアルタイム通信処理フォルダ
│
└─Shared           #クライアント&サーバー共有クラス
</pre>
