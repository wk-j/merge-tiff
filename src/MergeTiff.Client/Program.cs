using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MergeTiff {

    class Program {
        static void Main(string[] args) {
            var i1 = File.ReadAllBytes("resources/Thailand.1.tiff");
            var i2 = File.ReadAllBytes("resources/8.tiff");
            var images = new[] {
                    i1,
                    i2
                };

            var tiff = new TiffImage();

            var bytes = tiff.Merge(images);
            File.WriteAllBytes(".output/Hello.tiff", bytes);
        }
    }
}
