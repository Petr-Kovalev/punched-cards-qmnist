namespace PunchedCards.BitVectors
{
    internal interface IBitVector : IEquatable<IBitVector>
    {
        uint Count { get; }

        bool IsActive(uint bitIndex);

        IEnumerable<uint> ActiveBitIndicesSorted { get; }

        bool IEquatable<IBitVector>.Equals(IBitVector other)
        {
            return other != null &&
                   Count.Equals(other.Count) &&
                   ActiveBitIndicesSorted.SequenceEqual(other.ActiveBitIndicesSorted);
        }
    }
}