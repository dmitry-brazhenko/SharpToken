#if NETSTANDARD
using System.Runtime.CompilerServices;


namespace SharpToken
{
    /// <summary>
    /// This is a simple polyfill to support ReadOnlySpan in net standard.
    /// </summary>
    internal readonly ref struct ReadOnlySpan<T>
    {
        private readonly T[] _array;
        public int Length => _array.Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan(T[] array)
        {
            _array = array;
        }

        public T[] this[int startIndex, int endIndex]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _array.Slice(startIndex, endIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            return _array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T[](ReadOnlySpan<T> span)
        {
            return span._array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ReadOnlySpan<T>(T[] array)
        {
            return new ReadOnlySpan<T>(array);
        }
    }
}
#endif
