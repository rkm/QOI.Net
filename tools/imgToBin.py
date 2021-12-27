#!/usr/bin/env python3

import argparse
import json
import os
import sys

from PIL import Image
from PIL import ImageChops

def main():

    parser = argparse.ArgumentParser()
    parser.add_argument("input")
    args = parser.parse_args()

    img = Image.open(args.input)

    base_name = os.path.splitext(args.input)[0]

    print(f"size: {img.size}")
    print(f"info: {img.info}")

    with open(f"{base_name}.json", "w") as f:
        json.dump( img.info, f)
        f.write("\n")

    as_bytes = img.convert("RGBA").tobytes()

    outname = os.path.splitext(args.input)[0] + ".bin"
    print(f"Writing to {outname}")
    with open(outname, "wb") as f:
        f.write(as_bytes)

    return 0

if __name__ == "__main__":
	raise SystemExit(main())
