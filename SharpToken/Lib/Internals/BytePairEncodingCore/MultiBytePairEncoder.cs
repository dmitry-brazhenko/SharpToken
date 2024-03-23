#if NET8_0_OR_GREATER
using System;
using System.Runtime.CompilerServices;
using SharpToken;


namespace SharpToken
{
    internal readonly ref struct MultiBytePairEncoder
    {
        private readonly ReadOnlySpan<byte> _piece;
        private readonly BytePairIndex _encoder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MultiBytePairEncoder(ReadOnlySpan<byte> piece, BytePairIndex encoder)
        {
            _piece = piece;
            _encoder = encoder;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TokenEnumerator GetTokens()
        {
            var partitions = new FastPartitionList(_piece.Length);
            partitions.Initialize(_piece, _encoder);

            while (partitions.Length > 1)
            {
                var minRank = int.MaxValue;
                var minRankIdx = 0;

                for (var i = 0; i < partitions.Length - 1; i++)
                {
                    if (partitions[i].Rank < minRank)
                    {
                        minRank = partitions[i].Rank;
                        minRankIdx = i;
                    }
                }

                if (minRank != int.MaxValue)
                {
                    partitions[minRankIdx] = (partitions[minRankIdx].Start, GetRank(partitions, minRankIdx, 1) ?? int.MaxValue);

                    if (minRankIdx > 0)
                    {
                        partitions[minRankIdx - 1] = (partitions[minRankIdx - 1].Start, GetRank(partitions, minRankIdx - 1, 1) ?? int.MaxValue);
                    }

                    partitions.RemoveAt(minRankIdx + 1);
                }
                else
                {
                    break;
                }
            }

            return new TokenEnumerator(partitions, _piece, _encoder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int? GetRank(FastPartitionList partitions, int startIndex, int skip)
        {
            var endIndex = startIndex + skip + 2;
            if (endIndex >= partitions.Length)
            {
                return null;
            }

            var key = _piece[partitions[startIndex].Start..partitions[endIndex].Start];
            return _encoder.TryGetValue(key, out var rank) ? rank : null;
        }
    }
}

internal ref struct TokenEnumerator
{
    private readonly int _length;
    private readonly FastPartitionList _partitions;
    private readonly ReadOnlySpan<byte> _piece;
    private readonly BytePairIndex _encoder;
    private int _index;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TokenEnumerator(FastPartitionList partitions, ReadOnlySpan<byte> piece, BytePairIndex encoder)
    {
        _length = partitions.Length - 1;
        _partitions = partitions;
        _piece = piece;
        _encoder = encoder;
        _index = -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly TokenEnumerator GetEnumerator() => this;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        var index = _index + 1;
        if (index < _length)
        {
            _index = index;
            return true;
        }

        return false;
    }

    public int Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var key = _piece[_partitions[_index].Start.._partitions[_index + 1].Start];
            var token = _encoder[key];
            return token;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Dispose()
    {
        _partitions.Dispose();
    }
}

#endif
