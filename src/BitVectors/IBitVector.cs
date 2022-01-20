using System.Collections.Generic;

namespace PunchedCards.BitVectors
{
    internal interface IBitVector
    {
        uint Count { get; }

        IReadOnlyCollection<uint> ActiveBitIndices { get; }
    }
}