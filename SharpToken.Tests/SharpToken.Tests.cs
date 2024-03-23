using System.Net.Http;
using System.Text;
using NUnit.Framework;

namespace SharpToken.Tests;

public class Tests
{
    private static readonly List<string> ModelsList = new() { "p50k_base", "r50k_base", "cl100k_base" };

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
        var encoded = encoding.Encode(textToEncode);
        var decodedText = encoding.Decode(encoded);
        Assert.Multiple(() =>
        {
            Assert.That(encoded, Is.EqualTo(expectedEncoded));
            Assert.That(decodedText, Is.EqualTo(textToEncode));
        });
    }

    [Test]
    public async Task TestEncodingAndDecodingInParallel()
    {
        var tasks = TestData.Select(_ => Task.Run(() =>
        {
            var (encodingName, textToEncode, expectedEncoded) = _;
            var encoding = GptEncoding.GetEncoding(encodingName);
            var encoded = encoding.Encode(textToEncode);
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
}
