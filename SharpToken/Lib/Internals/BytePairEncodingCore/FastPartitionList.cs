#if NET8_0_OR_GREATER
using System;
using System.Buffers;
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
            _partitions = ArrayPool<(int Start, int Rank)>.Shared.Rent(Length);
            _index = ArrayPool<int>.Shared.Rent(Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Initialize(ReadOnlySpan<byte> piece, BytePairIndex encoder)
        {
            var len = Length;
            for (var i = 0; i < len; i++)
            {
                var endIndex = i + 2;
                var rank = endIndex < len
                    ? encoder.TryGetValue(piece[i..endIndex], out var r) ? r : int.MaxValue
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
            // TODO optimize with multibyte instructions NOTE Array.Copy does not work!
            Length--;
            for (var i = index; i < Length; i++)
            {
                _index[i] = _index[i + 1];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Dispose()
        {
            ArrayPool<int>.Shared.Return(_index);
            ArrayPool<(int Start, int Rank)>.Shared.Return(_partitions);
        }
    }
}

#endif
