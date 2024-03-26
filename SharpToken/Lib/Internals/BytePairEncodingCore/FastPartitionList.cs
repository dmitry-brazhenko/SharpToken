#if NET
using System;
using System.Buffers;
#endif
using System.Numerics;
using System.Runtime.CompilerServices;


namespace SharpToken
{
    internal ref struct FastPartitionList
    {
        public int Length { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; [MethodImpl(MethodImplOptions.AggressiveInlining)] private set; }
        private readonly int[] _index;
        private readonly (int Start, int Rank)[] _partitions;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FastPartitionList(int length)
        {
            Length = length + 1;
#if NET
            _partitions = ArrayPool<(int Start, int Rank)>.Shared.Rent(Length);
            _index = ArrayPool<int>.Shared.Rent(Length);
#else
            _partitions = new (int Start, int Rank)[Length];
            _index = new int[Length];
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public
#if NET
            readonly
#endif
            void Initialize(ReadOnlySpan<byte> piece, BytePairIndex encoder)
        {
            var len = Length;
            for (var i = 0; i < len; i++)
            {
                var endIndex = i + 2;
                var rank = endIndex < len
                    ? encoder.TryGetValue(piece.RangeSlice(i, endIndex), out var r) ? r : int.MaxValue
                    : int.MaxValue;

                _partitions[i] = (Start: i, Rank: rank);
                _index[i] = i;
            }
        }

        public (int Start, int Rank) this[int index]
        {
            set
            {
                var i = _index[index];
                _partitions[i] = value;
            }
            get
            {
                var i = _index[index];
                return _partitions[i];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(int index)
        {
            var len = --Length;

#if NET8_0_OR_GREATER
            var size = Vector<int>.Count;
            {
                for (var i = index; i < len; i++)
                {
                    if (len - i > size)
                    {
                        var startIndex = i + 1;
                        var span = _index.AsSpan(startIndex..(startIndex + size));
                        var vector = new Vector<int>(span);
                        vector.StoreUnsafe(ref _index[i]);
                        i += size - 1;
                    }
                    else
                    {
                        _index[i] = _index[i + 1];
                    }
                }
            }
#else
            for (var i = index; i < len; i++)
            {
                if (len - i >= 8)
                {
                    _index[i] = _index[i + 1];
                    _index[i + 1] = _index[i + 2];
                    _index[i + 2] = _index[i + 3];
                    _index[i + 3] = _index[i + 4];
                    _index[i + 4] = _index[i + 5];
                    _index[i + 5] = _index[i + 6];
                    _index[i + 6] = _index[i + 7];
                    _index[i + 7] = _index[i + 8];
                    i += 7;
                }
                else
                {
                    _index[i] = _index[i + 1];
                }
            }
#endif
        }

#if NET
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Dispose()
        {
            ArrayPool<int>.Shared.Return(_index);
            ArrayPool<(int Start, int Rank)>.Shared.Return(_partitions);
        }
#endif
    }
}
