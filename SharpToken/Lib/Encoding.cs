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

        private GptEncoding(
            Regex tokenizerRegex,
            BytePairIndex bytePairRanks,
            Dictionary<string, int> specialTokenMappings,
            int? explicitNVocab = null
        )
        {
            var maxTokenValue = Math.Max(
                GetMaxValueFromBytePairRanks(bytePairRanks),
                GetMaxValueFromSpecialToken(specialTokenMappings)
            );

            _specialTokenMappings = specialTokenMappings;

            if (explicitNVocab.HasValue)
            {
                if (bytePairRanks.Count + specialTokenMappings.Count != explicitNVocab.Value)
                {
                    throw new ArgumentException(
                        "The number of mergeable tokens and special tokens must be equal to explicit_n_vocab.");
                }

                if (maxTokenValue != explicitNVocab.Value - 1)
                {
                    throw new ArgumentException("The maximum token value must be equal to explicit_n_vocab - 1.");
                }
            }

            _bytePairEncodingCoreProcessor = new BytePairEncodingCore(bytePairRanks, specialTokenMappings, tokenizerRegex);

            int GetMaxValueFromBytePairRanks(BytePairIndex dictionary)
            {
                return dictionary.Select(_ => _.Value).Prepend(0).Max();
            }

            int GetMaxValueFromSpecialToken(Dictionary<string, int> dictionary)
            {
                return dictionary.Values.Prepend(0).Max();
            }
        }

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

#if NET8_0_OR_GREATER
        // keep this overload because it was part of previous public API:
        public List<int> Encode(string lineToEncode, ISet<string> allowedSpecial = null, ISet<string> disallowedSpecial = null)
        {
            return Encode(lineToEncode.AsSpan(), allowedSpecial, disallowedSpecial);
        }

        public List<int> Encode(ReadOnlySpan<char> lineToEncode, ISet<string> allowedSpecial = null, ISet<string> disallowedSpecial = null)
        {
#else
        public List<int> Encode(string lineToEncode, ISet<string> allowedSpecial = null, ISet<string> disallowedSpecial = null)
        {
#endif
            var allowedSpecialTokens = allowedSpecial is null
                // When null allow nothing
                ? Array.Empty<string>()
                : allowedSpecial.Contains("all")
                    ? (IReadOnlyCollection<string>) _specialTokenMappings.Keys
                    // filter / validate list to only known special tokens:
                    : (IReadOnlyCollection<string>) _specialTokenMappings.Keys.Where(allowedSpecial.Contains).ToArray();

            var disallowedSpecialTokens = disallowedSpecial == null || disallowedSpecial.Contains("all")
                // When null or all -> initialize with all except allowed
                ? allowedSpecial is null
                    ? _specialTokenMappings.Keys
                    : _specialTokenMappings.Keys.Where(_ => !allowedSpecial.Contains(_))
                // Else use provided list
                : disallowedSpecial;

            var match = disallowedSpecialTokens.FindMatch(lineToEncode);
            if (match.Success)
            {
                throw new ArgumentException($"Disallowed special token found: {match.Value}");
            }

            var encodedLine = _bytePairEncodingCoreProcessor.EncodeNative(lineToEncode, allowedSpecialTokens);
            return encodedLine;
        }

        // keep this overload because it was part of previous public API:
        public string Decode(List<int> inputTokensToDecode)
        {
            return Decode((IEnumerable<int>) inputTokensToDecode);
        }

        public string Decode(IEnumerable<int> inputTokensToDecode)
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


    }
}
