#if !NET8_0_OR_GREATER
#if NET
using System;
#endif
using System.Collections.Generic;


namespace SharpToken
{
    internal sealed class BytePairIndex : Dictionary<byte[], int>
    {
        public BytePairIndex(IDictionary<byte[], int> data) : base(data, ByteArrayEqualityComparer.Instance)
        {
        }

        public int this[ReadOnlySpan<byte> key] => base[key.ToArray()];

        public bool TryGetValue(ReadOnlySpan<byte> key, out int value)
        {
            return base.TryGetValue(key.ToArray(), out value);
        }
    }
}

#endif
