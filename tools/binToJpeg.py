#!/usr/bin/env python3
import argparse
import json
import os

from PIL import Image


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("input")
    parser.add_argument("width")
    parser.add_argument("height")
    args = parser.parse_args()

    with open(args.input, "rb") as f1:
        as_bytes = f1.read()

    base_name = os.path.splitext(args.input)[0]
    with open(f"{base_name}.json") as f2:
        info = json.load(f2)

    img = Image.frombytes("RGBA", (int(args.width), int(args.height)), as_bytes)
    img.convert("RGB").save(f"{base_name}-out.jpeg", **info)

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
