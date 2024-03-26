using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SharpToken.Tests
{
    public static class TestHelpers
    {
        public static List<Tuple<string, string, List<int>>> ReadTestPlans(string resourceName)
        {
            var testPlans = new List<Tuple<string, string, List<int>>>();
            var assembly = Assembly.GetExecutingAssembly();

            using var stream = assembly.GetManifestResourceStream(resourceName) ??
                               throw new InvalidOperationException();
            using var reader = new StreamReader(stream);

            while (reader.ReadLine() is { } line)
            {
                if (line.StartsWith("EncodingName: "))
                {
                    var encodingName = line.Substring("EncodingName: ".Length);
                    var sample = reader.ReadLine()!.Substring("Sample: ".Length);
                    var encodedStr = reader.ReadLine()!.Substring("Encoded: ".Length);

                    var encoded = Regex.Matches(encodedStr, @"\d+")
                        .Cast<Match>()
                        .Select(m => int.Parse(m.Value, CultureInfo.InvariantCulture))
                        .ToList();

                    testPlans.Add(Tuple.Create(encodingName, sample, encoded));
                }
            }

            return testPlans;
        }
    }
}
