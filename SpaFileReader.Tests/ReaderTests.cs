using System.IO;
using Xunit;
using static SpaTestUtils.TestFixture;

namespace SpaFileReader.Tests
{
    public class ReaderTests
    {
        [Fact]
        public void SpanReaderReadAsFloatsTest()
        {
            var bytes = File.ReadAllBytes(TestFile("test1.spa"));
            var readAbsorbanceStartPosition = SpaFile.ReadYUnitAsFloats(bytes);
            Assert.Equal(6846, readAbsorbanceStartPosition.Length);
        }

        [Fact]
        public void SpanReaderReadAsBytesTest()
        {
            var bytes = File.ReadAllBytes(TestFile("test1.spa"));
            var readAbsorbanceStartPosition = SpaFile.ReadYUnitAsBytes(bytes);
            Assert.Equal(27384, readAbsorbanceStartPosition.Length);
        }
    }
}
