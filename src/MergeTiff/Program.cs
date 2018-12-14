using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MergeTiff {
    class TiffImage {
        private int GetNumberOfPages(Image image) {
            var guid = image.FrameDimensionsList[0];
            var dimension = new FrameDimension(guid);
            var page = image.GetFrameCount(dimension);
            return page;
        }
        public void MergeTiff(string[] sourceFiles) {
            string[] sa = sourceFiles;
            ImageCodecInfo info = null;
            foreach (ImageCodecInfo ice in ImageCodecInfo.GetImageEncoders()) {
                if (ice.MimeType == "image/tiff") {
                    info = ice;
                }
            }

            Encoder enc = Encoder.SaveFlag;

            EncoderParameters ep = new EncoderParameters(1);
            ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.MultiFrame);

            Bitmap pages = null;

            int frame = 0;

            foreach (string s in sa) {
                if (frame == 0) {
                    MemoryStream ms = new MemoryStream(File.ReadAllBytes(s));
                    pages = (Bitmap)Image.FromStream(ms);
                    pages.Save(s, info, ep);
                } else {
                    ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.FrameDimensionPage);

                    try {
                        MemoryStream mss = new MemoryStream(File.ReadAllBytes(s));
                        Bitmap bm = (Bitmap)Image.FromStream(mss);
                        pages.SaveAdd(bm, ep);
                    } catch (Exception e) {
                    }
                }

                if (frame == sa.Length - 1) {
                    ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.Flush);
                    pages.SaveAdd(ep);
                }
                frame++;
            }
        }


        public byte[] Merge(IEnumerable<byte[]> images) {
            using (var merge = new MemoryStream()) {
                foreach (var item in images) {
                    using (var image = Image.FromStream(new MemoryStream(item))) {
                        var page = GetNumberOfPages(image);
                        foreach (var id in image.FrameDimensionsList) {
                            for (var index = 0; index < page; index++) {
                                var currentFrame = new FrameDimension(id);
                                image.SelectActiveFrame(currentFrame, index);
                                image.Save(merge, ImageFormat.Tiff);
                            }
                        }
                    }
                }
                merge.Position = 0;
                return merge.ToArray();
            }
        }
    }

    class Program {
        static void Main(string[] args) {
            var img = File.ReadAllBytes("resources/Thailand.tiff");
            var images = new[] {
                img,
                img,
                img
            };

            var tiff = new TiffImage();
            // var merged = tiff.Merge(images);
            // File.WriteAllBytes(".output/Thailand.tiff", merged);

            var files = new[] {
                "resources/Thailand.1.tiff",
                "resources/Thailand.2.tiff",
            };

            // System.NotImplementedException: Not implemented.
            tiff.MergeTiff(files);
        }
    }
}
