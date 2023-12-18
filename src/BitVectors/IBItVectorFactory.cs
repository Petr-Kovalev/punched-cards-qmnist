namespace PunchedCards.BitVectors
{
    internal interface IBitVectorFactory
    {
        IBitVector Create(IEnumerable<uint> activeBitIndices, uint count);
    }
}