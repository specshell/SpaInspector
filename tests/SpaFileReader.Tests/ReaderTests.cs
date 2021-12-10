using System.IO;
using Xunit;
using static SpaTestUtils.TestFixture;

namespace SpaFileReader.Tests;

public class ReaderTests
{
    private readonly byte[] _bytes;
    private readonly byte[] _noInterferogramBytes;

    public ReaderTests()
    {
        _bytes = File.ReadAllBytes(TestFile("test1.spa"));
        _noInterferogramBytes = File.ReadAllBytes(TestFile("test2.spa"));
    }

    [Fact]
    public void SpanReaderReadAsFloatsTest()
    {
        var yUnits = SpaFile.ReadYUnitAsSpanFloat(_bytes);
        Assert.Equal(6846, yUnits.Length);
        Assert.Equal(0.03549831f, yUnits[0]);
        Assert.Equal(0.009505022f, yUnits[^1]);
    }

    [Fact]
    public void SpanReaderReadAsFloatArrayTest()
    {
        var yUnits = SpaFile.ReadYUnitAsFloatArray(_bytes);
        Assert.Equal(6846, yUnits.Length);
        Assert.Equal(0.03549831f, yUnits[0]);
        Assert.Equal(0.009505022f, yUnits[^1]);
    }

    [Fact]
    public void SpanReaderReadAsDoublesTest()
    {
        var yUnits = SpaFile.ReadYUnitAsSpanDouble(_bytes);
        Assert.Equal(6846, yUnits.Length);
        Assert.Equal(0.03549830988049507, yUnits[0]);
        Assert.Equal(0.009505022317171097, yUnits[^1]);
    }

    [Fact]
    public void SpanReaderReadAsDoubleArrayTest()
    {
        var yUnits = SpaFile.ReadYUnitAsDoubleArray(_bytes);
        Assert.Equal(6846, yUnits.Length);
        Assert.Equal(0.03549830988049507, yUnits[0]);
        Assert.Equal(0.009505022317171097, yUnits[^1]);
    }

    [Fact]
    public void SpanReaderReadAsBytesTest()
    {
        var yUnits = SpaFile.ReadYUnitAsSpanByte(_bytes);
        Assert.Equal(27384, yUnits.Length);
    }

    [Fact]
    public void SpanReaderReadInterferogramAsFloatsTest()
    {
        var interferogram = SpaFile.ReadInterferogramAsSpanFloat(_bytes);
        Assert.Equal(12200, interferogram.Length);
        Assert.Equal(-1.9441359E-05f, interferogram[0]);
        Assert.Equal(-0.0009917123f, interferogram[^1]);
    }

    [Fact]
    public void SpanReaderReadBackgroundInterferogramAsFloatsTest()
    {
        var interferogram = SpaFile.ReadBackgroundInterogramAsSpanFloat(_bytes);
        Assert.Equal(12200, interferogram.Length);
        Assert.Equal(0.00012780126f, interferogram[0]);
        Assert.Equal(-0.00088237826f, interferogram[^1]);
    }

    [Fact]
    public void SpanReaderReadNoBackgroundInterferogramAsFloatsTest()
    {
        var interferogram = SpaFile.ReadBackgroundInterogramAsSpanFloat(_noInterferogramBytes);
        Assert.Equal(0, interferogram.Length);
        Assert.True(interferogram.IsEmpty);
    }
}
