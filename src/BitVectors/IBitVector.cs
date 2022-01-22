namespace PunchedCards.BitVectors
{
    internal interface IBitVector
    {
        uint Count { get; }

        bool IsBitActive(uint bitIndex);
    }
}