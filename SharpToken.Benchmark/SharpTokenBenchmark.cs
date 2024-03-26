using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;


namespace SharpToken.Benchmark
{
    [SimpleJob(RuntimeMoniker.Net471, baseline: true)]
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net80)]
    [RPlotExporter]
    [MemoryDiagnoser]
    public class SharpTokenBenchmark
    {
        private GptEncoding _encoding;
        private string _smallText;
        private string _largeText;
        private int[] _smallTextEncoded;
        private int[] _largeTextEncoded;

        [GlobalSetup]
        public void Setup()
        {
            _encoding = SharpToken.GptEncoding.GetEncoding("cl100k_base");
            _smallText = FileHelper.ReadFile("Files/small-text.txt");
            _largeText = FileHelper.ReadFile("Files/large-text.html");
            _smallTextEncoded = FileHelper.ReadJson<int[]>("Files/small-text.json");
            _largeTextEncoded = FileHelper.ReadJson<int[]>("Files/large-text.json");
        }

        [Benchmark]
        public int Encode_SmallText()
        {
            var encoded = _encoding.Encode(_smallText);
            return encoded.Count;
        }

        [Benchmark]
        public int Encode_LargeText()
        {
            var encoded = _encoding.Encode(_largeText);
            return encoded.Count;
        }


        [Benchmark]
        public int Decode_SmallText()
        {
            var decoded = _encoding.Decode(_smallTextEncoded);
            return decoded.Length;
        }

        [Benchmark]
        public int Decode_LargeText()
        {
            var decoded = _encoding.Decode(_largeTextEncoded);
            return decoded.Length;
        }


        [Benchmark]
        public int CountTokens_SmallText()
        {
            var count = _encoding.CountTokens(_smallText);
            return count;
        }

        [Benchmark]
        public int CountTokens_LargeText()
        {
            var count = _encoding.CountTokens(_largeText);
            return count;
        }
    }
}
