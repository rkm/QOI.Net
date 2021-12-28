using NUnit.Framework;
using System;
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
        public void SingleEmptyPixel()
        {
            var input = new byte[] { 0, 0, 0, 0 };

            var expected = new byte[] {
                0x71, 0x6f, 0x69, 0x66, // qoif
                0x00, 0x00, 0x00, 0x01, // width
                0x00 ,0x00, 0x00, 0x01, // height
                0x04, // channels
                0x01, // colourspace
                // blocks
                0x00, // single byte
                // end blocks
                0x01, // padding
            };

            var output = Encoder.Encode(input, 1, 1, 4, 1, out int outLen);
            Assert.AreEqual(expected.Length, outLen);
            Assert.AreEqual(expected, output.Slice(0, outLen).ToArray());
        }
    }
}