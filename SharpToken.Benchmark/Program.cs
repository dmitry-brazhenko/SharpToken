using System;
using BenchmarkDotNet.Running;


namespace SharpToken.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<CompareBenchmark>();
            Console.ReadLine();
        }
    }
}
