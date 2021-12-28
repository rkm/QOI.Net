namespace QOI.Net
{
    internal static class Util
    {
        public const int SEEN_BUFFER_LENGTH = 64;
        public const int HEADER_SIZE = 14;
        public const int PADDING_LENGTH = 1;

        /// <summary>
        /// The string "QOIF"
        /// </summary>
        public static readonly uint QOI_MAGIC = 1903126886;

        public const int QOI_SRGB = 0;
        public const int QOI_LINEAR = 1;

        public const int MAX_RUN_LENGTH = 62;

        public const byte QOI_OP_INDEX = 0x00;
        public const byte QOI_OP_DIFF = 0x40;
        public const byte QOI_OP_LUMA = 0x80;
        public const byte QOI_OP_RUN = 0xc0;
        public const byte QOI_OP_RGB = 0xfe;
        public const byte QOI_OP_RGBA = 0xff;
        public const byte QOI_MASK_2 = 0xc0;

        internal static int PixelHash(Pixel p) => (p.r * 3 + p.g * 5 + p.b * 7 + p.a * 11) % 64;
    }
}
