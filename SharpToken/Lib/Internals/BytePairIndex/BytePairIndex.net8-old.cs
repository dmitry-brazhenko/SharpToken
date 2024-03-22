using System.Collections.Generic;

#if !NET8_0_OR_GREATER

namespace SharpToken
{
    internal sealed class BytePairIndex : Dictionary<byte[], int>
    {
        public BytePairIndex(IDictionary<byte[], int> data) : base(data, ByteArrayEqualityComparer.Instance)
        {
        }
    }
}

#endif
