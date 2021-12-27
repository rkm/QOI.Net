using System.IO;

using NUnit.Framework;

namespace QOI.Net.Tests
{
    public class QOIDecoderTests
    {
        [Test]
        public void SingleEmptyPixelWithAlpha()
        {
            var expected = new byte[] { 0, 0, 0, 0 };

            var input = new byte[] {
                0x71, 0x6f, 0x69, 0x66,                         // magic
                0x00, 0x00, 0x00, 0x01,                         // width
                0x00 ,0x00, 0x00, 0x01,                         // height
                0x04,                                           // channels
                0x01,                                           // colourspace
                /* --- Data blocks ---------------------------- */
                0x00,                                           // QOI_OP_INDEX (0)
                /* --- End of data blocks --------------------  */
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, // padding
            };

            var output = QOIDecoder.Decode(input, out var width, out var height, out var channels, out var colourSpace);

            Assert.AreEqual(1, width);
            Assert.AreEqual(1, height);
            Assert.AreEqual(4, channels);
            Assert.AreEqual(1, colourSpace);
            Assert.AreEqual(expected, output.ToArray());
        }

        [Test]
        public void SingleNonEmptyPixelWithAlpha()
        {
            var expected = new byte[] { 1, 1, 1, 1 };

            var input = new byte[] {
                0x71, 0x6f, 0x69, 0x66,                         // magic
                0x00, 0x00, 0x00, 0x01,                         // width
                0x00, 0x00, 0x00, 0x01,                         // height
                0x04,                                           // channels
                0x01,                                           // colourspace
                /* --- Data blocks ---------------------------- */
                0xff,                                           // QOI_OP_RGBA
                  0x01, 0x01, 0x01, 0x01,                       // r,g,b,a
                /* --- End of data blocks --------------------  */
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, // padding
            };

            var output = QOIDecoder.Decode(input, out var width, out var height, out var channels, out var colourSpace);

            Assert.AreEqual(1, width);
            Assert.AreEqual(1, height);
            Assert.AreEqual(4, channels);
            Assert.AreEqual(1, colourSpace);
            Assert.AreEqual(expected, output.ToArray());
        }

        [Test]
        public void SingleEmptyPixelWithoutAlpha()
        {
            var expected = new byte[] { 0, 0, 0 };

            var input = new byte[] {
                0x71, 0x6f, 0x69, 0x66,                         // magic
                0x00, 0x00, 0x00, 0x01,                         // width
                0x00 ,0x00, 0x00, 0x01,                         // height
                0x03,                                           // channels
                0x01,                                           // colourspace
                /* --- Data blocks ---------------------------- */
                0xc0,                                           // QOI_OP_RUN (0)
                /* --- End of data blocks --------------------  */
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, // padding
            };

            var output = QOIDecoder.Decode(input, out var width, out var height, out var channels, out var colourSpace);

            Assert.AreEqual(1, width);
            Assert.AreEqual(1, height);
            Assert.AreEqual(3, channels);
            Assert.AreEqual(1, colourSpace);
            Assert.AreEqual(expected, output.ToArray());
        }

        [Test]
        public void SingleNonEmptyPixelWithoutAlpha()
        {
            var expected = new byte[] { 1, 1, 1 };

            var input = new byte[] {
                0x71, 0x6f, 0x69, 0x66,                         // magic
                0x00, 0x00, 0x00, 0x01,                         // width
                0x00 ,0x00, 0x00, 0x01,                         // height
                0x03,                                           // channels
                0x01,                                           // colourspace
                /* --- Data blocks ---------------------------- */
                0x7f,                                           // QOI_OP_DIFF (1,1,1)
                /* --- End of data blocks --------------------  */
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, // padding
            };

            var output = QOIDecoder.Decode(input, out var width, out var height, out var channels, out var colourSpace);

            Assert.AreEqual(1, width);
            Assert.AreEqual(1, height);
            Assert.AreEqual(3, channels);
            Assert.AreEqual(1, colourSpace);
            Assert.AreEqual(expected, output.ToArray());
        }

        [Test]
        public void RandBytes()
        {
            var reference = File.ReadAllBytes("randbytes.bin");

            var input = File.ReadAllBytes("randbytes.qoi");

            var output = QOIDecoder.Decode(input, out var width, out var height, out var channels, out var colourSpace);

            Assert.AreEqual(5, width);
            Assert.AreEqual(5, height);
            Assert.AreEqual(4, channels);
            Assert.AreEqual(1, colourSpace);
            Assert.AreEqual(reference, output.ToArray());
        }

        [Test]
        public void EdinburghCastle()
        {
            var reference = File.ReadAllBytes("scotland-edinburgh-castle-day.bin");

            var input = File.ReadAllBytes("scotland-edinburgh-castle-day.qoi");

            var output = QOIDecoder.Decode(input, out var width, out var height, out var channels, out var colourSpace);

            Assert.AreEqual(730, width);
            Assert.AreEqual(487, height);
            Assert.AreEqual(4, channels);
            Assert.AreEqual(1, colourSpace);
            Assert.AreEqual(reference, output.ToArray());
        }
    }
}
