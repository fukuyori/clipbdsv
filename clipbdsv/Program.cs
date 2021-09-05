using System;
using System.Windows;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Text;
using ZXing;
using CommandLineParser;

namespace clipbdsv {
        class Program {

        class Options {
            [Option('d', "directory", Required = false, HelpText = "Output Folder")]
            public string _folder { get; set; }
            [Option('f', "file", Required = false, HelpText = "Output Filename")]
            public string _filename { get; set; }
            [Option('t', "type", Required = false, HelpText = "Target Type CSV|RTF|TXT|IM")]
            public string _type { get; set; }
            [Option('e', "encode", Required = false, HelpText = "Encode TYpe (Default utf-8)")]
            public string _encode { get; set; }
        }

        [STAThread]
        static void Main(string[] args) {

            var result = Parser.Parse<Options>(args);

            // フォルダー
            string folder;
            if (result.Value._folder == null) {
                folder = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "ClipbdSv");
                try {
                    if (!Directory.Exists(folder)) {
                        Directory.CreateDirectory(folder);
                    }
                } catch {
                    Console.WriteLine("フォルダーが作成できませんでした");
                    Environment.Exit(1);
                }
            } else {
                folder = result.Value._folder;
                DirectoryInfo hoge;
                if (!Directory.Exists(folder)) {
                    try {
                        Directory.CreateDirectory(folder);
                    } catch {
                        Console.WriteLine("フォルダーが作成できませんでした");
                        Environment.Exit(1);
                    }
                }
            }

            // ファイル名
            string filename;
            if (result.Value._filename == null) {
                filename = DateTime.Now.ToString("yyyyMMdd_HHmmssffff") + (new Random()).Next(0, 99).ToString("00");
            } else {
                filename = result.Value._filename;
            }

            // エンコード
            Encoding enc = Encoding.GetEncoding("utf-8");
            if (result.Value._encode != null) {
                try {
                    enc = Encoding.GetEncoding(result.Value._encode);
                } catch {
                    Console.WriteLine($"{result.Value._encode}　は、正しいエンコード名ではありません");
                    Environment.Exit(1);
                }
            }

            // 出力形式の選択
            string dtfmt = "";
            if (result.Value._type == null) {
                if (Clipboard.ContainsData(DataFormats.CommaSeparatedValue))
                    dtfmt = "CSV";
                else if (Clipboard.ContainsData(DataFormats.Rtf))
                    dtfmt = "RTF";
                else if (Clipboard.ContainsData(DataFormats.Text))
                    dtfmt = "TXT";
                else if (Clipboard.ContainsData(DataFormats.Bitmap))
                    dtfmt = "IMG";
            } else {
                dtfmt = result.Value._type.ToUpper();
            }

            switch (dtfmt) {
                // CSV
                case "CSV":
                    if (Clipboard.ContainsData(DataFormats.CommaSeparatedValue)) {
                        var csv = Clipboard.GetData(DataFormats.CommaSeparatedValue);
                        using (StreamWriter sw = new StreamWriter(Path.Combine(folder, filename + ".csv"), false, enc)) {
                            sw.Write(csv.ToString());
                        }
                    }
                    break;

                // rtf
                case "RTF":
                    if (Clipboard.ContainsData(DataFormats.Rtf)) {
                        var rtf = Clipboard.GetData(DataFormats.Rtf);
                        using (StreamWriter sw = new StreamWriter(Path.Combine(folder, filename + ".rtf"), false, Encoding.GetEncoding(932))) {
                            sw.Write(rtf.ToString());
                        }
                    }
                    break;

                // UnicodeText形式のデータ
                case "TXT":
                    if (Clipboard.ContainsData(DataFormats.Text)) {
                        string str = Clipboard.GetText();
                        using (StreamWriter sw = new StreamWriter(Path.Combine(folder, filename + ".txt"), false, enc)) {
                            sw.Write(str);
                        }
                    }
                    break;

                // Bitmapデータ形式のデータ
                case "IMG":
                    if (Clipboard.ContainsData(DataFormats.Bitmap)) {
                        using (Bitmap bmp = GetBitmap(Clipboard.GetImage())) {
                            bmp.Save(Path.Combine(folder, filename + ".jpg"), ImageFormat.Jpeg);

                            // QRコード読み取り
                            BarcodeReader qrreader = new BarcodeReader();
                            Result qrresult = qrreader.Decode(new Bitmap(bmp));

                            if (qrresult != null) {
                                using (StreamWriter sw = new StreamWriter(Path.Combine(folder, filename + ".txt"), false, enc)) {
                                    sw.Write(qrresult);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Bitmap <-> BitmapSource converter
        /// https://gist.github.com/nashby/916300
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        static Bitmap GetBitmap(BitmapSource source) {
            Bitmap bmp = new Bitmap
            (
              source.PixelWidth,
              source.PixelHeight,
              System.Drawing.Imaging.PixelFormat.Format32bppPArgb
            );

            BitmapData data = bmp.LockBits
            (
                new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size),
                ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb
            );

            source.CopyPixels
            (
              Int32Rect.Empty,
              data.Scan0,
              data.Height * data.Stride,
              data.Stride
            );

            bmp.UnlockBits(data);

            return bmp;
        }
    }
}
