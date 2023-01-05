using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace PunchedCards.BitVectors
{
    [DataContract]
    internal sealed class BitVector : IBitVector
    {
        private const int NumberOfValuesThreshold = 32;

        [DataMember(Name = "Count")]
        private readonly uint _count;

        [DataMember(Name = "ActiveBitIndicesSorted")]
        private readonly uint[] _activeBitIndicesSorted;

        private int _hashCode;

        private BitVector()
        {
        }

        internal BitVector(IEnumerable<uint> activeBitIndices, uint count)
        {
            _activeBitIndicesSorted = activeBitIndices.Distinct().ToArray();
            Array.Sort(_activeBitIndicesSorted);
            _count = count;
        }

        public uint Count => _count;

        public bool IsActive(uint bitIndex) => (_activeBitIndicesSorted.Length <= NumberOfValuesThreshold ?
            Array.IndexOf(_activeBitIndicesSorted, bitIndex) :
            Array.BinarySearch(_activeBitIndicesSorted, bitIndex)) >= 0;

        public override bool Equals(object obj)
        {
            return obj is BitVector other &&
                   Count.Equals(other.Count) &&
                   _activeBitIndicesSorted.SequenceEqual(other._activeBitIndicesSorted);
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