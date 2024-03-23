#if !NET8_0_OR_GREATER
#if NET
using System.Buffers;
#endif
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

        public (List<int> tokens, int count) EncodeNative(string text, IReadOnlyCollection<string> allowedSpecialTokens, bool countOnly)
        {
            var encodedTokens = countOnly ? null : new List<int>((int) Math.Ceiling(text.Length / 4d) is var capacity && capacity > 4 ? capacity : 4);
            var startIndex = 0;
            var count = 0;
#if NET
            var pool = ArrayPool<byte>.Shared;
#endif

            while (true)
            {
                var slice = text;

                var nextSpecialMatch = allowedSpecialTokens.FindMatch(text, startIndex);
                if (nextSpecialMatch.Success)
                {
                    var endIndex = nextSpecialMatch.Index + startIndex;
                    slice = text.Substring(0, endIndex - startIndex);
                }

                foreach (var match in RegexTls.Matches(slice, startIndex).Cast<Match>())
                {
                    var segment = match.Value;
#if NET
                    var buffer = pool.Rent(Encoding.UTF8.GetMaxByteCount(segment.Length));
                    try
                    {
                        var size = Encoding.UTF8.GetBytes(segment, buffer);
                        var piece = buffer.AsSpan(..size);
#else
                        var piece = Encoding.UTF8.GetBytes(segment);
#endif

                        if (piece.Length == 1)
                        {
                            encodedTokens?.Add(Encoder[piece]);
                            count++;
                            continue;
                        }

                        var bytePairEncoder = new MultiBytePairEncoder(piece, Encoder);
                        var tokens = bytePairEncoder.GetTokens();

                        foreach (var token in tokens)
                        {
                            encodedTokens?.Add(token);
                            count++;
                        }
#if NET
                    }
                    finally
                    {
                        pool.Return(buffer);
                    }
#endif
                }

                if (nextSpecialMatch.Success)
                {
                    var specialToken = nextSpecialMatch.Value;
                    var specialTokenValue = SpecialTokensEncoder[specialToken];
                    encodedTokens?.Add(specialTokenValue);
                    count++;
                    startIndex += nextSpecialMatch.Index + specialToken.Length;
                }
                else
                {
                    break;
                }
            }

            return (encodedTokens, count);
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
}

#endif
