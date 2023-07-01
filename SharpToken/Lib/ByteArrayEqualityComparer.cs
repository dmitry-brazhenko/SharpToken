using System;
using System.Collections.Generic;

namespace SharpToken
{
    public class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
    {
        // Constants used in the GetHashCode method.
        private const int SampleSizeDivider = 16;
        private const int InitialHashValue = 17;
        private const int HashMultiplier = 31;

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
            for (var i = 0; i < length; i++)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(byte[] obj)
        {
            // Initialize the hash code.
            var hash = InitialHashValue;

            // Use a sampling strategy for very large byte arrays to save processing time.
            var step = Math.Max(1, obj.Length / SampleSizeDivider);

            // Compute the hash code from the sampled elements of the byte array.
            for (var i = 0; i < obj.Length; i += step)
            {
                hash = hash * HashMultiplier + obj[i];
            }

            return hash;
        }
    }
}
