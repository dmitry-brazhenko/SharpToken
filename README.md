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
var count = encoding.TokenCount("Hello, world!"); // Output: 4
```

## Supported Models

SharpToken currently supports the following models:

* `r50k_base`
* `p50k_base`
* `p50k_edit`
* `cl100k_base`

You can use any of these models when creating an instance of GptEncoding:

```csharp
var r50kBaseEncoding = GptEncoding.GetEncoding("r50k_base");
var p50kBaseEncoding = GptEncoding.GetEncoding("p50k_base");
var p50kEditEncoding = GptEncoding.GetEncoding("p50k_edit");
var cl100kBaseEncoding = GptEncoding.GetEncoding("cl100k_base");
```

### Model Prefix Matching

Apart from specifying direct model names, SharpToken also provides functionality to map model names based on specific prefixes. This allows users to retrieve an encoding based on a model's prefix.

Here are the current supported prefixes and their corresponding encodings:

| Model Prefix        | Encoding   |
|---------------------|------------|
| `gpt-4-`            | `cl100k_base` |
| `gpt-3.5-turbo-`    | `cl100k_base` |
| `gpt-35-turbo`      | `cl100k_base` |

Examples of model names that fall under these prefixes include:
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
| Encode_SmallText         |     37.466 us |   0.2264 us |   0.2007 us |  0.35 |    0.01 |     696 B |        0.01 |
| Encode_LargeText         |  6,071.416 us |  93.7597 us |  83.1155 us |  0.28 |    0.00 |  155547 B |        0.02 |
|                          |               |             |             |       |         |           |             |
| Decode_SmallText         |      1.656 us |   0.0324 us |   0.0385 us |  0.43 |    0.01 |    2320 B |        0.98 |
| Decode_LargeText         |    455.787 us |   7.8000 us |   7.2961 us |  0.77 |    0.01 |  286979 B |        1.00 |
|                          |               |             |             |       |         |           |             |
| CountTokens_SmallText    |     37.646 us |   0.4215 us |   0.3737 us |  0.36 |    0.01 |     184 B |       0.003 |
| CountTokens_LargeText    |  5,915.175 us | 114.8400 us | 145.2358 us |  0.27 |    0.00 |     195 B |       0.000 |
|                          |               |             |             |       |         |           |             |
| **.NET 6.0**             |               |             |             |       |         |           |             |
| Encode_SmallText         |     57.181 us |   1.1231 us |   1.0505 us |  0.54 |    0.01 |   50784 B |        0.80 |
| Encode_LargeText         | 13,910.718 us | 269.9655 us | 252.5259 us |  0.63 |    0.01 | 6382274 B |        0.82 |
|                          |               |             |             |       |         |           |             |
| Decode_SmallText         |      2.630 us |   0.0352 us |   0.0275 us |  0.69 |    0.01 |    2320 B |        0.98 |
| Decode_LargeText         |    481.238 us |   3.3083 us |   2.5829 us |  0.82 |    0.01 |  286982 B |        1.00 |
|                          |               |             |             |       |         |           |             |
| CountTokens_SmallText    |     57.812 us |   1.0915 us |   1.0210 us |  0.55 |    0.02 |   50080 B |       0.790 |
| CountTokens_LargeText    | 13,744.503 us | 184.7183 us | 172.7856 us |  0.64 |    0.01 | 6122801 B |       0.806 |
|                          |               |             |             |       |         |           |             |
| **.NET Framework 4.7.1** |               |             |             |       |         |           |             |
| Encode_SmallText         |    106.342 us |   1.8675 us |   1.9178 us |  1.00 |    0.00 |   63876 B |        1.00 |
| Encode_LargeText         | 21,966.802 us | 254.4615 us | 238.0234 us |  1.00 |    0.00 | 7752789 B |        1.00 |
|                          |               |             |             |       |         |           |             |
| Decode_SmallText         |      3.803 us |   0.0579 us |   0.0542 us |  1.00 |    0.00 |    2375 B |        1.00 |
| Decode_LargeText         |    590.159 us |   6.4986 us |   6.0788 us |  1.00 |    0.00 |  287512 B |        1.00 |
|                          |               |             |             |       |         |           |             |
| CountTokens_SmallText    |    106.034 us |   2.1028 us |   2.0652 us |  1.00 |    0.00 |   63355 B |       1.000 |
| CountTokens_LargeText    | 21,604.518 us | 164.2992 us | 153.6855 us |  1.00 |    0.00 | 7597916 B |       1.000 |

## Contributions and Feedback

If you encounter any issues or have suggestions for improvements, please feel free to open an issue or submit a pull
request on the project's repository.

Hope you find SharpToken useful for your projects and welcome any feedback you may have.
