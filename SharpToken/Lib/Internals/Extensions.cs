using System;
using System.Collections.Generic;

namespace SharpToken
{
    internal static class Extensions
    {
#if NET8_0_OR_GREATER
        public static FoundMatch FindMatch(this IEnumerable<string> searchValues, ReadOnlySpan<char> text)
        {
            foreach (var searchValue in searchValues)
            {
                // uses modern multibyte cpu instructions
                var index = text.IndexOf(searchValue);
                if (index != -1)
                {
                    return new FoundMatch { Success = true, Value = searchValue, Index = index };
                }
            }

            return new FoundMatch { Success = false };
        }
#else
        public static T[] Slice<T>(this T[] array, int startIndex, int endIndex)
        {
            var len = endIndex - startIndex;
            var slice = new T[len];
            Array.Copy(array, startIndex, slice, 0, len);
            return slice;
        }

        public static FoundMatch FindMatch(this IEnumerable<string> searchValues, string text, int startIndex = 0)
        {
            foreach (var searchValue in searchValues)
            {
                var index = text.IndexOf(searchValue, startIndex, StringComparison.Ordinal);
                if (index != -1)
                {
                    return new FoundMatch
                    {
                        Success = true,
                        Value = searchValue,
                        Index = index
                    };
                }
            }

            return new FoundMatch { Success = false };
        }
#endif
    }

    internal ref struct FoundMatch
    {
        public bool Success { get; set; }
        public string Value { get; set; }
        public int Index { get; set; }
    };
}
