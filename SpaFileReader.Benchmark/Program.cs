using BenchmarkDotNet.Running;

namespace SpaFileReader.Benchmark
{
    public class Program
    {
        private static void Main(string[] args)
            => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
