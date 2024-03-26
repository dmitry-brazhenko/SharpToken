using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace SharpToken
{
    internal static class Extensions
    {
#if NET8_0_OR_GREATER
        public static FoundMatch FindMatch(this IEnumerable<string> searchValues, ReadOnlySpan<char> text)
        {
            if (searchValues is string[] { Length: 0 })
            {
                return new FoundMatch { Success = false };
            }

            var minIndex = int.MaxValue;
            string value = null;
            foreach (var searchValue in searchValues)
            {
                // uses modern multibyte cpu instructions
                var index = text.IndexOf(searchValue);
                if (index != -1 && index < minIndex)
                {
                    minIndex = index;
                    value = searchValue;
                }
            }

            return new FoundMatch
            {
                Success = minIndex != int.MaxValue,
                Value = value,
                Index = minIndex
            };
        }
#else
        public static FoundMatch FindMatch(this IEnumerable<string> searchValues, string text, int startIndex = 0)
        {
            var minIndex = int.MaxValue;
            string value = null;
            foreach (var searchValue in searchValues)
            {
                var index = text.IndexOf(searchValue, startIndex, StringComparison.Ordinal);
                if (index != -1 && index < minIndex)
                {
                    minIndex = index;
                    value = searchValue;
                }
            }

            return new FoundMatch
            {
                Success = minIndex != int.MaxValue,
                Value = value,
                Index = minIndex
            };
        }
#endif


#if NETSTANDARD
        /// <summary>
        /// Helper to handle missing API in netstandard.
        /// </summary>
        public static T[] Slice<T>(this T[] array, int startIndex, int endIndex)
        {
            var len = endIndex - startIndex;
            var slice = new T[len];
            Array.Copy(array, startIndex, slice, 0, len);
            return slice;
        }
#endif


        /// <summary>
        /// Helper to handle missing API in netstandard.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<byte> RangeSlice(this ReadOnlySpan<byte> span, int startIndex, int endIndex)
        {
#if NET
            return span[startIndex..endIndex];
#else
            return span[startIndex, endIndex];
#endif
        }
    }


    internal ref struct FoundMatch
    {
        public bool Success { get; set; }
        public string Value { get; set; }
        public int Index { get; set; }
    };
}
