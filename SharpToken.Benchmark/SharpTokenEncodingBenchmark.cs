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
        private string[] _lines;

        [GlobalSetup]
        public void Setup()
        {
            _encoding = SharpToken.GptEncoding.GetEncoding("cl100k_base");
            _lines = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "../../../../../../../../SharpToken.Tests/data/GptEncoderTestSamples.txt"));
        }

        [Benchmark]
        public int Encode()
        {
            var sum = 0;
            var len = _lines.Length;
            for (var i = 0; i < len; i++)
            {
                var line = _lines[i];
                var encoded = _encoding.Encode(line);
                sum += encoded.Count;
            }

            return sum;
        }


        [Benchmark]
        public int CountTokens()
        {
            var sum = 0;
            var len = _lines.Length;
            for (var i = 0; i < len; i++)
            {
                var line = _lines[i];
                var count = _encoding.TokenCount(line);
                sum += count;
            }

            return sum;
        }
    }
}
