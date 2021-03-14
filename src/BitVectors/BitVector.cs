using System.Collections.Generic;
using System.Linq;

namespace PunchedCards.BitVectors
{
    internal sealed class BitVector : IBitVector
    {
        internal BitVector(IEnumerable<int> activeBitIndices, int count)
        {
            ActiveBitIndices = new List<int>(activeBitIndices);
            Count = count;
        }

        public int Count { get; }

        public IReadOnlyList<int> ActiveBitIndices { get; }

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