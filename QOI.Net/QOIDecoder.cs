using System;

namespace QOI.Net
{
    public static class QOIDecoder
    {
        public static ReadOnlySpan<byte> Decode(
            ReadOnlySpan<byte> input,
            out uint width,
            out uint height,
            out int channels,
            out int colourSpace
        )
        {
            if (input == null || input.Length < Util.HEADER_SIZE + Util.PADDING_LENGTH)
                throw new ArgumentOutOfRangeException(nameof(input), "Invalid input");

            var inCursor = 0;

            uint Read32(ReadOnlySpan<byte> buffer)
            {
                var a = buffer[inCursor++];
                var b = buffer[inCursor++];
                var c = buffer[inCursor++];
                var d = buffer[inCursor++];
                return (uint)((a << 24) | (b << 16) | (c << 8) | d);
            }

            if (Read32(input) != Util.QOI_MAGIC)
                throw new Exception("QOI_MAGIC not found at start of input buffer");

            width = Read32(input);
            height = Read32(input);
            channels = input[inCursor++];
            colourSpace = input[inCursor++];

            if (width == 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Invalid width");

            if (height == 0 || height >= Util.QOI_PIXELS_MAX / width)
                throw new ArgumentOutOfRangeException(nameof(height), "Invalid height");

            if (channels != 3 && channels != 4)
                throw new ArgumentOutOfRangeException(nameof(channels), "Only 3 (RGB) or 4 (RGBA) channels are supported");

            if (colourSpace != Util.QOI_SRGB && colourSpace != Util.QOI_LINEAR)
                throw new ArgumentOutOfRangeException(nameof(colourSpace), "Only 0 (SRGB) or 1 (Linear) colour spaces are supported");

            var outSize = width * height * channels;
            var output = new byte[outSize];

            var run = 0;
            var chunksLen = input.Length - Util.PADDING_LENGTH;

            Pixel pixel = default;
            pixel.a = 255;

            unsafe
            {
                var index = stackalloc Pixel[Util.SEEN_BUFFER_LENGTH];

                for (var outCursor = 0; outCursor < outSize; outCursor += channels)
                {
                    if (run > 0)
                    {
                        run--;
                    }
                    else if (inCursor < chunksLen)
                    {
                        int b1 = input[inCursor++];

                        if (b1 == Util.QOI_OP_RGB)
                        {
                            pixel.r = input[inCursor++];
                            pixel.g = input[inCursor++];
                            pixel.b = input[inCursor++];
                        }
                        else if (b1 == Util.QOI_OP_RGBA)
                        {
                            pixel.r = input[inCursor++];
                            pixel.g = input[inCursor++];
                            pixel.b = input[inCursor++];
                            pixel.a = input[inCursor++];
                        }
                        else if ((b1 & Util.QOI_MASK_2) == Util.QOI_OP_INDEX)
                        {
                            pixel = index[b1];
                        }
                        else if ((b1 & Util.QOI_MASK_2) == Util.QOI_OP_DIFF)
                        {
                            pixel.r += (byte)(((b1 >> 4) & 0x03) - 2);
                            pixel.g += (byte)(((b1 >> 2) & 0x03) - 2);
                            pixel.b += (byte)((b1 & 0x03) - 2);
                        }
                        else if ((b1 & Util.QOI_MASK_2) == Util.QOI_OP_LUMA)
                        {
                            int b2 = input[inCursor++];
                            int vg = (b1 & 0x3f) - 32;

                            pixel.r += (byte)(vg - 8 + ((b2 >> 4) & 0x0f));
                            pixel.g += (byte)vg;
                            pixel.b += (byte)(vg - 8 + (b2 & 0x0f));
                        }
                        else if ((b1 & Util.QOI_MASK_2) == Util.QOI_OP_RUN)
                        {
                            run = b1 & 0x3f;
                        }

                        index[Util.PixelHash(pixel) % 64] = pixel;
                    }

                    output[outCursor + 0] = pixel.r;
                    output[outCursor + 1] = pixel.g;
                    output[outCursor + 2] = pixel.b;
                    if (channels == 4)
                        output[outCursor + 3] = pixel.a;
                }
            }

            return output;
        }
    }
}
