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

    public List<int> EncodeNative(ReadOnlySpan<char> text, IReadOnlyCollection<string> allowedSpecialTokens)
    {
        var encodedTokens = new List<int>();
        var startIndex = 0;
        var pool = ArrayPool<byte>.Shared;

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
                    var piece = buffer.AsSpan(..size);

                    if (piece.Length == 1)
                    {
                        encodedTokens.Add(Encoder[piece]);
                        continue;
                    }

                    var bytePairEncoder = new MultiBytePairEncoder(piece, Encoder);
                    var tokens = bytePairEncoder.GetTokens();

                    foreach (var token in tokens)
                    {
                        encodedTokens.Add(token);
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
            }
            else
            {
                break;
            }
        }

        return encodedTokens;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryDecodeToken(int token, out byte[] tokenBytes)
    {
        return Decoder.TryGetValue(token, out tokenBytes) ||
               SpecialTokensDecoder.TryGetValue(token, out tokenBytes);
    }
}

#endif
