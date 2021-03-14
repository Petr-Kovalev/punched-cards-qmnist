using System.Collections.Generic;

namespace PunchedCards.BitVectors
{
    internal interface IBitVector
    {
        int Count { get; }

        IReadOnlyList<int> ActiveBitIndices { get; }
    }
}