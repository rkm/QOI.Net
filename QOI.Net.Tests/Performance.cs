using System;
using System.Diagnostics;
using System.IO;

using NUnit.Framework;

namespace QOI.Net.Tests
{
    public class Performance
    {
        private const int ITERATIONS = 10_000;

        [Test]
        public void EncodePerf()
        {
            var input = File.ReadAllBytes("scotland-edinburgh-castle-day.bin");

            ReadOnlySpan<byte> output = default;

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < ITERATIONS; ++i)
            {
                output = QOIEncoder.Encode(input, 730, 487, 4, 1, out var outLen);
            }

            Assert.NotZero(output[0]);

            var time = sw.Elapsed;
            Console.WriteLine($"Encode: {time}");
        }

        [Test]
        public void DecodePerf()
        {
            var input = File.ReadAllBytes("scotland-edinburgh-castle-day.qoi");

            ReadOnlySpan<byte> output = default;

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < ITERATIONS; ++i)
            {
                output = QOIDecoder.Decode(input, out var _, out var _, out var _, out var _);
            }

            Assert.NotZero(output[0]);

            var time = sw.Elapsed;
            Console.WriteLine($"Decode: {time}");
        }
    }
}
