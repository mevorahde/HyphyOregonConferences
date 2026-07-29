using System.Buffers.Binary;
using System.IO.Compression;

namespace HyphyOregon.ConferenceGenerator.Tests;

internal sealed class PngInspector
{
    private static readonly byte[] Signature = [137, 80, 78, 71, 13, 10, 26, 10];

    private PngInspector(int width, int height, byte colorType, byte[] pixels)
    {
        Width = width;
        Height = height;
        ColorType = colorType;
        Pixels = pixels;
    }

    public int Width { get; }

    public int Height { get; }

    public byte ColorType { get; }

    public byte[] Pixels { get; }

    public static PngInspector Read(string path) => Read(File.ReadAllBytes(path));

    public static PngInspector Read(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        if (!data.AsSpan(0, Signature.Length).SequenceEqual(Signature))
        {
            throw new InvalidDataException("The file is not a PNG.");
        }

        int width = 0;
        int height = 0;
        byte colorType = 0;
        var compressed = new MemoryStream();
        int offset = Signature.Length;

        while (offset < data.Length)
        {
            int length = checked((int)BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(offset, 4)));
            ReadOnlySpan<byte> type = data.AsSpan(offset + 4, 4);
            ReadOnlySpan<byte> content = data.AsSpan(offset + 8, length);
            offset = checked(offset + 12 + length);

            if (type.SequenceEqual("IHDR"u8))
            {
                width = checked((int)BinaryPrimitives.ReadUInt32BigEndian(content[..4]));
                height = checked((int)BinaryPrimitives.ReadUInt32BigEndian(content.Slice(4, 4)));
                if (content[8] != 8 || content[12] != 0)
                {
                    throw new InvalidDataException("Only non-interlaced 8-bit PNGs are supported.");
                }

                colorType = content[9];
                if (colorType != 6)
                {
                    throw new InvalidDataException("The PNG is not RGBA.");
                }
            }
            else if (type.SequenceEqual("IDAT"u8))
            {
                compressed.Write(content);
            }
            else if (type.SequenceEqual("IEND"u8))
            {
                break;
            }
        }

        if (width <= 0 || height <= 0)
        {
            throw new InvalidDataException("The PNG dimensions are invalid.");
        }

        compressed.Position = 0;
        using var decompressed = new MemoryStream();
        using (var zlib = new ZLibStream(compressed, CompressionMode.Decompress, leaveOpen: true))
        {
            zlib.CopyTo(decompressed);
        }

        byte[] scanlines = decompressed.ToArray();
        const int bytesPerPixel = 4;
        int stride = checked(width * bytesPerPixel);
        if (scanlines.Length != checked((stride + 1) * height))
        {
            throw new InvalidDataException("The PNG scanline length is invalid.");
        }

        byte[] pixels = new byte[checked(stride * height)];
        for (int row = 0; row < height; row++)
        {
            int scanlineOffset = row * (stride + 1);
            int outputOffset = row * stride;
            byte filter = scanlines[scanlineOffset];

            for (int column = 0; column < stride; column++)
            {
                byte encoded = scanlines[scanlineOffset + 1 + column];
                byte left = column >= bytesPerPixel
                    ? pixels[outputOffset + column - bytesPerPixel]
                    : (byte)0;
                byte above = row > 0
                    ? pixels[outputOffset - stride + column]
                    : (byte)0;
                byte upperLeft = row > 0 && column >= bytesPerPixel
                    ? pixels[outputOffset - stride + column - bytesPerPixel]
                    : (byte)0;

                pixels[outputOffset + column] = filter switch
                {
                    0 => encoded,
                    1 => unchecked((byte)(encoded + left)),
                    2 => unchecked((byte)(encoded + above)),
                    3 => unchecked((byte)(encoded + ((left + above) / 2))),
                    4 => unchecked((byte)(encoded + Paeth(left, above, upperLeft))),
                    _ => throw new InvalidDataException("The PNG uses an unknown filter.")
                };
            }
        }

        return new PngInspector(width, height, colorType, pixels);
    }

    public ReadOnlySpan<byte> PixelAt(int x, int y)
    {
        int offset = checked(((y * Width) + x) * 4);
        return Pixels.AsSpan(offset, 4);
    }

    private static byte Paeth(byte left, byte above, byte upperLeft)
    {
        int prediction = left + above - upperLeft;
        int leftDistance = Math.Abs(prediction - left);
        int aboveDistance = Math.Abs(prediction - above);
        int upperLeftDistance = Math.Abs(prediction - upperLeft);

        if (leftDistance <= aboveDistance && leftDistance <= upperLeftDistance)
        {
            return left;
        }

        return aboveDistance <= upperLeftDistance ? above : upperLeft;
    }
}
