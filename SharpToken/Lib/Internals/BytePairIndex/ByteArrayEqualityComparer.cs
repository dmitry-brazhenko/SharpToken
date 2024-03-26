#if !NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Numerics;


namespace SharpToken
{
    internal sealed class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
    {
        public static ByteArrayEqualityComparer Instance = new ByteArrayEqualityComparer();

        public bool Equals(byte[] x, byte[] y)
        {
            // If both arrays are the same instance or both are null,
            // they are considered equal.
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            // If one of the arrays is null, they are not equal.
            if (x == null || y == null)
            {
                return false;
            }

            // If the lengths of the arrays are different, they are not equal.
            var length = x.Length;
            if (length != y.Length)
            {
                return false;
            }

            // Compare the elements of the arrays for equality.
            var i = 0;
#if NET
            // use multibyte instructions
            var count = Vector<byte>.Count;
            while (i + count < length)
            {
                var left = new Vector<byte>(x.AsSpan()[i..(i + count)]);
                var right = new Vector<byte>(y.AsSpan()[i..(i + count)]);

                if (!Vector.EqualsAll(left, right))
                {
                    return false;
                }

                i += count;
            }
#endif
            for (; i < length; i++)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(byte[] bytes)
        {
            var hash = 17;
            for (var i = 0; i < bytes.Length; i++)
            {
                hash = hash * 31 + bytes[i];
            }

            return hash;
        }
    }
}

#endif
