using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using TiktokenSharp;
using Microsoft.DeepDev;
using Microsoft.ML.Tokenizers;

namespace SharpToken.Benchmark
{
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net80)]
    [SimpleJob(RuntimeMoniker.Net471)]
    [RPlotExporter]
    [MemoryDiagnoser]
    public class CompareBenchmark
    {
        private GptEncoding _sharpToken;
        private TikToken _tikToken;
        private ITokenizer _tokenizer;
        private Tokenizer _mlTokenizer;
        private string _kLongText;

        [GlobalSetup] // TODO: move this to SetupO200k?
        public async Task SetupCL100k()
        {
            _sharpToken = GptEncoding.GetEncoding("cl100k_base");
            _tikToken = await TikToken.GetEncodingAsync("cl100k_base").ConfigureAwait(false);
            _tokenizer = await TokenizerBuilder.CreateByModelNameAsync("gpt-4").ConfigureAwait(false);
            _mlTokenizer = Tokenizer.CreateTiktokenForModel("gpt-4");
            _kLongText = "King Lear, one of Shakespeare's darkest and most savage plays, tells the story of the foolish and Job-like Lear, who divides his kingdom, as he does his affections, according to vanity and whim. Lear’s failure as a father engulfs himself and his world in turmoil and tragedy.";
        }

        public async Task SetupO200k()
        {
            _sharpToken = GptEncoding.GetEncoding("o200k_base");
            _tikToken = await TikToken.GetEncodingAsync("o200k_base").ConfigureAwait(false);
            _tokenizer = await TokenizerBuilder.CreateByModelNameAsync("gpt-4o").ConfigureAwait(false);
            _mlTokenizer = Tokenizer.CreateTiktokenForModel("gpt-4o");
            _kLongText = "King Lear, one of Shakespeare's darkest and most savage plays, tells the story of the foolish and Job-like Lear, who divides his kingdom, as he does his affections, according to vanity and whim. Lear’s failure as a father engulfs himself and his world in turmoil and tragedy.";
        }

        [Benchmark]
        public int SharpToken()
        {
            var sum = 0;
            for (var i = 0; i < 10000; i++)
            {
                var encoded = _sharpToken.Encode(_kLongText);
                var decoded = _sharpToken.Decode(encoded);
                sum += decoded.Length;
            }

            return sum;
        }

        [Benchmark]
        public int TiktokenSharp()
        {
            var sum = 0;
            for (var i = 0; i < 10000; i++)
            {
                var encoded = _tikToken.Encode(_kLongText);
                var decoded = _tikToken.Decode(encoded);
                sum += decoded.Length;
            }

            return sum;
        }

        [Benchmark]
        public int TokenizerLib()
        {
            var sum = 0;
            for (var i = 0; i < 10000; i++)
            {
                var encoded = _tokenizer.Encode(_kLongText);
                var decoded = _tokenizer.Decode(encoded.ToArray());
                sum += decoded.Length;
            }

            return sum;
        }

        [Benchmark]
        public int MLTokenizers()
        {
            var sum = 0;
            for (var i = 0; i < 10000; i++)
            {
                var encoded = _mlTokenizer.EncodeToIds(_kLongText);
                var decoded = _mlTokenizer.Decode(encoded);
                sum += decoded.Length;
            }

            return sum;
        }
    }
}
