# SharpToken

SharpToken is a C# library that serves as a port of the Python [tiktoken](https://github.com/openai/tiktoken) library. It provides functionality for encoding and decoding tokens using GPT-based encodings. This library is built for .NET 6 and .NET Standard 2.1, making it compatible with a wide range of frameworks.

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

## Understanding Encoded Values
When you encode a string using the Encode method, the returned value is a list of integers that represent tokens in the specified encoding. These tokens are a compact way of representing the input text and can be processed more efficiently by various algorithms.

For example, encoding the text "Hello world!" using the cl100k_base encoding might produce the following list of integers:

```csharp
var encoded = cl100kBaseEncoding.Encode("Hello world!"); // Output: [9906, 1917, 0]
```
You can then use the `Decode` method to convert these tokenized integer values back into the original text:

```csharp
var decoded = cl100kBaseEncoding.Decode(encoded); // Output: "Hello world!"
```

With SharpToken, you can seamlessly switch between different encodings to find the one that best suits your needs. Just remember to use the same encoding for both the `Encode` and `Decode` methods to ensure accurate results.

## Advanced usage
### Custom Allowed Sets
   SharpToken allows you to specify custom sets of allowed special tokens when encoding text. To do this, pass a HashSet<string> containing the allowed special tokens as a parameter to the Encode method:
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
Similarly, you can specify custom sets of disallowed special tokens when encoding text. Pass a `HashSet<string>` containing the disallowed special tokens as a parameter to the Encode method:
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

SharpToken includes a set of test cases in the [TestPlans.txt](SharpToken.Tests/data/TestPlans.txt) file to ensure its compatibility with the Python tiktoken library. These test cases validate the functionality and behavior of SharpToken, providing a reliable reference for developers. Running the unit tests and verifying the test cases helps maintain consistency between the C# SharpToken library and the original Python implementation.

## Contributions and Feedback
If you encounter any issues or have suggestions for improvements, please feel free to open an issue or submit a pull request on the project's repository.

Hope you find SharpToken useful for your projects and welcome any feedback you may have.
