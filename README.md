clipbdsv (Clipboard Data Saver)
====

Overview

## Description

クリップボードに入れたデータをファイルに保存するコマンドライン・プログラムです。

プログラムを実行すると、標準でデスクトップの ClipbdSv フォルダに保存します。
ファイル作成時の日時がファイル名になり、ＣＳＶファイル、リッチテキスト・ファイル、テキストファイル、画像ファイル（JPEG）として保存されます。
画像ファイルは、ＱＲコードが含まれている場合、ＱＲコードの内容がテキストファイルに保存されます。

以下のコマンドライン・オプションがあります。

-f 出力ファイル名を指定（拡張子なし）

-d 出力先フォルダーの指定

-t 出力形式の指定（csv|rtf|txt|img)

-e 文字エンコード（標準 utf-8）

## Usage

clipbdsv

clipbdsv -d C:\temp -f clipimg -t img

clipbdsv -t txt -e Shift_JIS

clipbdsv --help

## Requirement

.Net Framework 4.7.2 で動作します。

## Install

Windows10上で、実行ファイル1つで動作するコマンドライン・プログラムです。
ZIPを解凍し、clipbdsv.exe をそのまま実行できます。
どこかのフォルダーに保存してタスクバーにピン止めしておくと便利です。

## Licence

[MIT](https://github.com/fukuyori/clipbdsv/blob/master/LICENSE.txt)

[GitHub nashby/BitmapFromBitmapSource.cs](https://gist.github.com/nashby/916300)

[GitHub wertrain/command-line-parser-cs](https://github.com/wertrain/command-line-parser-cs) public domain

[NuGet Michael Jahn/ZXing.Net 0.16.6](https://github.com/micjahn/ZXing.Net/) Apache-2.0

## Author

SPUMONI.ORG

twitter: @Spumo

Github: https://github.com/fukuyori/clipbdsv

Qiita: https://qiita.com/spumoni/items/cce3c3c5e24751b283bf