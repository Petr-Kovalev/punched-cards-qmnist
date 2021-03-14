using System;
using System.Collections.Generic;
using System.Linq;
using PunchedCards.BitVectors;

namespace PunchedCards.Helpers
{
    internal sealed class AdjacencyMatrix
    {
        internal AdjacencyMatrix(IReadOnlyCollection<Tuple<IBitVector, int>> bitVectors)
        {
            Size = bitVectors.First().Item1.Count;
            Matrix = new int[Size, Size];
            PopulateAdjacencyMatrix(Matrix, bitVectors, out var halfSum);
            HalfSum = halfSum;
        }

        internal int[,] Matrix { get; }

        internal int Size { get; }

        internal long HalfSum { get; }

        private static void PopulateAdjacencyMatrix(
            int[,] matrix,
            IEnumerable<Tuple<IBitVector, int>> bitVectors,
            out long halfSum)
        {
            halfSum = 0;

            foreach (var bitVector in bitVectors)
            {
                var activeBitIndices = bitVector.Item1.ActiveBitIndices;
                var numberOfOccurrences = bitVector.Item2;
                for (var i = 0; i < activeBitIndices.Count; i++)
                {
                    for (var j = i; j < activeBitIndices.Count; j++)
                    {
                        matrix[activeBitIndices[i], activeBitIndices[j]] += numberOfOccurrences;
                        halfSum += numberOfOccurrences;
                    }
                }
            }
        }
    }
}