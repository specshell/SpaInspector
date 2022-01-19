namespace SpaFileReader;

[PublicAPI]
public static class SpaFile
{
    private const short YUnitFlag = 3;
    private const short InterferogramFlag = 102;
    private const short BackgroundInterferogramFlag = 103;
    private const int SizeAddress = 0x000126;
    private const int PositionsAddress = 0x000130;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> ReadYUnitAsSpanByte(byte[] bytes)
    {
        var span = bytes.AsSpan();
        var yUnitAsBytes = ReadYUnitAsSpanByte(ref span);
        return yUnitAsBytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<float> ReadYUnitAsSpanFloat(byte[] bytes)
    {
        var span = bytes.AsSpan();
        var yUnitAsSpanFloat = ReadYUnitAsSpanFloat(ref span);
        return yUnitAsSpanFloat;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<float> ReadInterferogramAsSpanFloat(byte[] bytes)
    {
        var span = bytes.AsSpan();
        var yUnitAsSpanFloat = ReadInterferogramAsSpanFloat(ref span);
        return yUnitAsSpanFloat;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<float> ReadBackgroundInterogramAsSpanFloat(byte[] bytes)
    {
        var span = bytes.AsSpan();
        var yUnitAsSpanFloat = ReadBackgroundInterogramAsSpanFloat(ref span);
        return yUnitAsSpanFloat;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float[] ReadYUnitAsFloatArray(byte[] bytes)
    {
        var span = bytes.AsSpan();
        var yUnitAsFloatArray = ReadYUnitAsSpanFloat(ref span);
        return yUnitAsFloatArray.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<float> ReadYUnitAsSpanFloat(string file)
    {
        var bytes = File.ReadAllBytes(file);
        var yUnitAsSpanFloat = ReadYUnitAsSpanFloat(bytes);
        return yUnitAsSpanFloat;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float[] ReadYUnitAsFloatArray(string file)
    {
        var bytes = File.ReadAllBytes(file);
        var yUnitAsFloatArray = ReadYUnitAsFloatArray(bytes);
        return yUnitAsFloatArray;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double[] ReadYUnitAsDoubleArray(string file)
    {
        var bytes = File.ReadAllBytes(file);
        var yUnitAsDoubleArray = ReadYUnitAsDoubleArray(bytes);
        return yUnitAsDoubleArray;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<float[]> ReadYUnitAsFloatArrayAsync(string file)
    {
        var bytes = await File.ReadAllBytesAsync(file);
        var yUnitAsFloatArray = ReadYUnitAsFloatArray(bytes);
        return yUnitAsFloatArray;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<double[]> ReadYUnitAsDoubleArrayAsync(string file)
    {
        var bytes = await File.ReadAllBytesAsync(file);
        var yUnitAsDoubleArray = ReadYUnitAsDoubleArray(bytes);
        return yUnitAsDoubleArray;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<double> ReadYUnitAsSpanDouble(string file)
    {
        var bytes = File.ReadAllBytes(file);
        var yUnitAsSpanFloat = ReadYUnitAsDoubleArray(bytes);
        return yUnitAsSpanFloat;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<double> ReadYUnitAsSpanDouble(byte[] bytes)
    {
        var span = bytes.AsSpan();
        var yUnitAsDoubles = ReadYUnitAsSpanDouble(ref span);
        return yUnitAsDoubles;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double[] ReadYUnitAsDoubleArray(byte[] bytes)
    {
        var span = bytes.AsSpan();
        var yUnitAsDoubleArray = ReadYUnitAsDoubleArray(ref span);
        return yUnitAsDoubleArray;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> ReadYUnitAsSpanByte(ref Span<byte> bytes)
    {
        var (start, length) = ReadSpecificFlagPositions(ref bytes, YUnitFlag);
        var yUnitAsBytes = bytes.Slice(start, length);
        return yUnitAsBytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> WriteYUnitAsSpanByte(Span<byte> bytes, Span<byte> yUnits)
    {
        return WriteByteAt(ref bytes, YUnitFlag, yUnits);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<float> ReadYUnitAsSpanFloat(ref Span<byte> bytes)
    {
        var yUnitAsBytes = ReadYUnitAsSpanByte(ref bytes);
        var yUnitAsFloats = MemoryMarshal.Cast<byte, float>(yUnitAsBytes);
        // The floats in SPA file are stored from 4000 to 700 wave numbers
        // Reversed to read from 700 to 4000 like how wave numbers are read in CSV.
        yUnitAsFloats.Reverse();
        return yUnitAsFloats;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> WriteYUnitAsSpanFloat(Span<byte> bytes, Span<float> yUnit)
    {
        return WriteByteAt(ref bytes, YUnitFlag, MemoryMarshal.Cast<float, byte>(yUnit));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> ReadInterferogramAsSpanByte(ref Span<byte> bytes)
    {
        var (start, length) = ReadSpecificFlagPositions(ref bytes, InterferogramFlag);
        var yUnitAsBytes = bytes.Slice(start, length);
        return yUnitAsBytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<float> ReadInterferogramAsSpanFloat(ref Span<byte> bytes)
    {
        var yUnitAsBytes = ReadInterferogramAsSpanByte(ref bytes);
        var yUnitAsFloats = MemoryMarshal.Cast<byte, float>(yUnitAsBytes);
        return yUnitAsFloats;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> ReadBackgroundInterferogramAsSpanByte(ref Span<byte> bytes)
    {
        var (start, length) = ReadSpecificFlagPositions(ref bytes, BackgroundInterferogramFlag);
        var yUnitAsBytes = bytes.Slice(start, length);
        return yUnitAsBytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<float> ReadBackgroundInterogramAsSpanFloat(ref Span<byte> bytes)
    {
        var yUnitAsBytes = ReadBackgroundInterferogramAsSpanByte(ref bytes);
        var yUnitAsFloats = MemoryMarshal.Cast<byte, float>(yUnitAsBytes);
        return yUnitAsFloats;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double[] ReadYUnitAsDoubleArray(ref Span<byte> bytes)
    {
        var yUnitAsFloats = ReadYUnitAsSpanFloat(ref bytes);
        var yUnitAsDoubles = new double[yUnitAsFloats.Length];
        for (var i = 0; i < yUnitAsDoubles.Length; i++)
        {
            yUnitAsDoubles[i] = yUnitAsFloats[i];
        }

        return yUnitAsDoubles;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<double> ReadYUnitAsSpanDouble(ref Span<byte> bytes)
    {
        var yUnitAsFloats = ReadYUnitAsSpanFloat(ref bytes);
        var yUnitAsDoubles = new double[yUnitAsFloats.Length];
        for (var i = 0; i < yUnitAsDoubles.Length; i++)
        {
            yUnitAsDoubles[i] = yUnitAsFloats[i];
        }

        return yUnitAsDoubles;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int start, int length) ReadSpecificFlagPositions(ref Span<byte> bytes, short expectedFlag)
    {
        var size = bytes.ReadInt16At(SizeAddress);

        var position = PositionsAddress;

        for (var i = 0; i <= size; i++)
        {
            short flag = bytes.ReadByteAt(position);
            if (flag == expectedFlag)
            {
                var start = bytes.ReadInt32At(position + 2);
                var length = bytes.ReadInt32At(position + 6);
                return (start, length);
            }

            position += 16;
        }

        return (0, 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ReadByteAt(this ref Span<byte> data, int offset)
    {
        return offset + 1 > data.Length
            ? (byte)0
            : MemoryMarshal.Read<byte>(data.Slice(offset, 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ReadInt16At(this ref Span<byte> data, int offset)
    {
        return offset + 2 > data.Length
            ? (short)0
            : MemoryMarshal.Read<short>(data.Slice(offset, 2));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadInt32At(this ref Span<byte> data, int offset)
    {
        return offset + 4 > data.Length
            ? 0
            : MemoryMarshal.Read<int>(data.Slice(offset, 4));
    }
    private static Span<byte> WriteByteAt(this ref Span<byte> data, short expectedFlag, Span<byte> bytesToWrite)
    {
        var (start, length) = ReadSpecificFlagPositions(ref data, expectedFlag);
        var n = 0;
        for (var i = start; start + length > i; i ++)
        {
            data[i] = bytesToWrite[n];
            n++;
        }
        return data;
    }
}
