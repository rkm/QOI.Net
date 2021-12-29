using System.IO;

using NUnit.Framework;

namespace QOI.Net.Tests
{
    public class QOIEncoderTests
    {
        [Test]
        public void SingleEmptyPixelWithAlpha()
        {
            var expected = new byte[] {
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

            var input = new byte[] { 0, 0, 0, 0 };

            var output = QOIEncoder.Encode(input, 1, 1, 4, 1, out var outLen);

            Assert.AreEqual(expected.Length, outLen);
            Assert.AreEqual(expected, output[..outLen].ToArray());
        }

        [Test]
        public void SingleNonEmptyPixelWithAlpha()
        {
            var expected = new byte[] {
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

            var input = new byte[] { 1, 1, 1, 1 };

            var output = QOIEncoder.Encode(input, 1, 1, 4, 1, out var outLen);

            Assert.AreEqual(expected.Length, outLen);
            Assert.AreEqual(expected, output[..outLen].ToArray());
        }

        [Test]
        public void SingleEmptyPixelWithoutAlpha()
        {
            var expected = new byte[] {
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

            var input = new byte[] { 0, 0, 0 };

            var output = QOIEncoder.Encode(input, 1, 1, 3, 1, out var outLen);

            Assert.AreEqual(expected.Length, outLen);
            Assert.AreEqual(expected, output[..outLen].ToArray());
        }

        [Test]
        public void SingleNonEmptyPixelWithoutAlpha()
        {
            var expected = new byte[] {
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

            var input = new byte[] { 1, 1, 1 };

            var output = QOIEncoder.Encode(input, 1, 1, 3, 1, out var outLen);

            Assert.AreEqual(expected.Length, outLen);
            Assert.AreEqual(expected, output[..outLen].ToArray());
        }

        [Test]
        public void RandBytes()
        {
            var reference = File.ReadAllBytes("randbytes.qoi");

            var input = File.ReadAllBytes("randbytes.bin");

            var output = QOIEncoder.Encode(input, 5, 5, 4, 1, out var outLen);

            Assert.AreEqual(reference.Length, outLen);
            Assert.AreEqual(reference, output[..outLen].ToArray());
        }

        [Test]
        public void EdinburghCastle()
        {
            var reference = File.ReadAllBytes("scotland-edinburgh-castle-day.qoi");

            var input = File.ReadAllBytes("scotland-edinburgh-castle-day.bin");

            var output = QOIEncoder.Encode(input, 730, 487, 4, 1, out var outLen);

            Assert.AreEqual(reference.Length, outLen);
            Assert.AreEqual(reference, output[..outLen].ToArray());
        }

        [Test]
        public void EncodeQOITestImages()
        {
            foreach (var testImage in QOITestImage.TestImages)
            {
                var reference = File.ReadAllBytes($"{testImage.Name}.qoi");

                var input = File.ReadAllBytes($"{testImage.Name}.bin");

                var output = QOIEncoder.Encode(
                    input,
                    testImage.Width,
                    testImage.Height,
                    testImage.Channels,
                    0,
                    out var outLen
                );

                Assert.AreEqual(reference.Length, outLen, testImage.Name);
                Assert.AreEqual(reference, output[..outLen].ToArray(), testImage.Name);
            }
        }
    }
}
