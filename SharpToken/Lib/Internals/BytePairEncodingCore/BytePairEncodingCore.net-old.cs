#if !NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpToken
{
    internal sealed class BytePairEncodingCore
    {
        public BytePairEncodingCore(
            BytePairIndex bytePairEncoder,
            Dictionary<string, int> specialTokenEncoder,
            Regex tokenPatternRegex
        )
        {
            Encoder = bytePairEncoder;
            Decoder = bytePairEncoder.ToDictionary(pair => pair.Value, pair => pair.Key);

            SpecialTokensEncoder = specialTokenEncoder;
            SpecialTokensDecoder = specialTokenEncoder.ToDictionary(pair => pair.Value, pair => Encoding.UTF8.GetBytes(pair.Key));
            RegexTls = tokenPatternRegex;
        }

        public BytePairIndex Encoder { get; }
        public Dictionary<string, int> SpecialTokensEncoder { get; }
        public Dictionary<int, byte[]> Decoder { get; }
        public Dictionary<int, byte[]> SpecialTokensDecoder { get; }
        public Regex RegexTls { get; }

        public (List<int>, int) EncodeNative(string text, ISet<string> allowedSpecial)
        {
            var encodedTokens = new List<int>();
            var startIndex = 0;
            var lastTokenLength = 0;

            var allowedSpecialTokens = allowedSpecial.Count == 0
                ? Array.Empty<string>()
                : SpecialTokensEncoder.Keys
                    .Where(allowedSpecial.Contains)
                    .ToArray();

            while (true)
            {
                var textSegment = text;

                var nextSpecialMatch = allowedSpecialTokens.FindMatch(text, startIndex);
                if (nextSpecialMatch.Success)
                {
                    var endIndex = nextSpecialMatch.Index + startIndex;
                    textSegment = text.Substring(0, endIndex - startIndex);
                }

                foreach (var match in RegexTls.Matches(textSegment, startIndex).Cast<Match>())
                {
                    var encodedPiece = Encoding.UTF8.GetBytes(match.Value);

                    lastTokenLength = 0;
                    foreach (var token in BytePairEncode(encodedPiece, Encoder))
                    {
                        encodedTokens.Add(token);
                        lastTokenLength++;
                    }
                }

                if (nextSpecialMatch.Success)
                {
                    var specialToken = nextSpecialMatch.Value;
                    var specialTokenValue = SpecialTokensEncoder[specialToken];
                    encodedTokens.Add(specialTokenValue);
                    startIndex = nextSpecialMatch.Index + specialToken.Length;
                    lastTokenLength = 0;
                }
                else
                {
                    break;
                }
            }

            return (encodedTokens, lastTokenLength);
        }

        public byte[] DecodeNative(IEnumerable<int> tokens)
        {
            using (var memoryStream = new MemoryStream())
            {
                foreach (var token in tokens)
                {
                    if (TryDecodeToken(token, out var tokenBytes) && tokenBytes != null)
                    {
                        memoryStream.Write(tokenBytes, 0, tokenBytes.Length);
                    }
                }

                return memoryStream.ToArray();
            }
        }

        private bool TryDecodeToken(int token, out byte[] tokenBytes)
        {
            return Decoder.TryGetValue(token, out tokenBytes) ||
                   SpecialTokensDecoder.TryGetValue(token, out tokenBytes);
        }

        private static IEnumerable<T> BytePairMerge<T>(byte[] piece, BytePairIndex ranks,
            Func<(int Start, int End), T> f)
        {
            var partitions = Enumerable.Range(0, piece.Length + 1)
                .Select(i => (Start: i, Rank: int.MaxValue))
                .ToList();

            int? GetRank(IReadOnlyList<(int Start, int Rank)> partitionsList, int startIndex, int skip)
            {
                if (startIndex + skip + 2 >= partitionsList.Count)
                {
                    return null;
                }

                var key = piece.Slice(partitionsList[startIndex].Start, partitionsList[startIndex + skip + 2].Start);
                return ranks.TryGetValue(key, out var rank) ? rank : (int?) null;
            }

            for (var i = 0; i < partitions.Count - 2; i++)
            {
                var rank = GetRank(partitions, i, 0);
                if (rank.HasValue)
                {
                    partitions[i] = (partitions[i].Start, rank.Value);
                }
            }

            while (partitions.Count > 1)
            {
                var minRank = int.MaxValue;
                var minRankIdx = 0;

                for (var i = 0; i < partitions.Count - 1; i++)
                {
                    if (partitions[i].Rank < minRank)
                    {
                        minRank = partitions[i].Rank;
                        minRankIdx = i;
                    }
                }

                if (minRank != int.MaxValue)
                {
                    partitions[minRankIdx] = (partitions[minRankIdx].Start,
                        GetRank(partitions, minRankIdx, 1) ?? int.MaxValue);

                    if (minRankIdx > 0)
                    {
                        partitions[minRankIdx - 1] = (partitions[minRankIdx - 1].Start,
                            GetRank(partitions, minRankIdx - 1, 1) ?? int.MaxValue);
                    }

                    partitions.RemoveAt(minRankIdx + 1);
                }
                else
                {
                    break;
                }
            }

            for (var i = 0; i < partitions.Count - 1; i++)
            {
                yield return f((partitions[i].Start, partitions[i + 1].Start));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<int> BytePairEncode(byte[] inputBytes, BytePairIndex bytePairRanks)
        {
            if (inputBytes.Length == 1)
            {
                return new int[] { bytePairRanks[inputBytes] };
            }

            return BytePairMerge(inputBytes, bytePairRanks, pair =>
            {
                var key = inputBytes.Slice(pair.Start, pair.End);
                return bytePairRanks[key];
            });
        }
    }
}

#endif
