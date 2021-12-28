namespace QOI.Net
{
    public static class Decoder
    {
        public static ReadOnlySpan<byte> Decode(
            ReadOnlySpan<byte> input,
            out int width,
            out int height,
            out int channels,
            out int colourSpace
        )
        {
            var inCursor = 0;

            int ReadInt(ReadOnlySpan<byte> buffer)
            {
                var a = buffer[inCursor++];
                var b = buffer[inCursor++];
                var c = buffer[inCursor++];
                var d = buffer[inCursor++];
                return (a << 24) | (b << 16) | (c << 8) | d;
            }

            // TODO(rkm 2021-12-28) Error checking from reference implementation

            if (ReadInt(input) != Util.QOI_MAGIC)
                throw new Exception("QOI_MAGIC not found at start of input buffer");

            width = ReadInt(input);
            height = ReadInt(input);
            channels = input[inCursor++];
            colourSpace = input[inCursor++];

            var outSize = width * height * channels;
            var output = new byte[outSize];

            var run = 0;
            var chunksLen = input.Length - Util.PADDING_LENGTH;

            Pixel pixel = default;
            pixel.r = 0;
            pixel.g = 0;
            pixel.b = 0;
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
