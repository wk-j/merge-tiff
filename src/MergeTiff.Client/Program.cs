using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MergeTiff {

    class Program {
        static void Main(string[] args) {
            var img = File.ReadAllBytes("resources/Thailand.1.tiff");
            var images = new[] {
                    img,
                    img,
                    img
                };

            var tiff = new TiffImage();

            var bytes = tiff.Merge(images);
            File.WriteAllBytes(".output/Hello.tiff", bytes);
        }
    }
}
