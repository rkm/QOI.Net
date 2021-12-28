namespace QOI.Net
{
    public static class Encoder
    {
        private static int PixelHash(Pixel p) => (p.r * 3 + p.g * 5 + p.b * 7 + p.a * 11) % 64;

        public static ReadOnlySpan<byte> Encode(
            ReadOnlySpan<byte> input,
            uint width,
            uint height,
            int channels,
            int colourSpace,
            out int outLen
        )
        {
            if (channels != 3 && channels != 4)
                throw new ArgumentOutOfRangeException(nameof(channels), "Only 3 (RGB) or 4 (RGBA) channels are supported.");

            if (colourSpace != Constants.QOI_SRGB && colourSpace != Constants.QOI_LINEAR)
                throw new ArgumentOutOfRangeException(nameof(colourSpace), "Only 0 (SRGB) or 1 (Linear) colour spaces are supported.");

            bool haveAlpha = (channels == 4);
            var maxSize = width * height * (channels + 1) + Constants.HEADER_SIZE + Constants.PADDING_LENGTH;
            var buffer = new byte[maxSize];

            var outCursor = 0;

            void WriteInt(uint value)
            {
                buffer[outCursor++] = (byte)((value & 0xff000000) >> 24);
                buffer[outCursor++] = (byte)((value & 0x00ff0000) >> 16);
                buffer[outCursor++] = (byte)((value & 0x0000ff00) >> 8);
                buffer[outCursor++] = (byte)((value & 0x000000ff) >> 0);
            }

            WriteInt(Constants.QOI_MAGIC);
            WriteInt(width);
            WriteInt(height);
            buffer[outCursor++] = (byte)channels;
            buffer[outCursor++] = (byte)colourSpace;

            Pixel previous = default;
            previous.r = 0;
            previous.g = 0;
            previous.b = 0;
            previous.a = 255;

            Pixel pixel = default;
            byte run = 0;
            var finalPixelIdx = input.Length - channels;

            unsafe
            {
                var index = stackalloc Pixel[Constants.SEEN_BUFFER_LENGTH];

                for (var cursor = 0; cursor < input.Length; cursor += channels)
                {
                    pixel.r = input[cursor + 0];
                    pixel.g = input[cursor + 1];
                    pixel.b = input[cursor + 2];

                    if (channels == 4)
                        pixel.a = input[cursor + 3];
                    else
                        pixel.a = previous.a;

                    if (pixel.value == previous.value)
                    {
                        run++;
                        if (run == Constants.MAX_RUN_LENGTH || cursor == finalPixelIdx)
                        {
                            buffer[outCursor++] = (byte)(Constants.QOI_OP_RUN | (run - 1));
                            run = 0;
                        }
                    }
                    else
                    {
                        if (run > 0)
                        {
                            buffer[outCursor++] = (byte)(Constants.QOI_OP_RUN | (run - 1));
                            run = 0;
                        }

                        var hash = PixelHash(pixel);

                        if (index[hash].value == pixel.value)
                        {
                            buffer[outCursor++] = (byte)(Constants.QOI_OP_INDEX | hash);
                        }
                        else
                        {
                            index[hash] = pixel;

                            if (pixel.a == previous.a)
                            {
                                var vr = (char)(pixel.r - previous.r);
                                var vg = (char)(pixel.g - previous.g);
                                var vb = (char)(pixel.b - previous.b);

                                var vg_r = (char)(vr - vg);
                                var vg_b = (char)(vb - vg);

                                if (
                                    vr > -3 && vr < 2 &&
                                    vg > -3 && vg < 2 &&
                                    vb > -3 && vb < 2
                                )
                                {
                                    buffer[outCursor++] = (byte)(Constants.QOI_OP_DIFF | (vr + 2) << 4 | (vg + 2) << 2 | (vb + 2));
                                }
                                else if (
                                    vg_r > -9 && vg_r < 8 &&
                                    vg > -33 && vg < 32 &&
                                    vg_b > -9 && vg_b < 8
                                )
                                {
                                    buffer[outCursor++] = (byte)(Constants.QOI_OP_LUMA | (vg + 32));
                                    buffer[outCursor++] = (byte)((vg_r + 8) << 4 | (vg_b + 8));
                                }
                                else
                                {
                                    buffer[outCursor++] = Constants.QOI_OP_RGB;
                                    buffer[outCursor++] = pixel.r;
                                    buffer[outCursor++] = pixel.g;
                                    buffer[outCursor++] = pixel.b;
                                }
                            }
                            else
                            {
                                buffer[outCursor++] = Constants.QOI_OP_RGBA;
                                buffer[outCursor++] = pixel.r;
                                buffer[outCursor++] = pixel.g;
                                buffer[outCursor++] = pixel.b;
                                buffer[outCursor++] = pixel.a;
                            }
                        }
                    }

                    previous = pixel;
                }
            }

            buffer[outCursor++] = 1;
            outLen = outCursor;

            return buffer;
        }
    }
}