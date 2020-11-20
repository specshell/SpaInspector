using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SpaInspector
{
    public sealed record Spectra
    {
        private const int WaveNumberStart = 0x000244;
        private const int WaveNumberEnd = 0x000240;
        private const int WaveNumberSize = 0x000234;
        private const int Gain = 0x000368;
        private const int OpticalVelocity = 0x00036C;
        private const int SignalStrength = 0x000278;
        private const int FileDescription = 0x00001E;
        private const int SpectreDescription = 0x000380;

        private readonly int _absorbanceEnd;
        private readonly int _interferogramBackgroundEnd;
        private readonly int _interferogramAbsorbanceEnd;
        private readonly bool _isInterferogramPresent;

        public Spectra(byte[] content)
        {
            Content = content;
            for (var i = 0; content.Length > i; i += 4)
            {
                var adrValue = BitConverter.ToInt32(content, i);
                //Looking for a 32bit address containing  "9E00CC02" or "9E00DE02" - if not found default address is set to 0x408
                if (adrValue == 46923934 || adrValue == 48103582)
                {
                    _absorbanceEnd = i;
                }

                //Managing interferograms
                if (!_isInterferogramPresent && adrValue == 12200)
                {
                    _interferogramAbsorbanceEnd = i;
                    _isInterferogramPresent = true;
                }
                else if (_isInterferogramPresent && adrValue == 12200)
                {
                    _interferogramBackgroundEnd = _interferogramAbsorbanceEnd;
                    _interferogramAbsorbanceEnd = i;
                    break;
                }
            }
        }

        public byte[] Content { get; }

        private float GetFloat(int adr) => BitConverter.ToSingle(Content, adr);

        private int GetInt(int adr) => BitConverter.ToInt32(Content, adr);

        public float GetWaveNumberStart() => GetFloat(WaveNumberStart);

        public float GetWaveNumberEnd() => GetFloat(WaveNumberEnd);

        public int GetWaveNumberSize() => GetInt(WaveNumberSize);

        public float GetGain() => GetFloat(Gain);

        public float GetOpticalVelocity() => GetFloat(OpticalVelocity);

        public float GetSignalStrength() => GetFloat(SignalStrength);

        public string GetSpectreDescription() => GetString(SpectreDescription);

        public string GetFileDescription() => GetString(FileDescription);

        public void SetFileDescription(string str) => SetString(FileDescription, str);

        public float GetWaveResolution() => (GetWaveNumberEnd() - GetWaveNumberStart()) / GetWaveNumberSize();

        public int GetArbsorbanseInterferogramSize() =>
            _interferogramAbsorbanceEnd == 0 ? _interferogramAbsorbanceEnd : GetInt(_interferogramAbsorbanceEnd);

        public int GetBackgroundInterferogramSize() =>
            _interferogramBackgroundEnd == 0 ? _interferogramBackgroundEnd : GetInt(_interferogramBackgroundEnd);

        private string GetString(int adr)
        {
            var stringArray = Content.AsSpan().Slice(adr).ToArray();
            Stream stream = new MemoryStream(stringArray);
            var binaryReader = new BinaryReader(stream, Encoding.UTF8);
            return binaryReader.ReadNullTerminatedString();
        }

        private void SetString(int adr, string str)
        {
            var stringAllocationSize = 0;
            var stringArray = Content.AsSpan().Slice(adr).ToArray();
            using var streamRead = new MemoryStream(stringArray);
            using var binaryReader = new BinaryReader(streamRead, Encoding.UTF8);
            for (var i = -1; stringArray.Length > i; i++)
            {
                var c = binaryReader.ReadChar();
                // Attempt to escape loop once we found the null terminated string.
                if (c == 0)
                {
                    stringAllocationSize = -1;
                }

                // Read to end of assigned String in byte array, so we can safely replace that section of the byte array.
                // Meaning we hit data that does not belong to this section of the byte array
                if (stringAllocationSize != -1 || c == 0) continue;
                stringAllocationSize = i;
                break;
            }

            using var streamWrite = new MemoryStream(Content, adr, stringAllocationSize);
            using var binaryWriter = new BinaryWriter(streamWrite);
            binaryWriter.Write(Encoding.UTF8.GetBytes(str).AsSpan(0, stringAllocationSize));
        }

        public IList<float> GetAbsorbanceArray()
        {
            List<float> absorbanceArray = new();
            for (var i = _absorbanceEnd - (GetWaveNumberSize() * 4); _absorbanceEnd > i; i = i + 4)
            {
                var absorbanceValue = BinaryPrimitives.ReadSingleLittleEndian(Content.AsSpan().Slice(i, 4));
                absorbanceArray.Add(absorbanceValue);
            }

            return absorbanceArray;
        }

        public bool InsertAbsorbanceArray(IList<float> floatArray)
        {
            if (floatArray.Count != GetWaveNumberSize()) return false;
            for (int n = 0, i = _absorbanceEnd - (GetWaveNumberSize() * 4); _absorbanceEnd > i; i = i + 4, n++)
            {
                BinaryPrimitives.WriteSingleLittleEndian(Content.AsSpan().Slice(i, 4), floatArray[n]);
            }

            return true;
        }

        public IList<float> GetArbsorbanseInterferogramArray()
        {
            List<float> arbsorbanseInterferogramArray = new();
            if (_interferogramAbsorbanceEnd == 0) return arbsorbanseInterferogramArray;
            for (var i = _interferogramAbsorbanceEnd - (GetArbsorbanseInterferogramSize() * 4); _interferogramAbsorbanceEnd > i; i += 4)
            {
                var interferogramValue = BitConverter.ToSingle(Content, i);
                arbsorbanseInterferogramArray.Add(interferogramValue);
            }

            return arbsorbanseInterferogramArray;
        }

        public IList<float> GetBackgroundInterferogramArray()
        {
            List<float> backgroundInterferogramArray = new();
            if (_interferogramBackgroundEnd == 0) return backgroundInterferogramArray;
            for (var i = _interferogramBackgroundEnd - (GetBackgroundInterferogramSize() * 4); _interferogramBackgroundEnd > i; i += 4)
            {
                var interferogramValue = BitConverter.ToSingle(Content, i);
                backgroundInterferogramArray.Add(interferogramValue);
            }

            return backgroundInterferogramArray;
        }
    }
}
