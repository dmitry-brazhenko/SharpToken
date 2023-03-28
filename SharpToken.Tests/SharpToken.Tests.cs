using System.Diagnostics;
using System.Security.Cryptography;

namespace SharpToken.Tests;

public class Tests
{
    // private static readonly List<string> ModelsList = new() { "p50k_base", "r50k_base", "cl100k_base" };

    private static List<Tuple<string, string, List<int>>> _testData =
        TestHelpers.ReadTestPlans("SharpToken.Tests.data.TestPlans.txt");

    [SetUp]
    public void Setup()
    {
    }


    [Test]
    [TestCaseSource(nameof(_testData))]
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

    // [Test]
    // [TestCaseSource(nameof(ModelsList))]
    // public async Task TestLocalResourceMatchesRemoteResource(string modelName)
    // {
    //     var embeddedResourceName = $"SharpToken.data.{modelName}.tiktoken";
    //     var remoteResourceUrl = $"https://openaipublic.blob.core.windows.net/encodings/{modelName}.tiktoken";
    //
    //     using var sha256 = SHA256.Create();
    //
    //     // Read the embedded resource file and calculate its hash
    //     using var stream = typeof(GptEncoding).Assembly.GetManifestResourceStream(embeddedResourceName) ??
    //                        throw new InvalidOperationException();
    //     var embeddedResourceBytes = new byte[stream.Length];
    //     _ = await stream.ReadAsync(embeddedResourceBytes).ConfigureAwait(true);
    //     var embeddedResourceHash = sha256.ComputeHash(embeddedResourceBytes);
    //
    //     // Download the remote file and calculate its hash
    //     using var httpClient = new HttpClient();
    //     var remoteResourceBytes = await httpClient.GetByteArrayAsync(remoteResourceUrl).ConfigureAwait(true);
    //     var remoteResourceHash = sha256.ComputeHash(remoteResourceBytes);
    //
    //     // Compare the hashes and assert their equality
    //     Assert.That(embeddedResourceHash, Is.EqualTo(remoteResourceHash));
    // }
}
