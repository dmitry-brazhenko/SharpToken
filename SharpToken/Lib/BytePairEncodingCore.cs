using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpToken
{
    public class BytePairEncodingCore
    {
        public BytePairEncodingCore(
            Dictionary<byte[], int> bytePairEncoder = null,
            Dictionary<string, int> specialTokenEncoder = null,
            Regex tokenPatternRegex = null
        )
        {
            var comparer = new ByteArrayEqualityComparer();
            Encoder = bytePairEncoder == null
                ? new Dictionary<byte[], int>(comparer)
                : new Dictionary<byte[], int>(bytePairEncoder, comparer);
            Decoder = bytePairEncoder?.ToDictionary(pair => pair.Value, pair => pair.Key.ToArray())
                      ?? new Dictionary<int, byte[]>();

            SpecialTokensEncoder = specialTokenEncoder ?? new Dictionary<string, int>();
            SpecialTokensDecoder =
                specialTokenEncoder?.ToDictionary(pair => pair.Value, pair => Encoding.UTF8.GetBytes(pair.Key))
                ?? new Dictionary<int, byte[]>();
            RegexTls = tokenPatternRegex ?? new Regex("");

            var parts = SpecialTokensEncoder.Keys.Select(Regex.Escape).ToArray();
            var joinedParts = string.Join("|", parts);
            try
            {
                SpecialTokenPatternRegex = new Regex(joinedParts);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException("Invalid regular expression pattern.", e);
            }
        }

        public Dictionary<byte[], int> Encoder { get; }
        public Dictionary<string, int> SpecialTokensEncoder { get; }
        public Dictionary<int, byte[]> Decoder { get; }
        public Dictionary<int, byte[]> SpecialTokensDecoder { get; }
        public Regex RegexTls { get; }
        public Regex SpecialTokenPatternRegex { get; }

        public (List<int>, int) EncodeNative(string text, ISet<string> allowedSpecial)
        {
            var encodedTokens = new List<int>();
            var startIndex = 0;
            var lastTokenLength = 0;

            while (true)
            {
                var nextSpecialStartIndex =
                    FindNextSpecialStartIndex(text, allowedSpecial, startIndex, SpecialTokenPatternRegex);

                var endIndex = nextSpecialStartIndex ?? text.Length;
                var textSegment = text.Substring(startIndex, endIndex - startIndex);

                foreach (var match in RegexTls.Matches(textSegment).Cast<Match>())
                {
                    var encodedPiece = Encoding.UTF8.GetBytes(match.Value);
                    if (Encoder.TryGetValue(encodedPiece, out var token))
                    {
                        lastTokenLength = 1;
                        encodedTokens.Add(token);
                        continue;
                    }

                    var tokens = BytePairEncode(encodedPiece, Encoder).ToList();
                    lastTokenLength = tokens.Count;
                    encodedTokens.AddRange(tokens);
                }

                if (nextSpecialStartIndex.HasValue)
                {
                    var specialToken = text.Substring(nextSpecialStartIndex.Value);
                    var specialTokenValue = SpecialTokensEncoder[specialToken];
                    encodedTokens.Add(specialTokenValue);
                    startIndex = nextSpecialStartIndex.Value + specialToken.Length;
                    lastTokenLength = 0;
                }
                else
                {
                    break;
                }
            }

            return (encodedTokens, lastTokenLength);
        }

        private static int? FindNextSpecialStartIndex(string text, ISet<string> allowedSpecial, int startIndex,
            Regex specialRegex)
        {
            var searchIndex = startIndex;

            while (true)
            {
                var nextSpecialMatch = specialRegex.Match(text, searchIndex);

                if (!nextSpecialMatch.Success)
                {
                    return null;
                }

                var specialToken = nextSpecialMatch.Value;

                if (allowedSpecial.Contains(specialToken))
                {
                    return nextSpecialMatch.Index + searchIndex;
                }

                searchIndex = nextSpecialMatch.Index + searchIndex + 1;
            }
        }

        public List<byte> DecodeNative(int[] tokens)
        {
            var decodedBytes = new List<byte>(tokens.Length * 2);
            foreach (var token in tokens)
            {
                if (!TryDecodeToken(token, out var tokenBytes))
                {
                    continue;
                }

                if (tokenBytes != null)
                {
                    decodedBytes.AddRange(tokenBytes);
                }
            }

            return decodedBytes;
        }

        private bool TryDecodeToken(int token, out byte[] tokenBytes)
        {
            return Decoder.TryGetValue(token, out tokenBytes) ||
                   SpecialTokensDecoder.TryGetValue(token, out tokenBytes);
        }

        private static T[] BytePairMerge<T>(IReadOnlyCollection<byte> piece, IReadOnlyDictionary<byte[], int> ranks,
            Func<(int Start, int End), T> f)
        {
            var partitions = Enumerable.Range(0, piece.Count + 1)
                .Select(i => (Start: i, Rank: int.MaxValue))
                .ToList();

            int? GetRank(IReadOnlyList<(int Start, int Rank)> partitionsList, int startIndex, int skip)
            {
                if (startIndex + skip + 2 >= partitionsList.Count)
                {
                    return null;
                }

                var key = piece.Skip(partitionsList[startIndex].Start)
                    .Take(partitionsList[startIndex + skip + 2].Start - partitionsList[startIndex].Start)
                    .ToArray();

                return ranks.TryGetValue(key, out var rank) ? rank : (int?)null;
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

            var output = new List<T>(partitions.Count - 1);
            for (var i = 0; i < partitions.Count - 1; i++)
            {
                output.Add(f((partitions[i].Start, partitions[i + 1].Start)));
            }

            return output.ToArray();
        }

        private static IEnumerable<int> BytePairEncode(byte[] inputBytes, Dictionary<byte[], int> bytePairRanks)
        {
            if (inputBytes.Length == 1)
            {
                return new List<int> { bytePairRanks[inputBytes] }.ToArray();
            }

            return BytePairMerge(inputBytes, bytePairRanks, pair =>
            {
                var key = inputBytes.Skip(pair.Start).Take(pair.End - pair.Start).ToArray();
                return bytePairRanks[key];
            });
        }
    }

    internal sealed class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] x, byte[] y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return ReferenceEquals(x, y) || StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
        }

        public int GetHashCode(byte[] obj)
        {
            return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
        }
    }
}
