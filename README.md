# ColorCounter

クリップボード上のイメージから色をカウントする
カウントの対象色はHsl色相(Hue)の30度刻みの１２色　＋　白黒

## 開発環境

* OS:Windows 10 Pro
* IDE:Visual Studio Community 2022

## フレームワークなど

* WindowsForm
* .net 6.0

## 動作環境

.NET 6.0 sdKのインストールが必要です

https://dotnet.microsoft.com/ja-jp/download/dotnet/6.0


## 備考

Hsl色相(Hue)から対象色の前後15度をカウントしていく
0,0,0　255,255,255は透過色としてカウント対象外とする。(チェックボックス)

## 参考にしたサイト

https://www.peko-step.com/html/hsv.html

https://blog.ch3cooh.jp/entry/20120817/1345181943


コントラスト比計算

https://ja.wikipedia.org/wiki/Help:配色のコントラスト比

https://demo.grapecity.com/wijmo/demos/Input/InputColor/ContrastChecker/purejs

## 色配分の調整

各ピクセルの色をRGB からHsl形式に変換します。
Hslは、色相(Hue)・彩度(Saturation)・明度(Value・Brightness)で色を表現します。
   
色のシミュレーションは下記のサイトを利用しました。
https://www.peko-step.com/tool/hslrgb.html

本ドキュメントでは黒白以外の色についてHueから色を求めるため、色相色と表現します。
色相色は範囲は対象色の前後15度の計30度の範囲とします。
例）赤　RGB(255,0,0)　の場合、Hueは345～0(360)～15の範囲となります。

下記条件にて各色に加算をおこなっています。
1. Brightness >= 0.95(C1_Brightness) の場合は、単色：白として1を加算します。
2. Brightness <= 0.10(C2_Brightness) の場合は、単色：黒として1を加算します。
3. Saturation <= 0.0(C3_Saturation) かつ Hue <= 0.0(C3_Hue)の場合は、灰色（白黒の混色）として　白に0.5(C3_Value)と黒に0.5(1-C3_Value)を加算します。
4. Brightness が 0.20(C4_Brightness_Low) ～ 0.85(C4_Brightness_High)の場合は、単色：色相色として1を加算します。
5. Saturation <= 0.60(C5_Saturation) 場合は、色相色と白または黒の混色として色相色に0.75(C5_Value)、白または黒に0.25(1-C5_Value)を加算します。。
6. 1～5に該当しない場合、2色の色相色の混色として、主となる色相色に0.75(C6_Value)、近接する側の色相色に0.25(1-C6_Value)を加算します。
