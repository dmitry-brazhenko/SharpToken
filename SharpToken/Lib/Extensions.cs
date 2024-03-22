using System;


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
    }
}
