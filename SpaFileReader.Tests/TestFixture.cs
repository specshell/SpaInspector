using System;
using Xunit;

namespace SpaFileReader.Tests
{
    public class TestFixture
    {
        public static readonly string CurrentProject = $"{Environment.CurrentDirectory}/../../..";
        public static readonly string CurrentSolution = $"{CurrentProject}/..";

        public static readonly string TestFiles = $"{CurrentSolution}/TestFiles";

        public static string TestFile(string file) => $"{TestFiles}/{file}";
    }
}
