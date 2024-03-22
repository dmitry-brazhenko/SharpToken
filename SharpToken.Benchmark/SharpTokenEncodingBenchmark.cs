using System;
using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;


namespace SharpToken.Benchmark
{
    [SimpleJob(RuntimeMoniker.Net471, baseline: true)]
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net80)]
    [RPlotExporter]
    [MemoryDiagnoser]
    public class SharpTokenEncodingBenchmark
    {
        private GptEncoding _encoding;

        public static IEnumerable<string> Lines => File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "../../../../../../../../SharpToken.Tests/data/GptEncoderTestSamples.txt"));

        [GlobalSetup]
        public void Setup()
        {
            _encoding = SharpToken.GptEncoding.GetEncoding("cl100k_base");
        }

        [Benchmark]
        public List<List<int>> Encode()
        {
            var result = new List<List<int>>();
            foreach (var line in Lines)
            {
                var encoded = _encoding.Encode(line);
                result.Add(encoded);
            }

            return result;
        }
    }
}
