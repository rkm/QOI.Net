[![main](https://github.com/rkm/QOI.Net/actions/workflows/main.yml/badge.svg)](https://github.com/rkm/QOI.Net/actions/workflows/main.yml)
[![pre-commit.ci status](https://results.pre-commit.ci/badge/github/rkm/QOI.Net/main.svg)](https://results.pre-commit.ci/latest/github/rkm/QOI.Net/main)
[![Nuget](https://img.shields.io/nuget/v/Rkm.QOI.Net)](https://www.nuget.org/packages/Rkm.QOI.Net)
![GitHub](https://img.shields.io/github/license/rkm/QOI.Net)

# QOI.Net

A C# implementation of the [Quite OK Image Format](https://qoiformat.org/) for
Fast, Lossless Compression.

## Installation

```console
$ dotnet add package Rkm.QOI.Net
```

## Usage

### Encoder

```cs
var data = File.ReadAllBytes("scotland-edinburgh-castle-day.bin");

var qoi = QOIEncoder.Encode(
    data,
    width: 730,
    height: 487,
    channels: 4,
    colourSpace: 1,
    out var outLen
);
```

### Decoder

```cs
var qoi = File.ReadAllBytes("scotland-edinburgh-castle-day.qoi");

var decoded = QOIDecoder.Decode(
    qoi,
    out var width,
    out var height,
    out var channels,
    out var colourSpace
);
```

## Development

### pre-commit

This repo uses [pre-commit] to manage and automatically run a series of linters
and code formatters. After cloning the repo and changing into the directory, run
this once to setup pre-commit.

```shell
$ pip install pre-commit
$ pre-commit install
```

This will then run the checks before every commit. It can also be run manually
at any time:

```shell
$ pre-commit run [<hook>] (--all-files | --files <file list>)
```

<!-- Links -->

[pre-commit]: https://pre-commit.com
