#include <iostream>
#include <fstream>
#include <vector>

#define QOI_IMPLEMENTATION
#include "qoi.h"


int main(int argc, char* argv[])
{
    if (argc != 5)
    {
        printf("Usage: %s input-bin width height channels\n", argv[0]);
        return 1;
    }

    auto path = argv[1];
    auto width = (unsigned int) atoi(argv[2]);
    auto height = (unsigned int) atoi(argv[3]);
    auto channels = (unsigned int) atoi(argv[4]);

    std::ifstream in_file(path, std::ios::binary);
    std::streamsize size = in_file.tellg();
    in_file.seekg(0, std::ios::beg);

    auto bytes = width * height * channels;
    char* buffer = new char[bytes];
    if (!in_file.read(buffer, bytes))
    {
        printf("error reading into buffer\n");
        return 1;
    }

    in_file.close();

    qoi_desc desc = {
        .width      = width,
        .height     = height,
        .channels   = channels,
        .colorspace = QOI_LINEAR,
    };

    int encoded_length;
    void* encoded_image = qoi_encode(buffer, &desc, &encoded_length);
    if (encoded_image == NULL)
    {
        printf("qoi_encode failed\n");
        return 1;
    }

    delete[] buffer;

    // write out

    std::ofstream out_file("out.qoi", std::ios::binary);
    out_file.write((char*)encoded_image, encoded_length);
    out_file.close();

    free(encoded_image);

    return 0;
}
