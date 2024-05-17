# SharpToken

[![NuGet](https://img.shields.io/nuget/v/SharpToken.svg)](https://www.nuget.org/packages/SharpToken)
[![dotnet](https://github.com/dmitry-brazhenko/SharpToken/actions/workflows/build-test-and-publish.yml/badge.svg?branch=main)](https://github.com/dmitry-brazhenko/SharpToken/actions/workflows/build-test-and-publish.yml)
[![Last Commit](https://img.shields.io/github/last-commit/dmitry-brazhenko/SharpToken.svg)](https://github.com/dmitry-brazhenko/SharpToken/commits/main)
[![GitHub Issues](https://img.shields.io/github/issues/dmitry-brazhenko/SharpToken.svg)](https://github.com/dmitry-brazhenko/SharpToken/issues)
[![Used by](https://img.shields.io/nuget/dt/SharpToken.svg)](https://www.nuget.org/packages/SharpToken)
[![Contributors](https://img.shields.io/github/contributors/dmitry-brazhenko/SharpToken.svg)](https://github.com/dmitry-brazhenko/SharpToken/graphs/contributors)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)



SharpToken is a C# library that serves as a port of the Python [tiktoken](https://github.com/openai/tiktoken) library.
It provides functionality for encoding and decoding tokens using GPT-based encodings. This library is built for .NET 6, .NET 8
and .NET Standard 2.0, making it compatible with a wide range of frameworks.

> [!Important]
> The functionality in `SharpToken` has been added to [`Microsoft.ML.Tokenizers`](https://www.nuget.org/packages/Microsoft.ML.Tokenizers). `Microsoft.ML.Tokenizers` is a tokenizer library being developed by the .NET team and going forward, the central place for tokenizer development in .NET. By using `Microsoft.ML.Tokenizers`, you should see improved performance over existing tokenizer library implementations, including `SharpToken`. A stable release of `Microsoft.ML.Tokenizers` is expected alongside the .NET 9.0 release (November 2024). Instructions for migration can be found at https://github.com/dotnet/machinelearning/blob/main/docs/code/microsoft-ml-tokenizers-migration-guide.md.

## Installation

To install SharpToken, use the NuGet package manager:

```powershell
Install-Package SharpToken
```

Or, if you prefer using the .NET CLI:

```powershell
dotnet add package SharpToken
```

For more information, visit the [NuGet package page](https://www.nuget.org/packages/SharpToken).

## Usage

To use SharpToken in your project, first import the library:

```csharp
using SharpToken;
```

Next, create an instance of GptEncoding by specifying the desired encoding or model:

```csharp
// Get encoding by encoding name
var encoding = GptEncoding.GetEncoding("cl100k_base");

// Get encoding by model name
var encoding = GptEncoding.GetEncodingForModel("gpt-4");
```

You can then use the Encode method to encode a string:

```csharp
var encoded = encoding.Encode("Hello, world!"); // Output: [9906, 11, 1917, 0]
```

And use the Decode method to decode the encoded tokens:

```csharp
var decoded = encoding.Decode(encoded); // Output: "Hello, world!"
```

SharpToken also provides a high performance count method.
It is usefull to check prompt size before sending it to a LLM or to use it in a TextSplitter/Chunker for RAG.

```csharp
var count = encoding.CountTokens("Hello, world!"); // Output: 4
```

## Supported Models

SharpToken currently supports the following models:

* `r50k_base`
* `p50k_base`
* `p50k_edit`
* `cl100k_base`
* `o200k_base`

You can use any of these models when creating an instance of GptEncoding:

```csharp
var r50kBaseEncoding = GptEncoding.GetEncoding("r50k_base");
var p50kBaseEncoding = GptEncoding.GetEncoding("p50k_base");
var p50kEditEncoding = GptEncoding.GetEncoding("p50k_edit");
var cl100kBaseEncoding = GptEncoding.GetEncoding("cl100k_base");
var o200kBaseEncoding = GptEncoding.GetEncoding("o200k_base");
```

### Model Prefix Matching

Apart from specifying direct model names, SharpToken also provides functionality to map model names based on specific prefixes. This allows users to retrieve an encoding based on a model's prefix.

Here are the current supported prefixes and their corresponding encodings:

| Model Prefix        | Encoding   |
|---------------------|------------|
| `gpt-4o`            | `o200k_base` |
| `gpt-4-`            | `cl100k_base` |
| `gpt-3.5-turbo-`    | `cl100k_base` |
| `gpt-35-turbo`      | `cl100k_base` |

Examples of model names that fall under these prefixes include:
- For the prefix `gpt-4o`: `gpt-4o`, `gpt-4o-2024-05-13`, etc.
- For the prefix `gpt-4-`: `gpt-4-0314`, `gpt-4-32k`, etc.
- For the prefix `gpt-3.5-turbo-`: `gpt-3.5-turbo-0301`, `gpt-3.5-turbo-0401`, etc.
- For the Azure deployment name `gpt-35-turbo`.

To retrieve the encoding name based on a model name or its prefix, you can use the `GetEncodingNameForModel` method:

```csharp
string encodingName = Model.GetEncodingNameForModel("gpt-4-0314");  // This will return "cl100k_base"
```

If the provided model name doesn't match any direct model names or prefixes, the method will return `null`.




## Understanding Encoded Values

When you encode a string using the Encode method, the returned value is a list of integers that represent tokens in the
specified encoding. These tokens are a compact way of representing the input text and can be processed more efficiently
by various algorithms.

For example, encoding the text "Hello world!" using the cl100k_base encoding might produce the following list of
integers:

```csharp
var encoded = cl100kBaseEncoding.Encode("Hello world!"); // Output: [9906, 1917, 0]
```

You can then use the `Decode` method to convert these tokenized integer values back into the original text:

```csharp
var decoded = cl100kBaseEncoding.Decode(encoded); // Output: "Hello world!"
```

With SharpToken, you can seamlessly switch between different encodings to find the one that best suits your needs. Just
remember to use the same encoding for both the `Encode` and `Decode` methods to ensure accurate results.

## Advanced usage

### Custom Allowed Sets

SharpToken allows you to specify custom sets of allowed special tokens when encoding text. To do this, pass a
HashSet<string> containing the allowed special tokens as a parameter to the Encode method:

```csharp
const string encodingName = "cl100k_base";
const string inputText = "Some Text <|endofprompt|>";
var allowedSpecialTokens = new HashSet<string> { "<|endofprompt|>" };

var encoding = GptEncoding.GetEncoding(encodingName);
var encoded = encoding.Encode(inputText, allowedSpecialTokens);
var expectedEncoded = new List<int> { 8538, 2991, 220, 100276 };

Assert.Equal(expectedEncoded, encoded);
```

### Custom Disallowed Sets

Similarly, you can specify custom sets of disallowed special tokens when encoding text. Pass a `HashSet<string>`
containing the disallowed special tokens as a parameter to the Encode method:

```csharp
const string encodingName = "cl100k_base";
const string inputText = "Some Text";

var encoding = GptEncoding.GetEncoding(encodingName);

void TestAction()
{
    encoding.Encode(inputText, disallowedSpecial: new HashSet<string> { "Some" });
}

Assert.Throws<ArgumentException>(TestAction);
```

In this example, an `ArgumentException` is thrown because the input text contains a disallowed special token

## Testing and Validation

SharpToken includes a set of test cases in the [TestPlans.txt](SharpToken.Tests/data/TestPlans.txt) file to ensure its
compatibility with the Python tiktoken library. These test cases validate the functionality and behavior of SharpToken,
providing a reliable reference for developers. Running the unit tests and verifying the test cases helps maintain
consistency between the C# SharpToken library and the original Python implementation.

## Performance Compared to TiktokenSharp and TokenizerLib

SharpToken is the fastest library with the lowest allocations!

<details>
<summary>Benchmark Code</summary>

```csharp
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

    [GlobalSetup]
    public async Task Setup()
    {
        _sharpToken = GptEncoding.GetEncoding("cl100k_base");
        _tikToken = await TikToken.GetEncodingAsync("cl100k_base").ConfigureAwait(false);
        _tokenizer = await TokenizerBuilder.CreateByModelNameAsync("gpt-4").ConfigureAwait(false);
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
```

</details>

```
BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, Windows 11 (10.0.22631.3296)
11th Gen Intel Core i9-11950H 2.60GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100-preview.2.24157.14
  [Host]               : .NET 8.0.3 (8.0.324.11423), X64 RyuJIT AVX2
  .NET 6.0             : .NET 6.0.28 (6.0.2824.12007), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.3 (8.0.324.11423), X64 RyuJIT AVX2
  .NET Framework 4.7.1 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256
```

| Method            | Job                  | Runtime              | Mean      | Error    | StdDev    | Median    | Gen0       | Gen1      | Allocated |
|------------------ |--------------------- |--------------------- |----------:|---------:|----------:|----------:|-----------:|----------:|----------:|
| **MLTokenizers**  | .NET 8.0             | .NET 8.0             |  60.55 ms | 1.143 ms |  1.123 ms |  60.45 ms |  1000.0000 |         - |  13.12 MB |
| **MLTokenizers**  | .NET 6.0             | .NET 6.0             |  95.75 ms | 1.374 ms |  1.147 ms |  95.54 ms | 10500.0000 |         - | 126.19 MB |
| **MLTokenizers**  | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 291.77 ms | 5.811 ms | 11.195 ms | 291.64 ms | 21000.0000 |         - | 127.33 MB |
|                   |                      |                      |           |          |           |           |            |           |           |
| *SharpToken*      | .NET 8.0             | .NET 8.0             |  87.78 ms | 1.700 ms |  1.590 ms |  87.34 ms |  1000.0000 |         - |  22.13 MB |
| *SharpToken*      | .NET 6.0             | .NET 6.0             | 128.84 ms | 1.718 ms |  1.607 ms | 128.17 ms | 16250.0000 |  500.0000 | 196.31 MB |
| *SharpToken*      | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 356.21 ms | 6.843 ms | 10.854 ms | 355.09 ms | 34000.0000 | 1000.0000 | 204.39 MB |
|                   |                      |                      |           |          |           |           |            |           |           |
| *TokenizerLib*    | .NET 8.0             | .NET 8.0             | 109.26 ms | 2.082 ms |  4.482 ms | 107.90 ms | 18200.0000 |  600.0000 | 217.82 MB |
| *TokenizerLib*    | .NET 6.0             | .NET 6.0             | 126.16 ms | 2.959 ms |  8.630 ms | 122.34 ms | 18000.0000 |  500.0000 | 217.82 MB |
| *TokenizerLib*    | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 374.71 ms | 7.374 ms | 16.794 ms | 370.12 ms | 40000.0000 | 1000.0000 | 243.79 MB |
|                   |                      |                      |           |          |           |           |            |           |           |
| *TiktokenSharp*   | .NET 8.0             | .NET 8.0             | 177.34 ms | 3.506 ms |  8.797 ms | 174.98 ms | 28000.0000 | 1000.0000 | 338.98 MB |
| *TiktokenSharp*   | .NET 6.0             | .NET 6.0             | 196.17 ms | 3.912 ms |  8.422 ms | 195.52 ms | 26000.0000 |  666.6667 | 313.26 MB |
| *TiktokenSharp*   | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 488.22 ms | 9.696 ms | 15.931 ms | 487.17 ms | 63000.0000 | 1000.0000 | 378.31 MB |

## Performance

SharpToken is extreamly performance optimized on net8.0.
It uses modern multibyte CPU instructions and almost no heap allocations.

All core methods have been tested on a large and a small input text.

**Inputs:**
- `SmallText`: 453 B (text/plain)
- `LargeText`: 51 KB (text/html)

**Methods:**
- `Encode`: text to tokens
- `Decode`: tokens to text
- `CountTokens`: high performance API to count tokens of text


```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3296/23H2/2023Update/SunValley3)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.200
  [Host]               : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET 6.0             : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET Framework 4.7.1 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256
```

| Method                   | Mean          | Error       | StdDev      | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------------------- |--------------:|------------:|------------:|------:|--------:|----------:|------------:|
| **.NET 8.0**             |               |             |             |       |         |           |             |
| Encode_SmallText         |     22.649 us |   0.4244 us |   0.4359 us |  0.28 |    0.01 |     696 B |        0.02 |
| Encode_LargeText         |  4,542.505 us |  87.7988 us | 104.5182 us |  0.24 |    0.01 |  155547 B |        0.03 |
|                          |               |             |             |       |         |           |             |
| Decode_SmallText         |      1.623 us |   0.0324 us |   0.0373 us |  0.44 |    0.02 |    2320 B |        0.98 |
| Decode_LargeText         |    454.570 us |   6.8980 us |   6.4524 us |  0.80 |    0.02 |  286979 B |        1.00 |
|                          |               |             |             |       |         |           |             |
| CountTokens_SmallText    |     22.008 us |   0.1165 us |   0.0909 us |  0.28 |    0.00 |     184 B |       0.005 |
| CountTokens_LargeText    |  4,231.353 us |  14.5157 us |  11.3329 us |  0.23 |    0.00 |     195 B |       0.000 |
|                          |               |             |             |       |         |           |             |
| **.NET 6.0**             |               |             |             |       |         |           |             |
| Encode_SmallText         |     36.370 us |   0.7178 us |   1.0962 us |  0.45 |    0.02 |   37344 B |        0.91 |
| Encode_LargeText         | 11,213.070 us | 219.6291 us | 269.7243 us |  0.59 |    0.02 | 5062574 B |        0.91 |
|                          |               |             |             |       |         |           |             |
| Decode_SmallText         |      2.588 us |   0.0394 us |   0.0350 us |  0.70 |    0.02 |    2320 B |        0.98 |
| Decode_LargeText         |    489.467 us |   8.9195 us |   8.3433 us |  0.86 |    0.02 |  286985 B |        1.00 |
|                          |               |             |             |       |         |           |             |
| CountTokens_SmallText    |     34.758 us |   0.2027 us |   0.1896 us |  0.45 |    0.01 |   36832 B |       0.907 |
| CountTokens_LargeText    | 11,252.083 us | 215.8912 us | 212.0340 us |  0.61 |    0.01 | 4907169 B |       0.907 |
|                          |               |             |             |       |         |           |             |
| **.NET Framework 4.7.1** |               |             |             |       |         |           |             |
| Encode_SmallText         |     79.947 us |   1.5621 us |   3.0097 us |  1.00 |    0.00 |   41138 B |        1.00 |
| Encode_LargeText         | 18,961.252 us | 253.1816 us | 236.8262 us |  1.00 |    0.00 | 5567685 B |        1.00 |
|                          |               |             |             |       |         |           |             |
| Decode_SmallText         |      3.723 us |   0.0728 us |   0.0997 us |  1.00 |    0.00 |    2375 B |        1.00 |
| Decode_LargeText         |    570.787 us |  11.0356 us |  11.8080 us |  1.00 |    0.00 |  287496 B |        1.00 |
|                          |               |             |             |       |         |           |             |
| CountTokens_SmallText    |     77.521 us |   1.0802 us |   0.9020 us |  1.00 |    0.00 |   40616 B |       1.000 |
| CountTokens_LargeText    | 18,485.392 us | 313.5834 us | 277.9836 us |  1.00 |    0.00 | 5413237 B |       1.000 |

## Contributions and Feedback

If you encounter any issues or have suggestions for improvements, please feel free to open an issue or submit a pull
request on the project's repository.

Hope you find SharpToken useful for your projects and welcome any feedback you may have.
