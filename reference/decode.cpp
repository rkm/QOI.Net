#include <fstream>
#include <iostream>
#include <string>

#define QOI_IMPLEMENTATION
#include "qoi.h"


int main(int argc, char* argv[])
{

    if (argc != 3)
    {
        printf("Usage: %s input-qoi channels\n", argv[0]);
        return 1;
    }

    std::string path(argv[1]);
    auto channels = atoi(argv[2]);

    qoi_desc desc;

    void* decoded_image = qoi_read(path.c_str(), &desc, channels);
    if (decoded_image == NULL)
    {
        printf("qoi_read failed\n");
        return 1;
    }

    auto idx = path.find_last_of(".");
    std::ofstream out_file(path.substr(0, idx) + ".decoded.bin", std::ios::binary);
    int decoded_length = desc.width * desc.height * channels;
    out_file.write((char*)decoded_image, decoded_length);
    out_file.close();

    free(decoded_image);

    return 0;
}
