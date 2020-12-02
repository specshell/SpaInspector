using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace SpaFileReader
{
    public record Read : IDisposable
    {
        private static readonly DateTime SpaFileEpoch = new(1899, 12, 31, 0, 0, 0, DateTimeKind.Utc);
        private BinaryReader _binaryReader;
        private SpaBuilder _builder;
        private SpectrumBuilder _spectrumBuilder;

        public Read(Stream stream)
        {
            _binaryReader = new BinaryReader(stream);
            _builder = new SpaBuilder();
            _spectrumBuilder = new SpectrumBuilder();
        }

        public Spa ReadBinaryToSpa()
        {
            _binaryReader.Position(30);
            var fileTitle = _binaryReader.ReadNullTerminatedString();
            _builder.FileTitle(fileTitle);

            // Read out number of lines in header
            _binaryReader.Position(294);
            var numberOfKeys = _binaryReader.ReadInt16();

            // days since 31/12/1899, 00:00
            var timestamp = _binaryReader.ReadUInt32();
            var dateTime = SpaFileEpoch.Add(TimeSpan.FromSeconds(timestamp));
            _builder.FileDateTime(dateTime);
            /*
                headers / metadata start from dec 304 and goes till dec 496
                    key 2 = headers
                    key 3 = Unit Intensities (absorbance)
                    key 27 = History
                    key 106 = start of Settings Info and size.
                    key 105 = end of Settings Info
                    key 107 = Spectra Title and timestamp
                    key 128 = ??

                This key seems possible to repeat up to three times
                    key 130 at hex 190, dec 400 = one of them is unit end
                                                = second is file exp position
                                                = third is ATR correction (optional)

                These values contain the interferograms and are optional
                   key 103 and 101 are paired, is the background interferogram
                   key 103 start of interferogram and byte size
                   key 101 end of interferogram

                   key 102 and 100 are paired, is the unit interferogram
                   key 102 start off interferogram and byte size
                   key 100 end of interferogram
            */

            // +1 to include an null key
            for (int i = 0, pos = 304; i < numberOfKeys+1; i++, pos += 16)
            {
                _binaryReader.Position(pos);
                var key = _binaryReader.ReadByte();
                if (i != 0 && (key == 2 || key == 0))
                {
                    _builder.Spectrum(_spectrumBuilder.Build());
                    _builder.Build();
                    _spectrumBuilder = new SpectrumBuilder();
                }
                switch (key)
                {
                    case 2:
                        ReadHeaders(pos);
                        break;
                    case 3:
                        _spectrumBuilder.UnitIntensities(ReadIntensities(pos));
                        break;
                    case 27:
                        _binaryReader.Position(pos + 2);
                        var historyPos = _binaryReader.ReadUInt32();
                        _binaryReader.Position(historyPos);
                        _spectrumBuilder.History(_binaryReader.ReadNullTerminatedString());
                        break;
                    case 106:
                        _binaryReader.Position(pos + 2);
                        var settingsInfoPos = _binaryReader.ReadUInt32();
                        _binaryReader.Position(settingsInfoPos + 44);
                        _spectrumBuilder.Gain(_binaryReader.ReadSingle())
                            .OpticalVelocity(_binaryReader.ReadSingle());
                        break;
                    case 107:
                        _binaryReader.Position(pos + 2);
                        var spectraTitlePos = _binaryReader.ReadUInt32();
                        _binaryReader.Position(spectraTitlePos);
                        var name = _binaryReader.ReadNullTerminatedString();
                        _spectrumBuilder.Name(name);
                        _binaryReader.Position(spectraTitlePos + 256);
                        var spectraTimestamp = _binaryReader.ReadUInt32();
                        var spectraDateTime = SpaFileEpoch.Add(TimeSpan.FromSeconds(spectraTimestamp));
                        _spectrumBuilder.DateTime(spectraDateTime);
                        break;
                    // case 103:
                    //     // Background Interferogram
                    //     _builder.BackgroundInterferogram(ReadIntensities(pos));
                    //     break;
                    // case 102:
                    //     // Unit Interferogram
                    //     _builder.UnitInterferogram(ReadIntensities(pos));
                    //     break;
                }
            }

            _binaryReader.Close();

            return _builder.Build();
        }

        private float[] ReadIntensities(int pos)
        {
            _binaryReader.Position(pos + 2);
            var intensityPos = _binaryReader.ReadInt32();
            _binaryReader.Position(pos + 6);
            var intensitySize = _binaryReader.ReadInt32();
            return ReadFloats(intensityPos, intensitySize);
        }

        private float[] ReadFloats(int position, int size)
        {
            _binaryReader.Position(position);
            var asBytes = _binaryReader.ReadBytes(size).AsSpan();
            var asFloats = MemoryMarshal.Cast<byte, float>(asBytes);
            return asFloats.ToArray();
        }

        private Headers ReadHeaders(int pos)
        {
            _binaryReader.Position(pos + 2);
            var infoPos = _binaryReader.ReadUInt32();
// other positions:
//   unitSize pos = info_pos + 4
//   xaxis unit code = info_pos + 8
//   data unit code = info_pos + 12
//   fistx_pos = info_pos + 16
//   lastx_pos = info_pos + 20
//   number of scan pos = info_pos + 36;
//   number of background scan pos = info_pos + 52;
//   Signal Strength = info_pos + 72

            _binaryReader.Position(infoPos + 4);
            var unitSize = _binaryReader.ReadUInt32();

            _binaryReader.Position(infoPos + 8);
            var key = _binaryReader.ReadByte();
            string xunits;
            string xtitle;
            switch (key)
            {
                case 1:
                    xunits = "cm ^ -1";
                    xtitle = "Wave numbers";
                    break;
                case 2:
                    xunits = "";
                    xtitle = "Data points";
                    break;
                case 3:
                    xunits = "nm";
                    xtitle = "Wavelengths";
                    break;
                case 4:
                    xunits = "um";
                    xtitle = "Wavelengths";
                    break;
                case 32:
                    xunits = "cm^-1";
                    xtitle = "Raman Shift";
                    break;
                default:
                    xunits = "";
                    xtitle = "x axis";
                    break;
            }

            _binaryReader.Position(infoPos + 12);
            key = _binaryReader.ReadByte();
            string units;
            string title;
            switch (key)
            {
                case 17:
                    units = "absorbance";
                    title = "Absorbance";
                    break;
                case 16:
                    units = "percent";
                    title = "Transmittance";
                    break;
                case 11:
                    units = "percent";
                    title = "Reflectance";
                    break;
                case 12:
                    units = "";
                    title = "Log(1/R)";
                    break;
                case 20:
                    units = "Kubelka_Munk";
                    title = "Kubelka-Munk";
                    break;
                case 22:
                    units = "V";
                    title = "Volts";
                    break;
                case 26:
                    units = "";
                    title = "Photoacoustic";
                    break;
                case 31:
                    units = "";
                    title = "Raman Intensity";
                    break;
                default:
                    units = "";
                    title = "Intensity";
                    break;
            }

            _binaryReader.Position(infoPos + 16);
            var firstx = _binaryReader.ReadSingle();
            _binaryReader.Position(infoPos + 20);
            var lastx = _binaryReader.ReadSingle();
            _binaryReader.Position(infoPos + 36);
            var numberOfScan = _binaryReader.ReadUInt32();
            _binaryReader.Position(infoPos + 52);
            var numberOfBackgroundScan = _binaryReader.ReadUInt32();
            _binaryReader.Position(infoPos + 72);
            var signalStrength = _binaryReader.ReadSingle();

            _spectrumBuilder.UnitSize(unitSize)
                .XUnits(xunits).XTitle(xtitle)
                .Unit(units).UnitTitle(title)
                .FirstX(firstx).LastX(lastx).SignalStrength(signalStrength)
                .NumberOfScan(numberOfScan).NumberOfBackgroundScan(numberOfBackgroundScan);

            return new Headers
            {
                UnitSize = unitSize,
                XUnits = xunits,
                XTitle = xtitle,
                Unit = units,
                UnitTitle = title,
                FirstX = firstx,
                LastX = lastx,
                NumberOfScan = numberOfScan,
                NumberOfBackgroundScan = numberOfBackgroundScan,
            };
        }

        public void Dispose()
        {
            _binaryReader?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
