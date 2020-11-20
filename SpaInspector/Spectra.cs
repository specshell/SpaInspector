using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SpaInspector
{
    public sealed record Spectra
    {
        private readonly int _absorbanceEnd;
        private readonly int _interferogramBackgroundEnd;
        private readonly int _interferogramAbsorbanceEnd;
        private readonly bool _isInterferogramPresent;
        private List<float> _arbsorbanceInterferogramList = new();
        private List<float> _backgroundInterferogramList = new();
        private List<float> _absorbanceList = new();

        public Spectra(byte[] content)
        {
            Content = content;
            for (var i = 0; content.Length > i; i += 4)
            {
                var adrValue = BinaryPrimitives.ReadInt32LittleEndian(Content.AsSpan().Slice(i, 4));
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

        public float WaveNumberStart => GetFloat(Constants.WaveNumberStart);

        public float WaveNumberEnd => GetFloat(Constants.WaveNumberEnd);

        public int WaveNumberSize => GetInt(Constants.WaveNumberSize);

        public float Gain => GetFloat(Constants.Gain);

        public float OpticalVelocity => GetFloat(Constants.OpticalVelocity);

        public float SignalStrength => GetFloat(Constants.SignalStrength);

        public string SpectreDescription => GetString(Constants.SpectreDescription);

        public string FileDescription => GetString(Constants.FileDescription);

        public void SetFileDescription(string str) => SetString(Constants.FileDescription, str);

        public float WaveResolution => (WaveNumberEnd - WaveNumberStart) / WaveNumberSize;

        public int AbsorbanceInterferogramSize =>
            _interferogramAbsorbanceEnd == 0 ? _interferogramAbsorbanceEnd : GetInt(_interferogramAbsorbanceEnd);

        public int BackgroundInterferogramSize =>
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

        public IList<float> AbsorbanceList
        {
            get
            {
                if (_absorbanceEnd == 0 || _absorbanceList.Count != 0) return _absorbanceList;
                for (var i = _absorbanceEnd - (WaveNumberSize * 4); _absorbanceEnd > i; i = i + 4)
                {
                    var absorbanceValue = BinaryPrimitives.ReadSingleLittleEndian(Content.AsSpan().Slice(i, 4));
                    _absorbanceList.Add(absorbanceValue);
                }

                return _absorbanceList;
            }
        }

        public bool InsertAbsorbanceList(IList<float> floatArray)
        {
            if (floatArray.Count != WaveNumberSize) return false;
            for (int n = 0, i = _absorbanceEnd - (WaveNumberSize * 4); _absorbanceEnd > i; i = i + 4, n++)
            {
                BinaryPrimitives.WriteSingleLittleEndian(Content.AsSpan().Slice(i, 4), floatArray[n]);
            }

            return true;
        }

        public IList<float> AbsorbanceInterferogramList
        {
            get
            {
                if (_interferogramAbsorbanceEnd == 0 || _arbsorbanceInterferogramList.Count != 0) return _arbsorbanceInterferogramList;
                for (var i = _interferogramAbsorbanceEnd - (AbsorbanceInterferogramSize * 4); _interferogramAbsorbanceEnd > i; i += 4)
                {
                    var interferogramValue = BitConverter.ToSingle(Content, i);
                    _arbsorbanceInterferogramList.Add(interferogramValue);
                }


                return _arbsorbanceInterferogramList;
            }
        }

        public IList<float> BackgroundInterferogramList
        {
            get
            {
                if (_interferogramBackgroundEnd == 0 || _backgroundInterferogramList.Count != 0) return _backgroundInterferogramList;
                for (var i = _interferogramBackgroundEnd - (BackgroundInterferogramSize * 4); _interferogramBackgroundEnd > i; i += 4)
                {
                    var interferogramValue = BitConverter.ToSingle(Content, i);
                    _backgroundInterferogramList.Add(interferogramValue);
                }

                return _backgroundInterferogramList;
            }
        }
    }
}
