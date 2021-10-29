using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SpaFileReader
{
    [PublicAPI]
    public static class SpaFile
    {
        private static readonly ArrayPool<byte> Pool = ArrayPool<byte>.Shared;
        private const short YUnitFlag = 3;
        private const int PositionsAddress = 304;

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
        public static float[] ReadYUnitAsFloatArray(byte[] bytes)
        {
            var span = bytes.AsSpan();
            var yUnitAsFloatArray = ReadYUnitAsSpanFloat(ref span);
            return yUnitAsFloatArray.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<float> ReadYUnitAsSpanFloat(string file)
        {
            Span<byte> flagSpan = stackalloc byte[16];

            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.RandomAccess);
            fileStream.Seek(PositionsAddress, SeekOrigin.Begin);
            byte flag;
            do
            {
                fileStream.Read(flagSpan);
                flag = flagSpan.ReadByteAt(0);
            } while (flag != YUnitFlag);

            var startPosition = flagSpan.ReadInt32At(2);
            var size = flagSpan.ReadInt32At(6);
            var bytes = new byte[size];
            var bytesSpan = bytes.AsSpan(0, size);

            fileStream.Seek(startPosition - fileStream.Position, SeekOrigin.Current);
            fileStream.Read(bytesSpan);
            fileStream.Dispose();

            var yUnitAsFloatSpan = MemoryMarshal.Cast<byte, float>(bytesSpan);
            yUnitAsFloatSpan.Reverse();
            return yUnitAsFloatSpan;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float[] ReadYUnitAsFloatArray(string file)
        {
            Span<byte> flagSpan = stackalloc byte[16];

            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.RandomAccess);
            fileStream.Seek(PositionsAddress, SeekOrigin.Begin);
            byte flag;
            do
            {
                fileStream.Read(flagSpan);
                flag = flagSpan.ReadByteAt(0);
            } while (flag != YUnitFlag);

            var startPosition = flagSpan.ReadInt32At(2);
            var size = flagSpan.ReadInt32At(6);
            var bytes = Pool.Rent(size);
            var bytesSpan = bytes.AsSpan(0, size);

            fileStream.Seek(startPosition - fileStream.Position, SeekOrigin.Current);
            fileStream.Read(bytesSpan);
            fileStream.Dispose();

            var yUnitAsFloatSpan = MemoryMarshal.Cast<byte, float>(bytesSpan);
            yUnitAsFloatSpan.Reverse();
            var yUnitAsFloatArray = yUnitAsFloatSpan.ToArray();
            Pool.Return(bytes);
            return yUnitAsFloatArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double[] ReadYUnitAsDoubleArray(string file)
        {
            Span<byte> flagSpan = stackalloc byte[16];

            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.RandomAccess);
            fileStream.Seek(PositionsAddress, SeekOrigin.Begin);
            byte flag;
            do
            {
                fileStream.Read(flagSpan);
                flag = flagSpan.ReadByteAt(0);
            } while (flag != YUnitFlag);

            var startPosition = flagSpan.ReadInt32At(2);
            var size = flagSpan.ReadInt32At(6);
            var bytes = Pool.Rent(size);
            var bytesSpan = bytes.AsSpan(0, size);

            fileStream.Seek(startPosition - fileStream.Position, SeekOrigin.Current);
            fileStream.Read(bytesSpan);
            fileStream.Dispose();

            var yUnitAsFloatSpan = MemoryMarshal.Cast<byte, float>(bytesSpan);
            yUnitAsFloatSpan.Reverse();
            var length = yUnitAsFloatSpan.Length;
            var yUnitAsDoubleArray = new double[length];
            for (var i = 0; i < length; i++)
            {
                yUnitAsDoubleArray[i] = yUnitAsFloatSpan[i];
            }

            Pool.Return(bytes);
            return yUnitAsDoubleArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<float[]> ReadYUnitAsFloatArrayAsync(string file)
        {
            // spa files are so small that true async file read does not make sense...
            //
            return Task.FromResult(ReadYUnitAsFloatArray(file));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<double[]> ReadYUnitAsDoubleArrayAsync(string file)
        {
            return Task.FromResult(ReadYUnitAsDoubleArray(file));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<double> ReadYUnitAsSpanDouble(string file)
        {
            var readYUnitAsDoubleSpan = ReadYUnitAsDoubleArray(file).AsSpan();
            return readYUnitAsDoubleSpan;
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
            var position = PositionsAddress;

            byte flag;
            do
            {
                flag = bytes.ReadByteAt(position);
                if (flag == expectedFlag)
                {
                    var start = bytes.ReadInt32At(position + 2);
                    var length = bytes.ReadInt32At(position + 6);
                    return (start, length);
                }

                position += 16;
            } while (flag != expectedFlag) ;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByteAt(this byte[] data, int offset)
        {
            return offset + 1 > data.Length
                ? (byte)0
                : MemoryMarshal.Read<byte>(data.AsSpan(offset, 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16At(this byte[] data, int offset)
        {
            return offset + 2 > data.Length
                ? (short)0
                : MemoryMarshal.Read<short>(data.AsSpan(offset, 2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32At(this byte[] data, int offset)
        {
            return offset + 4 > data.Length
                ? 0
                : MemoryMarshal.Read<int>(data.AsSpan(offset, 4));
        }
    }
}
