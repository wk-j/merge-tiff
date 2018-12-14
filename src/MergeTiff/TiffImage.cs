using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace MergeTiff {
    public class TiffImage {
        private int GetNumberOfPages(Image image) {
            var guid = image.FrameDimensionsList[0];
            var dimension = new FrameDimension(guid);
            var page = image.GetFrameCount(dimension);
            return page;
        }

        ImageCodecInfo GetTiffInfo() =>
            ImageCodecInfo.GetImageDecoders().Where(x => x.MimeType == "image/tiff").First();

        public byte[] Merge(IEnumerable<byte[]> images) {

            if (images.Count() == 1) return images.First();

            var info = GetTiffInfo();
            var enc = Encoder.SaveFlag;

            var ep = new EncoderParameters(1);
            ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.MultiFrame);

            using (var merged = new MemoryStream()) {
                var first = images.First();
                var ms = new MemoryStream(first);
                var pages = (Bitmap)Image.FromStream(ms);
                pages.Save(merged, info, ep);

                foreach (var (x, i) in images.Skip(1).Select((x, i) => (x, i))) {

                    ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.FrameDimensionPage);

                    var bm = (Bitmap)Image.FromStream(merged);
                    pages.SaveAdd(bm, ep);

                    if (i == images.Count() - 1) {
                        ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.Flush);
                        pages.SaveAdd(ep);
                    }
                }
                return merged.ToArray();
            }
        }
    }
}
