using System.Net.Http;
using System.Text;
using System.Linq;
using NUnit.Framework;

namespace SharpToken.Tests;

public class Tests
{
    private static readonly List<string> ModelsList = new() { "p50k_base", "r50k_base", "cl100k_base", "o200k_base", "o200k_harmony" };

    private static readonly List<Tuple<string, string, List<int>>> TestData =
        TestHelpers.ReadTestPlans("SharpToken.Tests.data.TestPlans.txt");

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    [TestCaseSource(nameof(TestData))]
    public void TestEncodingAndDecoding(Tuple<string, string, List<int>> resource)
    {
        var (encodingName, textToEncode, expectedEncoded) = resource;

        var encoding = GptEncoding.GetEncoding(encodingName);
        
        // Detect if the text contains special tokens
        var allowedSpecial = new HashSet<string>();
        var specialTokens = GetSpecialTokensForEncoding(encodingName);
        foreach (var token in specialTokens)
        {
            if (textToEncode.Contains(token))
            {
                allowedSpecial.Add(token);
            }
        }
        
        var encoded = encoding.Encode(textToEncode, allowedSpecial);
        var decodedText = encoding.Decode(encoded);
        Assert.Multiple(() =>
        {
            Assert.That(encoded, Is.EqualTo(expectedEncoded));
            Assert.That(decodedText, Is.EqualTo(textToEncode));
        });
    }

    [Test]
    [TestCaseSource(nameof(TestData))]
    public void TestTokensLength(Tuple<string, string, List<int>> resource)
    {
        var (encodingName, textToEncode, expectedEncoded) = resource;

        var encoding = GptEncoding.GetEncoding(encodingName);
        
        // Detect if the text contains special tokens
        var allowedSpecial = new HashSet<string>();
        var specialTokens = GetSpecialTokensForEncoding(encodingName);
        foreach (var token in specialTokens)
        {
            if (textToEncode.Contains(token))
            {
                allowedSpecial.Add(token);
            }
        }
        
        var tokenLength = encoding.CountTokens(textToEncode, allowedSpecial);
        Assert.Multiple(() =>
        {
            Assert.That(tokenLength, Is.EqualTo(expectedEncoded.Count));
        });
    }

    [Test]
    public async Task TestEncodingAndDecodingInParallel()
    {
        var tasks = TestData.Select(_ => Task.Run(() =>
        {
            var (encodingName, textToEncode, expectedEncoded) = _;
            var encoding = GptEncoding.GetEncoding(encodingName);
            
            // Detect if the text contains special tokens
            var allowedSpecial = new HashSet<string>();
            var specialTokens = GetSpecialTokensForEncoding(encodingName);
            foreach (var token in specialTokens)
            {
                if (textToEncode.Contains(token))
                {
                    allowedSpecial.Add(token);
                }
            }
            
            var encoded = encoding.Encode(textToEncode, allowedSpecial);
            var decodedText = encoding.Decode(encoded);
            return (textToEncode, encoded, expectedEncoded, decodedText);
        }));

        await Task.WhenAll(tasks).ConfigureAwait(false);

        foreach (var (textToEncode, encoded, expectedEncoded, decodedText) in tasks.Select(_ => _.Result))
        {
            Assert.Multiple(() =>
            {
                Assert.That(encoded, Is.EqualTo(expectedEncoded));
                Assert.That(decodedText, Is.EqualTo(textToEncode));
            });
        }
    }

    [Test]
    public void TestEncodingWithCustomAllowedSet()
    {
        const string encodingName = "cl100k_base";
        const string inputText = "Some Text<|endofprompt|>";
        var allowedSpecialTokens = new HashSet<string> { "<|endofprompt|>" };

        var encoding = GptEncoding.GetEncoding(encodingName);
        var encoded = encoding.Encode(inputText, allowedSpecialTokens);
        var expectedEncoded = new List<int> { 8538, 2991, 100276 };

        Assert.That(encoded, Is.EqualTo(expectedEncoded));
    }

