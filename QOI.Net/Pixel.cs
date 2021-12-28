using System.Runtime.InteropServices;

namespace QOI.Net
{
    [StructLayout(LayoutKind.Explicit)]
    internal ref struct Pixel
    {
        [FieldOffset(0)] public byte r;
        [FieldOffset(1)] public byte g;
        [FieldOffset(2)] public byte b;
        [FieldOffset(3)] public byte a;

        [FieldOffset(0)] public int value;
    }
}
