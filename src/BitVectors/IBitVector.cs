using System.Collections.Generic;

namespace PunchedCards.BitVectors
{
    internal interface IBitVector
    {
        uint Count { get; }

        bool IsActive(uint bitIndex);

        IEnumerable<uint> ActiveBitIndicesSorted { get; }
    }
}