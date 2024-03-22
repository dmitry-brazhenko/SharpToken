using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpToken
{
    public class GptEncoding
    {
        private readonly BytePairEncodingCore _bytePairEncodingCoreProcessor;
        private readonly Dictionary<string, int> _specialTokenMappings;

        private GptEncoding(Regex tokenizerRegex,
            BytePairIndex bytePairRanks,
            Dictionary<string, int> specialTokenMappings,
            int? explicitNVocab = null)
        {
            MaxTokenValue = Math.Max(
                GetMaxValueFromDictionary(bytePairRanks),
                GetMaxValueFromDictionary(specialTokenMappings)
            );
            _specialTokenMappings = specialTokenMappings;

            if (explicitNVocab.HasValue)
            {
                if (bytePairRanks.Count + specialTokenMappings.Count != explicitNVocab.Value)
                {
                    throw new ArgumentException(
                        "The number of mergeable tokens and special tokens must be equal to explicit_n_vocab.");
                }

                if (MaxTokenValue != explicitNVocab.Value - 1)
                {
                    throw new ArgumentException("The maximum token value must be equal to explicit_n_vocab - 1.");
                }
            }

            _bytePairEncodingCoreProcessor = new BytePairEncodingCore(bytePairRanks, specialTokenMappings, tokenizerRegex);
        }

        private int MaxTokenValue { get; }

        public static GptEncoding GetEncoding(string encodingName)
        {
            var modelParams = ModelParamsGenerator.GetModelParams(encodingName);

            var encoding = new GptEncoding(
                modelParams.TokenizerRegex,
                modelParams.MergeableRanks,
                modelParams.SpecialTokens,
                modelParams.ExplicitNVocab
            );

            return encoding;
        }

        public static GptEncoding GetEncodingForModel(string modelName)
        {
            var encodingName = Model.GetEncodingNameForModel(modelName);
            return GetEncoding(encodingName);
        }

        private static string SpecialTokenRegex(ISet<string> tokens)
        {
            var escapedTokens = tokens.Select(Regex.Escape);
            var inner = string.Join("|", escapedTokens);
            return $"({inner})";
        }

        public List<int> Encode(string lineToEncode,
            ISet<string> allowedSpecial = null,
            ISet<string> disallowedSpecial = null)
        {
            var specialTokensSet = new HashSet<string>(_specialTokenMappings.Keys);

            if (allowedSpecial == null)
            {
                allowedSpecial = new HashSet<string>();
            }

            if (disallowedSpecial == null)
            {
                disallowedSpecial = new HashSet<string> { "all" };
            }

            if (disallowedSpecial.Contains("all"))
            {
                disallowedSpecial = new HashSet<string>(specialTokensSet);
                disallowedSpecial.ExceptWith(allowedSpecial);
            }

            if (allowedSpecial.Contains("all"))
            {
                allowedSpecial = specialTokensSet;
            }

            if (disallowedSpecial.Count > 0)
            {
                var regexPattern = SpecialTokenRegex(disallowedSpecial);
                var match = Regex.Match(lineToEncode, regexPattern);
                if (match.Success)
                {
                    throw new ArgumentException($"Disallowed special token found: {match.Value}");
                }
            }

            var encodedLine = _bytePairEncodingCoreProcessor.EncodeNative(lineToEncode, allowedSpecial);
            return encodedLine.Item1;
        }

        public string Decode(List<int> inputTokensToDecode)
        {
            // Validate the input parameter
            if (inputTokensToDecode == null)
            {
                throw new ArgumentNullException(nameof(inputTokensToDecode), "Input tokens cannot be null.");
            }

            // Decode tokens
            var decodedBytes = _bytePairEncodingCoreProcessor.DecodeNative(inputTokensToDecode);

            // Convert bytes to UTF-8 string
            return Encoding.UTF8.GetString(decodedBytes);
        }

        private static int GetMaxValueFromDictionary(BytePairIndex dictionary)
        {
            return dictionary.Select(_ => _.Value).Prepend(0).Max();
        }

        private static int GetMaxValueFromDictionary(Dictionary<string, int> dictionary)
        {
            return dictionary.Values.Prepend(0).Max();
        }
    }
}
