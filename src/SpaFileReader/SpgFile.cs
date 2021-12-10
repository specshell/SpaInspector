namespace SpaFileReader;

[PublicAPI]
public static class SpgFile
{
    private const short YUnitFlag = 3;
    private const int PositionsAddress = 0x000130;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<float[]> ReadYUnitAsFloatArray(byte[] bytes)
    {
        var span = bytes.AsSpan();
        var yUnitAsFloatArray = ReadAllYUnitsAsFloats(span);
        return yUnitAsFloatArray;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<float[]> ReadYUnitAsFloatArray(string file)
    {
        var bytes = File.ReadAllBytes(file);
        return ReadYUnitAsFloatArray(bytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<double[]> ReadYUnitAsDoubleArray(string file)
    {
        var bytes = File.ReadAllBytes(file);
        return ReadAllYUnitsAsDoubles(bytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<IEnumerable<float[]>> ReadYUnitAsFloatArrayAsync(string file)
    {
        var bytes = await File.ReadAllBytesAsync(file);
        var yUnitAsFloatArray = ReadYUnitAsFloatArray(bytes);
        return yUnitAsFloatArray;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<IEnumerable<double[]>> ReadYUnitAsDoubleArrayAsync(string file)
    {
        var bytes = await File.ReadAllBytesAsync(file);
        return ReadYUnitAsDoubleArray(bytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<double[]> ReadYUnitAsDoubleArray(byte[] bytes)
    {
        var span = bytes.AsSpan();
        return ReadAllYUnitsAsDoubles(span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IEnumerable<float[]> ReadAllYUnitsAsFloats(Span<byte> bytes)
    {
        var position = PositionsAddress;
        short flag = 1;
        var yunits = new List<float[]>();
        while (flag != 0)
        {
            flag = bytes.ReadByteAt(position);
            if (flag == YUnitFlag)
            {
                var start = bytes.ReadInt32At(position + 2);
                var length = bytes.ReadInt32At(position + 6);
                var yUnitAsBytes = bytes.Slice(start, length);
                var yUnitAsFloats = MemoryMarshal.Cast<byte, float>(yUnitAsBytes);
                // The floats in SPA file are stored from 4000 to 700 wave numbers
                // Reversed to read from 700 to 4000 like how wave numbers are read in CSV.
                yUnitAsFloats.Reverse();
                yunits.Add(yUnitAsFloats.ToArray());
            }

            position += 16;
        }

        return yunits;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IEnumerable<double[]> ReadAllYUnitsAsDoubles(Span<byte> bytes)
    {
        var position = PositionsAddress;
        short flag = 1;
        var yunits = new List<double[]>();
        while (flag != 0)
        {
            flag = bytes.ReadByteAt(position);
            if (flag == YUnitFlag)
            {
                var start = bytes.ReadInt32At(position + 2);
                var length = bytes.ReadInt32At(position + 6);
                var yUnitAsBytes = bytes.Slice(start, length);
                var yUnitAsFloats = MemoryMarshal.Cast<byte, float>(yUnitAsBytes);
                // The floats in SPA file are stored from 4000 to 700 wave numbers
                // Reversed to read from 700 to 4000 like how wave numbers are read in CSV.
                yUnitAsFloats.Reverse();
                var doubles = new double[yUnitAsFloats.Length];
                for (var i = 0; i < yUnitAsFloats.Length; i++)
                {
                    doubles[i] = yUnitAsFloats[i];
                }
                yunits.Add(doubles);
            }

            position += 16;
        }

        return yunits;
    }
}
