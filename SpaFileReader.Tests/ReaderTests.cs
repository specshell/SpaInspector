using System.IO;
using Xunit;
using static SpaTestUtils.TestFixture;

namespace SpaFileReader.Tests
{
    public class ReaderTests
    {
        private readonly byte[] _bytes;

        public ReaderTests()
        {
            _bytes = File.ReadAllBytes(TestFile("test1.spa"));
        }

        [Fact]
        public void SpanReaderReadAsFloatsTest()
        {
            var yUnits = SpaFile.ReadYUnitAsFloats(_bytes);
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
            var yUnits = SpaFile.ReadYUnitAsDoubles(_bytes);
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
            var yUnits = SpaFile.ReadYUnitAsBytes(_bytes);
            Assert.Equal(27384, yUnits.Length);
        }
    }
}
