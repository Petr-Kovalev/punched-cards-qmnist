using System.Collections.Generic;
using System.Linq;

namespace PunchedCards.BitVectors
{
    internal sealed class BitVector : IBitVector
    {
        internal BitVector(IEnumerable<uint> activeBitIndices, uint count)
        {
            ActiveBitIndices = new List<uint>(activeBitIndices);
            Count = count;
        }

        public uint Count { get; }

        public IReadOnlyList<uint> ActiveBitIndices { get; }

        public override bool Equals(object obj)
        {
            return obj is BitVector other &&
                   Count.Equals(other.Count) &&
                   ActiveBitIndices.Count.Equals(other.ActiveBitIndices.Count) &&
                   ActiveBitIndices.All(activeBitIndex => other.ActiveBitIndices.Contains(activeBitIndex));
        }

        public override int GetHashCode()
        {
            var hashCode = 17;

            unchecked
            {
                hashCode = hashCode * 23 + Count.GetHashCode();

                foreach (var activeBitIndex in ActiveBitIndices.OrderBy(index => index))
                {
                    hashCode = hashCode * 23 + activeBitIndex.GetHashCode();
                }
            }

            return hashCode;
        }
    }
}