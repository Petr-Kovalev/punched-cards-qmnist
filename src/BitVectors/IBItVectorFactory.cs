using System.Collections.Generic;

namespace PunchedCards.BitVectors
{
    internal interface IBitVectorFactory
    {
        IBitVector Create(IEnumerable<uint> activeBitIndices, uint count);
    }
}