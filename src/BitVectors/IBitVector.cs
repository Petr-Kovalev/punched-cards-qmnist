using System.Collections.Generic;

namespace PunchedCards.BitVectors
{
    internal interface IBitVector
    {
        uint Count { get; }

        IReadOnlyList<uint> ActiveBitIndices { get; }
    }
}