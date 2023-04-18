using System;
using System.Collections.Generic;

namespace SharpToken
{

    public readonly struct ModelParams
    {
        public int? ExplicitNVocab { get; }
        public string PatStr { get; }
        public Dictionary<byte[], int> MergeableRanks { get; }
        public Dictionary<string, int> SpecialTokens { get; }

        public ModelParams(
            int? explicitNVocab = null,
            string patStr = null!,
            Dictionary<byte[], int> mergeableRanks = null!,
            Dictionary<string, int>? specialTokens = null)
        {
            ExplicitNVocab = explicitNVocab;
            PatStr = patStr;
            MergeableRanks = mergeableRanks;
            SpecialTokens = specialTokens ?? new Dictionary<string, int>();
        }
    }

    public static class ModelParamsGenerator
    {
        private const string EndOfText = "<|endoftext|>";
        private const string FimPrefix = "<|fim_prefix|>";
        private const string FimMiddle = "<|fim_middle|>";
        private const string FimSuffix = "<|fim_suffix|>";
        private const string EndOfPrompt = "<|endofprompt|>";

        public static ModelParams GetModelParams(string encodingName)
        {
            return encodingName.ToLower() switch
            {
                "r50k_base" => R50KBase(),
                "p50k_base" => P50KBase(),
                "p50k_edit" => P50KEdit(),
                "cl100k_base" => Cl100KBase(),
                _ => throw new ArgumentException($"Unknown encoding name: {encodingName}")
            };
        }

        private static ModelParams R50KBase()
        {
            var mergeableRanks = EmbeddedResourceReader.LoadTokenBytePairEncoding("SharpToken.data.r50k_base.tiktoken");

            return new ModelParams
            (
                50257,
                @"'s|'t|'re|'ve|'m|'ll|'d| ?\p{L}+| ?\p{N}+| ?[^\s\p{L}\p{N}]+|\s+(?!\S)|\s+",
                mergeableRanks,
                new Dictionary<string, int> {{EndOfText, 50256}}
            );
        }

        private static ModelParams P50KBase()
        {
            var mergeableRanks = EmbeddedResourceReader.LoadTokenBytePairEncoding("SharpToken.data.p50k_base.tiktoken");

            return new ModelParams
            (
                50281,
                @"'s|'t|'re|'ve|'m|'ll|'d| ?\p{L}+| ?\p{N}+| ?[^\s\p{L}\p{N}]+|\s+(?!\S)|\s+",
                mergeableRanks,
                new Dictionary<string, int> {{EndOfText, 50256}}
            );
        }

        private static ModelParams P50KEdit()
        {
            var mergeableRanks = EmbeddedResourceReader.LoadTokenBytePairEncoding("SharpToken.data.p50k_base.tiktoken");

            var specialTokens = new Dictionary<string, int>
            {
                {EndOfText, 50256}, {FimPrefix, 50281}, {FimMiddle, 50282}, {FimSuffix, 50283}
            };

            return new ModelParams
            (
                patStr: @"'s|'t|'re|'ve|'m|'ll|'d| ?\p{L}+| ?\p{N}+| ?[^\s\p{L}\p{N}]+|\s+(?!\S)|\s+",
                mergeableRanks: mergeableRanks,
                specialTokens: specialTokens
            );
        }

        private static ModelParams Cl100KBase()
        {
            var mergeableRanks =
                EmbeddedResourceReader.LoadTokenBytePairEncoding("SharpToken.data.cl100k_base.tiktoken");

            var specialTokens = new Dictionary<string, int>
            {
                {EndOfText, 100257},
                {FimPrefix, 100258},
                {FimMiddle, 100259},
                {FimSuffix, 100260},
                {EndOfPrompt, 100276}
            };

            return new ModelParams
            (
                patStr:
                @"(?i:'s|'t|'re|'ve|'m|'ll|'d)|[^\r\n\p{L}\p{N}]?\p{L}+|\p{N}{1,3}| ?[^\s\p{L}\p{N}]+[\r\n]*|\s*[\r\n]+|\s+(?!\S)|\s+",
                mergeableRanks: mergeableRanks,
                specialTokens: specialTokens
            );
        }
    }
}