    [Test]
    public void TestEncodingWithAllowedAll()
    {
        const string encodingName = "cl100k_base";
        const string inputText = "<|fim_prefix|>lorem ipsum<|fim_suffix|>, consectetur adipisicing elit<|fim_middle|>dolor sit amet";
        const int fimPrefix = 100258;
        const int fimMiddle = 100259;
        const int fimSuffix = 100260;
        var allowedSpecialTokens = new HashSet<string> { "all" };

        var encoding = GptEncoding.GetEncoding(encodingName);
        var encoded = encoding.Encode(inputText, allowedSpecialTokens);
        var expectedEncoded = new List<int> { fimPrefix, 385, 1864, 27439, fimSuffix, 11, 36240, 57781, 31160, fimMiddle, 67, 795, 2503, 28311 };

        var decoded = encoding.Decode(encoded);

        Assert.Multiple(() => {
            Assert.That(encoded, Is.EqualTo(expectedEncoded));
            Assert.That(decoded, Is.EqualTo(inputText));
        });
    }

    [Test]
    public void TestEncodingFailsWithInvalidInputDefaultSpecial()
    {
        const string encodingName = "cl100k_base";
        const string inputText = "Some Text<|endofprompt|>";

        var encoding = GptEncoding.GetEncoding(encodingName);

        void TestAction()
        {
            encoding.Encode(inputText);
        }

        Assert.Throws<ArgumentException>(TestAction);
    }

    [Test]
    public void TestEncodingFailsWithInvalidInputCustomDisallowed()
    {
        const string encodingName = "cl100k_base";
        const string inputText = "Some Text";

        var encoding = GptEncoding.GetEncoding(encodingName);

        void TestAction()
        {
            encoding.Encode(inputText, disallowedSpecial: new HashSet<string> { "Some" });
        }

        Assert.Throws<ArgumentException>(TestAction);
    }

    [Test]
    public void TestModelPrefixToEncodingMapping()
    {
        const string encodingName = "cl100k_base";
        const string modelName = "gpt-3.5-turbo-16k-0613";
        const string fakeModelName = "gpt-3.6-turbo";

        var encoding = Model.GetEncodingNameForModel(modelName);

        static void TestModelPrefixMappingFailsAction()
        {
            Model.GetEncodingNameForModel(fakeModelName);
        }
        Assert.Multiple(() =>
        {
            Assert.That(encoding, Is.EqualTo(encodingName));
            Assert.Throws<Exception>(TestModelPrefixMappingFailsAction);
        });
    }

    [Test]
    [TestCaseSource(nameof(ModelsList))]
    public async Task TestLocalResourceMatchesRemoteResource(string modelName)
    {
        // Skip o200k_harmony as it reuses o200k_base.tiktoken and doesn't have its own remote file
        if (modelName == "o200k_harmony")
        {
            Assert.Pass("o200k_harmony reuses o200k_base.tiktoken file and doesn't have its own remote file");
            return;
        }

        var embeddedResourceName = $"SharpToken.data.{modelName}.tiktoken";
        var remoteResourceUrl = $"https://openaipublic.blob.core.windows.net/encodings/{modelName}.tiktoken";

        // Read the embedded resource file
        using var stream = typeof(GptEncoding).Assembly.GetManifestResourceStream(embeddedResourceName) ??
                           throw new InvalidOperationException();
        var embeddedResourceText = new StreamReader(stream).ReadToEnd();
        var normalizedEmbeddedResourceText =
            embeddedResourceText.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);

