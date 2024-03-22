#if NET8_0_OR_GREATER
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;


namespace SharpToken;

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

    public (List<int>, int) EncodeNative(ReadOnlySpan<char> text, ISet<string> allowedSpecial)
    {
        var encodedTokens = new List<int>();
        var startIndex = 0;
        var lastTokenLength = 0;
        var pool = ArrayPool<byte>.Shared;

        var allowedSpecialTokens = allowedSpecial.Count == 0
            ? Array.Empty<string>()
            : SpecialTokensEncoder.Keys
                .Where(allowedSpecial.Contains)
                .ToArray();

        while (true)
        {
            var slice = text[startIndex..];

            var nextSpecialMatch = allowedSpecialTokens.FindMatch(slice);
            if (nextSpecialMatch.Success)
            {
                slice = slice[..nextSpecialMatch.Index];
            }

            foreach (var match in RegexTls.EnumerateMatches(slice))
            {
                var segment = slice[match.Index..(match.Index + match.Length)];

                var buffer = pool.Rent(Encoding.UTF8.GetMaxByteCount(segment.Length));
                try
                {
                    var size = Encoding.UTF8.GetBytes(segment, buffer);
                    var encodedPiece = buffer.AsSpan(..size);

                    lastTokenLength = 0;
                    foreach (var token in BytePairEncode(encodedPiece))
                    {
                        encodedTokens.Add(token);
                        lastTokenLength++;
                    }
                }
                finally
                {
                    pool.Return(buffer);
                }
            }

            if (nextSpecialMatch.Success)
            {
                var specialToken = nextSpecialMatch.Value;
                var specialTokenValue = SpecialTokensEncoder[specialToken];
                encodedTokens.Add(specialTokenValue);
                startIndex += nextSpecialMatch.Index + specialToken.Length;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private List<int> BytePairEncode(ReadOnlySpan<byte> piece)
    {
        if (piece.Length == 1)
        {
            return [Encoder[piece]];
        }

        var partitions = Enumerable.Range(0, piece.Length + 1)
            .Select(i => (Start: i, Rank: int.MaxValue))
            .ToList();

        int? GetRank(ReadOnlySpan<byte> piece, IReadOnlyList<(int Start, int Rank)> partitionsList, int startIndex, int skip)
        {
            if (startIndex + skip + 2 >= partitionsList.Count)
            {
                return null;
            }

            var key = piece[partitionsList[startIndex].Start..partitionsList[startIndex + skip + 2].Start];
            return Encoder.TryGetValue(key, out var rank) ? rank : null;
        }

        for (var i = 0; i < partitions.Count - 2; i++)
        {
            var rank = GetRank(piece, partitions, i, 0);
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
                    GetRank(piece, partitions, minRankIdx, 1) ?? int.MaxValue);

                if (minRankIdx > 0)
                {
                    partitions[minRankIdx - 1] = (partitions[minRankIdx - 1].Start,
                        GetRank(piece, partitions, minRankIdx - 1, 1) ?? int.MaxValue);
                }

                partitions.RemoveAt(minRankIdx + 1);
            }
            else
            {
                break;
            }
        }

        var output = new List<int>(partitions.Count - 1);
        for (var i = 0; i < partitions.Count - 1; i++)
        {
            var key = piece[partitions[i].Start..partitions[i + 1].Start];
            var token = Encoder[key];
            output.Add(token);
        }

        return output;
    }
}

#endif
