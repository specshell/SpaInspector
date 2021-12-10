using System.Runtime.InteropServices;

namespace SpaInspectorReader;

public record Read : IDisposable
{
    private static readonly DateTime SpaFileEpoch = new(1899, 12, 31, 0, 0, 0, DateTimeKind.Utc);
    private readonly BinaryReader _binaryReader;

    public Read(Stream stream)
    {
        _binaryReader = new BinaryReader(stream);
    }

    public Spa ReadBinaryToSpa()
    {
        var builder = new SpaBuilder();
        _binaryReader.Position(30);
        var specTitle = _binaryReader.ReadNullTerminatedString();
        builder.Title(specTitle);
        _binaryReader.Position(296);

        // days since 31/12/1899, 00:00
        var timestamp = _binaryReader.ReadUInt32();
        var dateTime = SpaFileEpoch.Add(TimeSpan.FromSeconds(timestamp));
        builder.DateTime(dateTime);
        /*
            headers / metadata start from dec 304 and goes till dec 496
                key 2 at hex 130, dec 304 = headers
                key 106 at hex 140, dec 320 = start of Settings Info and size.
                key 105 at hex 150, dec 336 = end of Settings Info
                key 128 at hex 140, dec 320 = ??
                key 27 at hex 170, dec 368 = History
                key 3 at hex 180, dec 384 = Unit Intensities (absorbance)

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

        var pos = 304;

        byte key = 1;
        while (key != 0)
        {
            _binaryReader.Position(pos);
            key = _binaryReader.ReadByte();
            switch (key)
            {
                case 2:
                    ReadHeaders(builder, pos);
                    break;
                case 3:
                    builder.UnitIntensities(ReadIntensities(pos));
                    break;
                case 27:
                    _binaryReader.Position(pos + 2);
                    var historyPos = _binaryReader.ReadUInt32();
                    _binaryReader.Position(historyPos);
                    builder.History(_binaryReader.ReadNullTerminatedString());
                    break;
                case 106:
                    _binaryReader.Position(pos + 2);
                    var settingsInfoPos = _binaryReader.ReadUInt32();
                    _binaryReader.Position(settingsInfoPos + 44);
                    builder.Gain(_binaryReader.ReadSingle())
                        .OpticalVelocity(_binaryReader.ReadSingle());
                    break;
                case 103:
                    // Background Interferogram
                    builder.BackgroundInterferogram(ReadIntensities(pos));
                    break;
                case 102:
                    // Unit Interferogram
                    builder.UnitInterferogram(ReadIntensities(pos));
                    break;
            }

            pos += 16;
        }

        _binaryReader.Close();

        return builder.Build();
    }

    private Span<float> ReadIntensities(int pos)
    {
        _binaryReader.Position(pos + 2);
        var intensityPos = _binaryReader.ReadInt32();
        _binaryReader.Position(pos + 6);
        var intensitySize = _binaryReader.ReadInt32();
        var spanFloats = ReadFloats(intensityPos, intensitySize);
        spanFloats.Reverse();
        return spanFloats;
    }

    private Span<float> ReadFloats(int position, int size)
    {
        _binaryReader.Position(position);
        var asBytes = _binaryReader.ReadBytes(size).AsSpan();
        var asFloats = MemoryMarshal.Cast<byte, float>(asBytes);
        return asFloats;
    }

    private Headers ReadHeaders(SpaBuilder builder, int pos)
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
        string xunit;
        string xtitle;
        switch (key)
        {
            case 1:
                xunit = "cm^-1";
                xtitle = "Wave numbers";
                break;
            case 2:
                xunit = "";
                xtitle = "Data points";
                break;
            case 3:
                xunit = "nm";
                xtitle = "Wavelengths";
                break;
            case 4:
                xunit = "um";
                xtitle = "Wavelengths";
                break;
            case 32:
                xunit = "cm^-1";
                xtitle = "Raman Shift";
                break;
            default:
                xunit = "";
                xtitle = "x axis";
                break;
        }

        _binaryReader.Position(infoPos + 12);
        key = _binaryReader.ReadByte();
        string yunit;
        string ytitle;
        switch (key)
        {
            case 17:
                yunit = "absorbance";
                ytitle = "Absorbance";
                break;
            case 16:
                yunit = "percent";
                ytitle = "Transmittance";
                break;
            case 11:
                yunit = "percent";
                ytitle = "Reflectance";
                break;
            case 12:
                yunit = "";
                ytitle = "Log(1/R)";
                break;
            case 20:
                yunit = "Kubelka_Munk";
                ytitle = "Kubelka-Munk";
                break;
            case 22:
                yunit = "V";
                ytitle = "Volts";
                break;
            case 26:
                yunit = "";
                ytitle = "Photoacoustic";
                break;
            case 31:
                yunit = "";
                ytitle = "Raman Intensity";
                break;
            default:
                yunit = "";
                ytitle = "Intensity";
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

        builder.UnitSize(unitSize)
            .XUnits(xunit).XTitle(xtitle)
            .Unit(yunit).UnitTitle(ytitle)
            .FirstX(firstx).LastX(lastx).SignalStrength(signalStrength)
            .NumberOfScan(numberOfScan).NumberOfBackgroundScan(numberOfBackgroundScan);

        return new Headers
        {
            UnitSize = unitSize,
            XUnits = xunit,
            XTitle = xtitle,
            YUnit = yunit,
            YUnitTitle = ytitle,
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