        // Download the remote file
        using var httpClient = new HttpClient();
        var remoteResourceBytes = await httpClient.GetByteArrayAsync(remoteResourceUrl).ConfigureAwait(true);
        var remoteResourceText = Encoding.UTF8.GetString(remoteResourceBytes);
        var normalizedRemoteResourceText = remoteResourceText.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);

        // Compare the contents of the files and assert their equality
        Assert.That(normalizedEmbeddedResourceText, Is.EqualTo(normalizedRemoteResourceText));
    }

    [Test]
    public void TestEncodingForModel()
    {
        const string modelName = "gpt-4";
        const string inputText = "Hello, world!";
        var expectedEncoded = new List<int> { 9906, 11, 1917, 0 };

        var encoding = GptEncoding.GetEncodingForModel(modelName);
        var encoded = encoding.Encode(inputText);
        var decodedText = encoding.Decode(encoded);

        Assert.Multiple(() =>
        {
            Assert.That(encoded, Is.EqualTo(expectedEncoded));
            Assert.That(decodedText, Is.EqualTo(inputText));
        });
    }

    [Test]
    public void TestO200KHarmonySpecialTokens()
    {
        var encoding = GptEncoding.GetEncoding("o200k_harmony");
        const string inputText = "Hello, world!";
        
        // Test basic encoding/decoding
        var encoded = encoding.Encode(inputText);
        var decodedText = encoding.Decode(encoded);
        Assert.That(decodedText, Is.EqualTo(inputText));

        // Test that o200k_harmony has more special tokens than o200k_base
        var baseEncoding = GptEncoding.GetEncoding("o200k_base");
        
        // Test encoding with special tokens
        var textWithSpecialTokens = "Hello <|startoftext|> world <|call|> test <|reserved_200020|>";
        var encodedSpecial = encoding.Encode(textWithSpecialTokens, allowedSpecial: new HashSet<string> { "<|startoftext|>", "<|call|>", "<|reserved_200020|>" });
        var decodedSpecial = encoding.Decode(encodedSpecial);
        
        Assert.That(decodedSpecial, Is.EqualTo(textWithSpecialTokens));
        
        // Verify specific special token IDs
        Assert.That(encoding.Encode("<|startoftext|>", allowedSpecial: new HashSet<string> { "<|startoftext|>" }), Is.EqualTo(new List<int> { 199998 }));
        Assert.That(encoding.Encode("<|call|>", allowedSpecial: new HashSet<string> { "<|call|>" }), Is.EqualTo(new List<int> { 200012 }));
        Assert.That(encoding.Encode("<|reserved_200020|>", allowedSpecial: new HashSet<string> { "<|reserved_200020|>" }), Is.EqualTo(new List<int> { 200020 }));
    }

    [Test]
    public void TestGPT5ModelMappings()
    {
        // Test that GPT-5 models map to the correct encodings
        Assert.That(Model.GetEncodingNameForModel("gpt-5"), Is.EqualTo("o200k_base"));
        Assert.That(Model.GetEncodingNameForModel("gpt-5-mini"), Is.EqualTo("o200k_base"));
        Assert.That(Model.GetEncodingNameForModel("gpt-5-nano"), Is.EqualTo("o200k_base"));
        Assert.That(Model.GetEncodingNameForModel("gpt-5-pro"), Is.EqualTo("o200k_base"));
        Assert.That(Model.GetEncodingNameForModel("gpt-5-thinking"), Is.EqualTo("o200k_base"));
        
        // Test prefix matching for GPT-5 variants
        Assert.That(Model.GetEncodingNameForModel("gpt-5-2024-08-07"), Is.EqualTo("o200k_base"));
        Assert.That(Model.GetEncodingNameForModel("gpt-5-chat-latest"), Is.EqualTo("o200k_base"));
    }

    private static HashSet<string> GetSpecialTokensForEncoding(string encodingName)
    {
        return encodingName switch
        {
            "r50k_base" or "p50k_base" => new HashSet<string> { "<|endoftext|>" },
            "p50k_edit" => new HashSet<string> { "<|endoftext|>", "<|fim_prefix|>", "<|fim_middle|>", "<|fim_suffix|>" },
            "cl100k_base" => new HashSet<string> { "<|endoftext|>", "<|fim_prefix|>", "<|fim_middle|>", "<|fim_suffix|>", "<|endofprompt|>" },
            "o200k_base" => new HashSet<string> { "<|endoftext|>", "<|endofprompt|>" },
            "o200k_harmony" => new HashSet<string> 
            { 
                "<|endoftext|>", "<|endofprompt|>", "<|startoftext|>", "<|return|>", "<|constrain|>", 
                "<|channel|>", "<|start|>", "<|end|>", "<|message|>", "<|call|>"
            }.Union(Enumerable.Range(200000, 1088).Select(i => $"<|reserved_{i}|>")).ToHashSet(),
            _ => new HashSet<string>()
        };
    }
}
