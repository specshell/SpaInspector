namespace SpaFileReader.Tests;

public class WriterTest
{
    private readonly byte[] _bytes;
    private readonly byte[] _noInterferogramBytes;

    public WriterTest()
    {
        _bytes = File.ReadAllBytes(TestFile("test1.spa"));
        _noInterferogramBytes = File.ReadAllBytes(TestFile("test2.spa"));
    }

    [Fact]
    public void WriteYUnitAsBytesTest()
    {
        var convertedSpa = SpaFile.WriteYUnitAsSpanByte(_bytes, SpaFile.ReadYUnitAsSpanByte(_noInterferogramBytes));
        var spaToWrite = SpaFile.ReadYUnitAsFloatArray(_noInterferogramBytes);
        var writtenSpa = SpaFile.ReadYUnitAsFloatArray(convertedSpa.ToArray());
        Assert.Equal(spaToWrite, writtenSpa);
    }

    [Fact]
    public void WriteYUnitAsFloatsTest()
    {
        var convertedSpa = SpaFile.WriteYUnitAsSpanFloat( _bytes, SpaFile.ReadYUnitAsFloatArray(_noInterferogramBytes));
        var spaToWrite = SpaFile.ReadYUnitAsFloatArray(_noInterferogramBytes);
        var writtenSpa = SpaFile.ReadYUnitAsFloatArray(convertedSpa.ToArray());
        Assert.Equal(spaToWrite, writtenSpa);
    }


}
