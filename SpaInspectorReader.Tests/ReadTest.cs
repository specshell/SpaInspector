using System;
using Xunit;
using static SpaTestUtils.TestFixture;

namespace SpaInspectorReader.Tests
{
    public class ReadTest
    {
        [Fact]
        public void LittleEndian()
        {
            Assert.True(BitConverter.IsLittleEndian);
        }

        [Fact]
        public async void BinaryReaderReadTest()
        {
            var memoryStream = await TestFile("test1.spa").FileAsMemoryStream();
            var spa = memoryStream.ReadSpa();
            Assert.Contains("ASC2016114_2021-05-06_11-13-32", spa.Title);
            Assert.Equal(new DateTime(2021, 05, 06, 11, 13, 32, DateTimeKind.Utc), spa.DateTime);
        }
    }
}
