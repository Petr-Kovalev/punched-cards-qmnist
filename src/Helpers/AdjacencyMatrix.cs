using System.Collections.Generic;
using PunchedCards.BitVectors;

namespace PunchedCards.Helpers
{
    internal sealed class AdjacencyMatrix
    {
        internal AdjacencyMatrix(IEnumerable<IBitVector> bitVectors)
        {
            Matrix = PopulateAdjacencyMatrix(bitVectors, out var halfSum);
            HalfSum = halfSum;
        }

        internal int[,] Matrix { get; }

        internal int Size => Matrix.GetLength(0);

        internal long HalfSum { get; }

        private static int[,] PopulateAdjacencyMatrix(IEnumerable<IBitVector> bitVectors, out long halfSum)
        {
            int[,] adjacencyMatrix = null;
            halfSum = 0;

            foreach (var bitVector in bitVectors)
            {
                adjacencyMatrix ??= new int[bitVector.Count, bitVector.Count];

                var activeBitIndices = bitVector.ActiveBitIndices;
                for (var i = 0; i < activeBitIndices.Count; i++)
                {
                    for (var j = i; j < activeBitIndices.Count; j++)
                    {
                        adjacencyMatrix[activeBitIndices[i], activeBitIndices[j]]++;
                        halfSum++;
                    }
                }
            }

            return adjacencyMatrix;
        }
    }
}