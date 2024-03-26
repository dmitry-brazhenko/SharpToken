#if !NET8_0_OR_GREATER
#if NET
using System;
#endif
using System.Collections.Generic;
using System.Linq;


namespace SharpToken
{
    internal sealed class BytePairIndex : Dictionary<byte[], int>
    {
        public int MaxKeyLength { get; }

        public BytePairIndex(IDictionary<byte[], int> data) : base(data, ByteArrayEqualityComparer.Instance)
        {
            MaxKeyLength = data.Max(_ => _.Key.Length);
        }

        public int this[ReadOnlySpan<byte> key] => base[key.ToArray()];

        public bool TryGetValue(ReadOnlySpan<byte> key, out int value)
        {
            return base.TryGetValue(key.ToArray(), out value);
        }
    }
}

#endif
