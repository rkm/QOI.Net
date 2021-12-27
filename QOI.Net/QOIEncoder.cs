using System;

namespace QOI.Net
{
    public static class QOIEncoder
    {
        public static ReadOnlySpan<byte> Encode(
            ReadOnlySpan<byte> input,
            uint width,
            uint height,
            int channels,
            int colourSpace,
            out int outLen
        )
        {
            if (input == null || input.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(input), "Invalid input");

            if (width == 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Invalid width");

            if (height == 0)
                throw new ArgumentOutOfRangeException(nameof(height), "Invalid height");

            if (channels != 3 && channels != 4)
                throw new ArgumentOutOfRangeException(nameof(channels), "Only 3 (RGB) or 4 (RGBA) channels are supported");

            if (colourSpace != Util.QOI_SRGB && colourSpace != Util.QOI_LINEAR)
                throw new ArgumentOutOfRangeException(nameof(colourSpace), "Only 0 (SRGB) or 1 (Linear) colour spaces are supported");

            bool haveAlpha = (channels == 4);
            var maxSize = width * height * (channels + 1) + Util.HEADER_SIZE + Util.PADDING_LENGTH;
            var output = new byte[maxSize];

            var outCursor = 0;

            void Write32(uint value)
            {
                output[outCursor++] = (byte)((value & 0xff000000) >> 24);
                output[outCursor++] = (byte)((value & 0x00ff0000) >> 16);
                output[outCursor++] = (byte)((value & 0x0000ff00) >> 8);
                output[outCursor++] = (byte)((value & 0x000000ff) >> 0);
            }

            Write32(Util.QOI_MAGIC);
            Write32(width);
            Write32(height);
            output[outCursor++] = (byte)channels;
            output[outCursor++] = (byte)colourSpace;

            Pixel previous = default;
            previous.a = 255;

            Pixel pixel = default;
            byte run = 0;
            var finalPixelIdx = input.Length - channels;

            unsafe
            {
                var index = stackalloc Pixel[Util.SEEN_BUFFER_LENGTH];

                for (var inCursor = 0; inCursor < input.Length; inCursor += channels)
                {
                    pixel.r = input[inCursor + 0];
                    pixel.g = input[inCursor + 1];
                    pixel.b = input[inCursor + 2];

                    if (channels == 4)
                        pixel.a = input[inCursor + 3];
                    else
                        pixel.a = previous.a;

                    if (pixel.value == previous.value)
                    {
                        run++;
                        if (run == Util.MAX_RUN_LENGTH || inCursor == finalPixelIdx)
                        {
                            output[outCursor++] = (byte)(Util.QOI_OP_RUN | (run - 1));
                            run = 0;
                        }
                    }
                    else
                    {
                        if (run > 0)
                        {
                            output[outCursor++] = (byte)(Util.QOI_OP_RUN | (run - 1));
                            run = 0;
                        }

                        var hash = Util.PixelHash(pixel);

                        if (index[hash].value == pixel.value)
                        {
                            output[outCursor++] = (byte)(Util.QOI_OP_INDEX | hash);
                        }
                        else
                        {
                            index[hash] = pixel;

                            if (pixel.a == previous.a)
                            {
                                var vr = (sbyte)(pixel.r - previous.r);
                                var vg = (sbyte)(pixel.g - previous.g);
                                var vb = (sbyte)(pixel.b - previous.b);

                                var vg_r = (sbyte)(vr - vg);
                                var vg_b = (sbyte)(vb - vg);

                                if (
                                    vr > -3 && vr < 2 &&
                                    vg > -3 && vg < 2 &&
                                    vb > -3 && vb < 2
                                )
                                {
                                    output[outCursor++] = (byte)(Util.QOI_OP_DIFF | (vr + 2) << 4 | (vg + 2) << 2 | (vb + 2));
                                }
                                else if (
                                    vg_r > -9 && vg_r < 8 &&
                                    vg > -33 && vg < 32 &&
                                    vg_b > -9 && vg_b < 8
                                )
                                {
                                    output[outCursor++] = (byte)(Util.QOI_OP_LUMA | (vg + 32));
                                    output[outCursor++] = (byte)((vg_r + 8) << 4 | (vg_b + 8));
                                }
                                else
                                {
                                    output[outCursor++] = Util.QOI_OP_RGB;
                                    output[outCursor++] = pixel.r;
                                    output[outCursor++] = pixel.g;
                                    output[outCursor++] = pixel.b;
                                }
                            }
                            else
                            {
                                output[outCursor++] = Util.QOI_OP_RGBA;
                                output[outCursor++] = pixel.r;
                                output[outCursor++] = pixel.g;
                                output[outCursor++] = pixel.b;
                                output[outCursor++] = pixel.a;
                            }
                        }
                    }

                    previous = pixel;
                }
            }

            outCursor += 7;
            output[outCursor++] = 1;
            outLen = outCursor;

            return output;
        }
    }
}
