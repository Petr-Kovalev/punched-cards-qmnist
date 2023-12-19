namespace PunchedCards.BitVectors
{
    internal sealed class BitVector : IBitVector
    {
        private const int NumberOfValuesThreshold = 32;

        private readonly uint[] _activeBitIndicesSorted;

        private int _hashCode;

        public BitVector()
        {
        }

        internal BitVector(IEnumerable<uint> activeBitIndices, uint count)
        {
            ActiveBitIndicesSorted = activeBitIndices;
            Count = count;
        }

        public uint Count { get; init; }

        public IEnumerable<uint> ActiveBitIndicesSorted
        {
            get => _activeBitIndicesSorted;

            init
            {
                _activeBitIndicesSorted = value.Distinct().ToArray();
                Array.Sort(_activeBitIndicesSorted);
            }
        }

        public bool IsActive(uint bitIndex) => (_activeBitIndicesSorted.Length <= NumberOfValuesThreshold ?
            Array.IndexOf(_activeBitIndicesSorted, bitIndex) :
            Array.BinarySearch(_activeBitIndicesSorted, bitIndex)) >= 0;

        public override bool Equals(object obj)
        {
            return Equals(obj as IBitVector);
        }

        public override int GetHashCode()
        {
            if (_hashCode == 0)
            {
                var hashCode = new HashCode();
                foreach (var activeBitIndex in _activeBitIndicesSorted)
                {
                    hashCode.Add(activeBitIndex);
                }
                _hashCode = hashCode.ToHashCode();
            }

            return _hashCode;
        }
    }
}