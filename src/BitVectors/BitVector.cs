using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace PunchedCards.BitVectors
{
    internal sealed class BitVector : IBitVector
    {
        private readonly ImmutableArray<uint> _activeBitIndices;

        internal BitVector(IEnumerable<uint> activeBitIndices, uint count)
        {
            _activeBitIndices = activeBitIndices.Distinct().OrderBy(index => index).ToImmutableArray();
            Count = count;
        }

        public uint Count { get; }

        public bool IsBitActive(uint bitIndex) => _activeBitIndices.Contains(bitIndex);

        public override bool Equals(object obj)
        {
            return obj is BitVector other &&
                   Count.Equals(other.Count) &&
                   _activeBitIndices.SequenceEqual(other._activeBitIndices);
        }

        public override int GetHashCode()
        {
            var hashCode = 17;

            unchecked
            {
                hashCode = hashCode * 23 + Count.GetHashCode();

                foreach (var activeBitIndex in _activeBitIndices)
                {
                    hashCode = hashCode * 23 + activeBitIndex.GetHashCode();
                }
            }

            return hashCode;
        }
    }
}