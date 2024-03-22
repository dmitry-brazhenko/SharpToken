using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SharpToken
{
    internal readonly struct ModelParams
    {
        public int? ExplicitNVocab { get; }
        public Regex TokenizerRegex { get; }
        public BytePairIndex MergeableRanks { get; }
        public Dictionary<string, int> SpecialTokens { get; }

        public ModelParams(
            int? explicitNVocab = null,
            Regex tokenizerRegex = null,
            BytePairIndex mergeableRanks = null,
            Dictionary<string, int> specialTokens = null)
        {
            ExplicitNVocab = explicitNVocab;
            TokenizerRegex = tokenizerRegex;
            MergeableRanks = mergeableRanks;
            SpecialTokens = specialTokens ?? new Dictionary<string, int>();
        }
    }

    internal static class ModelParamsGenerator
    {
        private const string EndOfText = "<|endoftext|>";
        private const string FimPrefix = "<|fim_prefix|>";
        private const string FimMiddle = "<|fim_middle|>";
        private const string FimSuffix = "<|fim_suffix|>";
        private const string EndOfPrompt = "<|endofprompt|>";

        private static readonly ConcurrentDictionary<string, ModelParams> Cache = new ConcurrentDictionary<string, ModelParams>();

        public static ModelParams GetModelParams(string encodingName)
        {
            return Cache.GetOrAdd(encodingName.ToLower(), key =>
            {
                switch (encodingName.ToLower())
                {
                    case "r50k_base":
                        return R50KBase();

                    case "p50k_base":
                        return P50KBase();

                    case "p50k_edit":
                        return P50KEdit();

                    case "cl100k_base":
                        return Cl100KBase();

                    default:
                        throw new ArgumentException($"Unknown encoding name: {encodingName}");
                }
            });
        }

        private static ModelParams R50KBase()
        {
            var mergeableRanks = EmbeddedResourceReader.LoadTokenBytePairEncoding("SharpToken.data.r50k_base.tiktoken");

            return new ModelParams
            (
                explicitNVocab: 50257,
                tokenizerRegex: ModelParamsGeneratorRegex.Regex50KBase(),
                mergeableRanks: mergeableRanks,
                specialTokens: new Dictionary<string, int> { { EndOfText, 50256 } }
            );
        }

        private static ModelParams P50KBase()
        {
            var mergeableRanks = EmbeddedResourceReader.LoadTokenBytePairEncoding("SharpToken.data.p50k_base.tiktoken");

            return new ModelParams
            (
                explicitNVocab: 50281,
                tokenizerRegex: ModelParamsGeneratorRegex.Regex50KBase(),
                mergeableRanks: mergeableRanks,
                specialTokens: new Dictionary<string, int> { { EndOfText, 50256 } }
            );
        }

        private static ModelParams P50KEdit()
        {
            var mergeableRanks = EmbeddedResourceReader.LoadTokenBytePairEncoding("SharpToken.data.p50k_base.tiktoken");

            var specialTokens = new Dictionary<string, int> { { EndOfText, 50256 }, { FimPrefix, 50281 }, { FimMiddle, 50282 }, { FimSuffix, 50283 } };

            return new ModelParams
            (
                tokenizerRegex: ModelParamsGeneratorRegex.Regex50KBase(),
                mergeableRanks: mergeableRanks,
                specialTokens: specialTokens
            );
        }

        private static ModelParams Cl100KBase()
        {
            var mergeableRanks = EmbeddedResourceReader.LoadTokenBytePairEncoding("SharpToken.data.cl100k_base.tiktoken");

            var specialTokens = new Dictionary<string, int>
            {
                { EndOfText, 100257 },
                { FimPrefix, 100258 },
                { FimMiddle, 100259 },
                { FimSuffix, 100260 },
                { EndOfPrompt, 100276 }
            };

            return new ModelParams
            (
                tokenizerRegex: ModelParamsGeneratorRegex.RegexCl100KBase(),
                mergeableRanks: mergeableRanks,
                specialTokens: specialTokens
            );
        }
    }

    internal sealed partial class ModelParamsGeneratorRegex
    {
#if NET8_0_OR_GREATER
        [GeneratedRegex(@"'s|'t|'re|'ve|'m|'ll|'d| ?\p{L}+| ?\p{N}+| ?[^\s\p{L}\p{N}]+|\s+(?!\S)|\s+")]
        public static partial Regex Regex50KBase();

        [GeneratedRegex(@"(?i:'s|'t|'re|'ve|'m|'ll|'d)|[^\r\n\p{L}\p{N}]?\p{L}+|\p{N}{1,3}| ?[^\s\p{L}\p{N}]+[\r\n]*|\s*[\r\n]+|\s+(?!\S)|\s+")]
        public static partial Regex RegexCl100KBase();
#else
        public static Regex Regex50KBase() => new Regex(@"'s|'t|'re|'ve|'m|'ll|'d| ?\p{L}+| ?\p{N}+| ?[^\s\p{L}\p{N}]+|\s+(?!\S)|\s+", RegexOptions.Compiled);

        public static Regex RegexCl100KBase() => new Regex(@"(?i:'s|'t|'re|'ve|'m|'ll|'d)|[^\r\n\p{L}\p{N}]?\p{L}+|\p{N}{1,3}| ?[^\s\p{L}\p{N}]+[\r\n]*|\s*[\r\n]+|\s+(?!\S)|\s+", RegexOptions.Compiled);
#endif
    }
}
