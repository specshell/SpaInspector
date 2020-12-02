using System;
using System.IO;
using Xunit;
using static SpaFileReader.Tests.TestFixture;

namespace SpaFileReader.Tests
{
    public class ReadTest
    {
        [Fact]
        public async void Test1()
        {
            var memoryStream = await TestFile("bg_nonbutchered.SPA").FileAsMemoryStream();
            var spa = memoryStream.ReadSpa();
            Assert.Equal("Bkg Tue Jan 28 09:15:50 2020 (GMT+01:00)", spa.FileTitle);
            Assert.Equal(new DateTime(2020, 01, 28, 8, 15, 50, DateTimeKind.Utc), spa.FileDateTime);
        }

        [Fact]
        public async void Test2() {
            var memoryStream = await TestFile("spectre_data3.spa").FileAsMemoryStream();
            var spa = memoryStream.ReadSpa();
            Assert.Equal("**2020-08-20_11-38-05", spa.FileTitle);
            Assert.Equal(new DateTime(2020, 08, 20, 11, 38, 38, DateTimeKind.Utc), spa.FileDateTime);
        }

        [Fact]
        public async void Test3() {
            var memoryStream = await TestFile("M1-M10.SPG").FileAsMemoryStream();
            var spa = memoryStream.ReadSpa();
            Assert.Equal("**2020-08-20_11-38-05", spa.FileTitle);
            Assert.Equal(new DateTime(2020, 08, 20, 11, 38, 38, DateTimeKind.Utc), spa.FileDateTime);
        }

        [Fact]
        public void Date()
        {
            var datetime = new DateTime(1899, 12, 31);
            Assert.Equal(0, datetime.Hour);
            Assert.Equal(0, datetime.Minute);
        }
    }
}
