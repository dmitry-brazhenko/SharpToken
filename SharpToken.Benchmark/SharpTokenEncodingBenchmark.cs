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
        private string _largeText;

        [GlobalSetup]
        public void Setup()
        {
            _encoding = SharpToken.GptEncoding.GetEncoding("cl100k_base");
            _lines = FileHelper.ReadFileLines("../../../../../../../../SharpToken.Tests/data/GptEncoderTestSamples.txt");
            _largeText = FileHelper.ReadFile("Files/large-text.html");
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


        [Benchmark]
        public int CountTokens_LargeInput()
        {
            var count = _encoding.TokenCount(_largeText);
            return count;
        }
    }
}
