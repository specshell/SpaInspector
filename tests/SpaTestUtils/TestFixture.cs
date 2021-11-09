using System.IO;

namespace SpaTestUtils
{
    public static class TestFixture
    {
        public static string TestFile(string file) => Path.Combine(".", "..", "..", "..", "..", "TestFiles", file);
        public static string BenchmarkTestFile(string file) => Path.Combine(".", "..", "..", "..", "..", "..", "..", "..", "..", "TestFiles", file);
    }
}
