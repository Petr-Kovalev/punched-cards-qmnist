namespace PunchedCards.BitVectors
{
    internal sealed class BitVectorFactory : IBitVectorFactory
    {
        public IBitVector Create(IEnumerable<uint> activeBitIndices, uint count)
        {
            return new BitVector(activeBitIndices, count);
        }
    }
}