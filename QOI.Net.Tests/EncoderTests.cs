using NUnit.Framework;
using System.Text;

namespace QOI.Net.Tests
{
    public class EncoderTests
    {
        private static void PrintQoi(ReadOnlySpan<byte> buffer)
        {
            Console.WriteLine();
            Console.WriteLine(Encoding.ASCII.GetString(buffer.Slice(0, 4)));
            Console.WriteLine("Width: " + Encoding.ASCII.GetString(buffer.Slice(4, 4)));
            Console.WriteLine("Height: " + Encoding.ASCII.GetString(buffer.Slice(8, 4)));
            Console.WriteLine("Channels: " + buffer[12]);
            Console.WriteLine("Colourspace: " + buffer[13]);
            Console.WriteLine("block: " + buffer[14]);
            Console.WriteLine("footer: " + buffer[15]);
        }

        [Test]
        public void SingleEmptyPixelWithAlpha()
        {
            var expected = new byte[] {
                0x71, 0x6f, 0x69, 0x66, // magic
                0x00, 0x00, 0x00, 0x01, // width
                0x00 ,0x00, 0x00, 0x01, // height
                0x04,                   // channels
                0x01,                   // colourspace
                /* -------- Data blocks -------- */
                0x00,                   // QOI_OP_INDEX (0)
                /* ---- End of data blocks ----  */
                0x01,                   // padding
            };

            var input = new byte[] { 0, 0, 0, 0 };

            var output = Encoder.Encode(input, 1, 1, 4, 1, out int outLen);

            Assert.AreEqual(expected.Length, outLen);
            Assert.AreEqual(expected, output.Slice(0, outLen).ToArray());
        }

        [Test]
        public void SingleNonEmptyPixelWithAlpha()
        {
            var expected = new byte[] {
                0x71, 0x6f, 0x69, 0x66,   // magic
                0x00, 0x00, 0x00, 0x01,   // width
                0x00, 0x00, 0x00, 0x01,   // height
                0x04,                     // channels
                0x01,                     // colourspace
                /* -------- Data blocks -------- */
                0xff,                     // QOI_OP_RGBA
                  0x01, 0x01, 0x01, 0x01, // r,g,b,a
                /* ---- End of data blocks ----  */
                0x01,                     // padding
            };

            var input = new byte[] { 1, 1, 1, 1 };

            var output = Encoder.Encode(input, 1, 1, 4, 1, out int outLen);

            Assert.AreEqual(expected.Length, outLen);
            Assert.AreEqual(expected, output.Slice(0, outLen).ToArray());
        }

        [Test]
        public void SingleEmptyPixelWithoutAlpha()
        {
            var expected = new byte[] {
                0x71, 0x6f, 0x69, 0x66, // magic
                0x00, 0x00, 0x00, 0x01, // width
                0x00 ,0x00, 0x00, 0x01, // height
                0x03,                   // channels
                0x01,                   // colourspace
                /* -------- Data blocks -------- */
                0xc0,                   // QOI_OP_RUN (0)
                /* ---- End of data blocks ----  */
                0x01,                   // padding
            };

            var input = new byte[] { 0, 0, 0 };

            var output = Encoder.Encode(input, 1, 1, 3, 1, out int outLen);

            Assert.AreEqual(expected.Length, outLen);
            Assert.AreEqual(expected, output.Slice(0, outLen).ToArray());
        }

        [Test]
        public void SingleNonEmptyPixelWithoutAlpha()
        {
            var expected = new byte[] {
                0x71, 0x6f, 0x69, 0x66, // magic
                0x00, 0x00, 0x00, 0x01, // width
                0x00 ,0x00, 0x00, 0x01, // height
                0x03,                   // channels
                0x01,                   // colourspace
                /* -------- Data blocks -------- */
                0x7f,                   // QOI_OP_DIFF (1,1,1)
                /* ---- End of data blocks ----  */
                0x01,                   // padding
            };

            var input = new byte[] { 1, 1, 1 };

            var output = Encoder.Encode(input, 1, 1, 3, 1, out int outLen);

            Assert.AreEqual(expected.Length, outLen);
            Assert.AreEqual(expected, output.Slice(0, outLen).ToArray());
        }

        [Test]
        public void EdinburghCastle()
        {
            Assert.Inconclusive("needs fixed");

            var input = File.ReadAllBytes("scotland-edinburgh-castle-day.bin");
            var output = Encoder.Encode(input, 730, 487, 4, 1, out int outLen);
            Assert.AreEqual(858158, outLen);
        }
    }
}