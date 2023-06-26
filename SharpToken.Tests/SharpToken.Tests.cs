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
