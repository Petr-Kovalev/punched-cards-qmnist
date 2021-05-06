using System.Collections.Generic;
using System.Linq;

namespace PunchedCards.BitVectors
{
    internal sealed class BitVector : IBitVector
    {
        internal BitVector(IEnumerable<uint> activeBitIndices, uint count)
        {
            ActiveBitIndices = new List<uint>(activeBitIndices.OrderBy(index => index));
            Count = count;
        }

        public uint Count { get; }

        public IReadOnlyList<uint> ActiveBitIndices { get; }

        public override bool Equals(object obj)
        {
            return obj is BitVector other &&
                   Count.Equals(other.Count) &&
                   AreActiveBitIndicesEqual(ActiveBitIndices, other.ActiveBitIndices);
        }

        private static bool AreActiveBitIndicesEqual(IReadOnlyList<uint> first, IReadOnlyList<uint> second)
        {
            if (!first.Count.Equals(second.Count))
            {
                return false;
            }

            for (var i = 0; i < first.Count; i++)
            {
                if (!first[i].Equals(second[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 17;

            unchecked
            {
                hashCode = hashCode * 23 + Count.GetHashCode();

                foreach (var activeBitIndex in ActiveBitIndices)
                {
                    hashCode = hashCode * 23 + activeBitIndex.GetHashCode();
                }
            }

            return hashCode;
        }
    }
}