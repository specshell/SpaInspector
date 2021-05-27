using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpaFileReader
{
    public static class SpaFileReader
    {
        private const short UnitIntensityFlag = 3;
        private const int PositionsAddress = 0x000120;

        public static Span<float> ReadXUnit(byte[] bytes)
        {
            var span = bytes.AsSpan();
            var absorbance = ReadXUnit(ref span);
            return absorbance;
        }

        public static Span<float> ReadXUnit(ref Span<byte> bytes)
        {
            var (start, length) = ReadSpecificFlagPositions(ref bytes, UnitIntensityFlag);
            var spanFloats = ReadFloats(ref bytes, start, length);
            // Floats in SPA file are stored from 4000 to 700 wave numbers
            // Reversed to read from 700 to 4000 like how wave numbers are read in CSV.
            spanFloats.Reverse();
            return spanFloats;
        }

        private static (int start, int length) ReadSpecificFlagPositions(ref Span<byte> bytes, short expectedFlag)
        {
            var position = PositionsAddress;

            byte flag = 1;
            while (flag != expectedFlag)
            {
                flag = bytes.ReadByteAt(position);
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

        public static Span<float> ReadFloats(ref Span<byte> bytes, int start, int length)
        {
            var asBytes = bytes.Slice(start, length);
            var asFloats = MemoryMarshal.Cast<byte, float>(asBytes);
            return asFloats;
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
    }
}
