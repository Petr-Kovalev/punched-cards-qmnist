using System;
using System.Collections.Generic;
using System.Linq;

namespace PunchedCards.BitVectors
{
    internal sealed class BitVector : IBitVector
    {
        private readonly uint[] _activeBitIndicesSorted;

        internal BitVector(IEnumerable<uint> activeBitIndices, uint count)
        {
            _activeBitIndicesSorted = activeBitIndices.ToArray();
            Array.Sort(_activeBitIndicesSorted);
            Count = count;
        }

        public uint Count { get; }

        public bool IsActive(uint bitIndex) => Array.BinarySearch(_activeBitIndicesSorted, bitIndex) >= 0;

        public override bool Equals(object obj)
        {
            return obj is BitVector other &&
                   Count.Equals(other.Count) &&
                   _activeBitIndicesSorted.SequenceEqual(other._activeBitIndicesSorted);
        }

        public override int GetHashCode()
        {
            var hashCode = 17;

            unchecked
            {
                hashCode = hashCode * 23 + Count.GetHashCode();

                foreach (var activeBitIndex in _activeBitIndicesSorted)
                {
                    hashCode = hashCode * 23 + activeBitIndex.GetHashCode();
                }
            }

            return hashCode;
        }
    }
}