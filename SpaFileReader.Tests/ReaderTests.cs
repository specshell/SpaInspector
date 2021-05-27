using System.IO;
using Xunit;
using static SpaTestUtils.TestFixture;

namespace SpaFileReader.Tests
{
    public class ReaderTests
    {
        [Fact]
        public void SpanReaderReadTest()
        {
            var bytes = File.ReadAllBytes(TestFile("test1.spa"));
            var readAbsorbanceStartPosition = SpaFileReader.ReadXUnit(bytes);
            Assert.Equal(6846, readAbsorbanceStartPosition.Length);
        }

    }
}
