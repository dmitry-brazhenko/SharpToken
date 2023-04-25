using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SharpToken
{

    public static class EmbeddedResourceReader
    {
        private static IEnumerable<string> ReadEmbeddedResourceAsLines(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var stream = assembly.GetManifestResourceStream(resourceName) ??
                               throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
            var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();

            reader.Close();
            reader.Dispose();
            stream.Close();
            stream.Dispose();

            return content.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);
        }

        public static Dictionary<byte[], int> LoadTokenBytePairEncoding(string dataSourceName)
        {
            var contents = ReadEmbeddedResourceAsLines(dataSourceName).Where(line => !string.IsNullOrEmpty(line))
                .Select(line => line.Split(' ').Where(l=> !string.IsNullOrWhiteSpace(l)).ToArray()).ToArray();

            return contents.ToDictionary(
                splitLine => Convert.FromBase64String(splitLine[0]),
                splitLine => int.Parse(splitLine[1], CultureInfo.InvariantCulture)
            );
        }
    }
}
