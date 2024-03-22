using System;
using System.Collections.Generic;


namespace SharpToken
{
    internal static class Extensions
    {
        public static T[] Slice<T>(this T[] array, int startIndex, int endIndex)
        {
            var len = endIndex - startIndex;
            var slice = new T[len];
            Array.Copy(array, startIndex, slice, 0, len);
            return slice;
        }


        public static FoundMatch FindMatch(this IEnumerable<string> searchValues, string text, int startIndex)
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
    }

    internal struct FoundMatch
    {
        public bool Success { get; set; }
        public string Value { get; set; }
        public int Index { get; set; }
    };
}
