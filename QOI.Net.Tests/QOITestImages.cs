using System.Collections.Generic;
using System.IO.Compression;
using System.IO;

namespace QOI.Net.Tests
{

    public class QOITestImage
    {
        private const string DIR_NAME = "qoi_test_images";

        public readonly string Name;
        public uint Width;
        public uint Height;
        public int Channels;

        public static readonly List<QOITestImage> TestImages =
            new List<QOITestImage>
            {
                new QOITestImage($"{DIR_NAME}/dice", 800, 600, 4),
                new QOITestImage($"{DIR_NAME}/kodim10", 512, 768, 3),
                new QOITestImage($"{DIR_NAME}/kodim23", 768, 512, 3),
                new QOITestImage($"{DIR_NAME}/qoi_logo", 448, 220, 4),
                new QOITestImage($"{DIR_NAME}/testcard", 256, 256, 4),
                new QOITestImage($"{DIR_NAME}/wikipedia_008", 1152, 858, 3),
            };

        static QOITestImage()
        {
            if (!Directory.Exists(DIR_NAME))
                ZipFile.ExtractToDirectory($"img/{DIR_NAME}.zip", ".");
        }

        private QOITestImage(string name, uint width, uint height, int channels)
        {
            Name = name;
            Width = width;
            Height = height;
            Channels = channels;
        }
    }
}
