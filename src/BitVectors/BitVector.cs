using System.Collections.Generic;
using System.Linq;

namespace PunchedCards.BitVectors
{
    internal sealed class BitVector : IBitVector
    {
        private readonly uint[] _activeBitIndicesOrdered;

        internal BitVector(IEnumerable<uint> activeBitIndices, uint count)
        {
            _activeBitIndicesOrdered = activeBitIndices.Distinct().OrderBy(index => index).ToArray();
            Count = count;
        }

        public uint Count { get; }

        public bool IsBitActive(uint bitIndex) => OrderedContains(_activeBitIndicesOrdered, bitIndex);

        private static bool OrderedContains(uint[] indices, uint index)
        {
            var left = 0;
            var right = indices.Length - 1;

            while (left <= right)
            {
                var middle = (left + right) / 2;

                var middleValue = indices[middle];
                if (middleValue == index)
                {
                    return true;
                }

                if (middleValue < index)
                {
                    left = middle + 1;
                }
                else
                {
                    right = middle - 1;
                }
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is BitVector other &&
                   Count.Equals(other.Count) &&
                   _activeBitIndicesOrdered.Length.Equals(other._activeBitIndicesOrdered.Length) &&
                   _activeBitIndicesOrdered.SequenceEqual(other._activeBitIndicesOrdered);
        }

        public override int GetHashCode()
        {
            var hashCode = 17;

            unchecked
            {
                hashCode = hashCode * 23 + Count.GetHashCode();

                foreach (var activeBitIndex in _activeBitIndicesOrdered)
                {
                    hashCode = hashCode * 23 + activeBitIndex.GetHashCode();
                }
            }

            return hashCode;
        }
    }
}