using System;
using System.Windows;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Text;

namespace clipbdsv {
        class Program {
        [STAThread]
        static void Main(string[] args) {

            // フォルダー
            string folder = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "ClipbdSv");
            if (!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }

            // ファイル名
            string filename = DateTime.Now.ToString("yyyyMMdd_hhmmssffff") + (new Random()).Next(0, 99).ToString("00");
            Encoding enc = Encoding.GetEncoding("utf-8");

            // 出力形式の選択
            string dtfmt = "";
            if (Clipboard.ContainsData(DataFormats.CommaSeparatedValue))
                dtfmt = "CSV";
            else if (Clipboard.ContainsData(DataFormats.Rtf))
                dtfmt = "RTF";
            else if (Clipboard.ContainsData(DataFormats.Text))
                dtfmt = "TXT";
            else if (Clipboard.ContainsImage())
                dtfmt = "IMG";


            switch (dtfmt) {
                // CSV
                case "CSV":
                    var csv = Clipboard.GetData(DataFormats.CommaSeparatedValue);
                    using (StreamWriter sw = new StreamWriter(Path.Combine(folder, filename + ".csv"), false, enc)) {
                        sw.Write(csv.ToString());
                    }
                    break;

                // rtf
                case "RTF":
                    var rtf = Clipboard.GetData(DataFormats.Rtf);
                    using (StreamWriter sw = new StreamWriter(Path.Combine(folder, filename + ".rtf"), false, Encoding.GetEncoding(932))) {
                        sw.Write(rtf.ToString());
                    }
                    break;

                // UnicodeText形式のデータ
                case "TXT":
                    string str = Clipboard.GetText();
                    using (StreamWriter sw = new StreamWriter(Path.Combine(folder, filename + ".txt"), false, enc)) {
                        sw.Write(str);
                    }
                    break;

                // Bitmapデータ形式のデータ
                case "IMG":
                    using (Bitmap bmp = GetBitmap(Clipboard.GetImage())) {
                        bmp.Save(Path.Combine(folder, filename + ".jpg"), ImageFormat.Jpeg);
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
