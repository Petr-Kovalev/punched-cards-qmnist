using System.Collections.Generic;

namespace PunchedCards.BitVectors
{
    internal sealed class BitVectorFactory : IBitVectorFactory
    {
        public IBitVector Create(IEnumerable<int> activeBitIndices, int count)
        {
            return new BitVector(activeBitIndices, count);
        }
    }
}