using System;
using System.Collections.Generic;
using PunchedCards.BitVectors;

namespace PunchedCards.Helpers
{
    internal sealed class AdjacencyMatrix : IAdjacencyMatrix
    {
        private readonly uint[,] _adjacencyMatrix;

        internal AdjacencyMatrix(IEnumerable<IBitVector> bitVectors)
        {
            _adjacencyMatrix = PopulateAdjacencyMatrix(bitVectors, out var size, out var halfSum);
            Size = size;
            HalfSum = halfSum;
        }

        public uint Size { get; }

        public uint this[int i, int j] => _adjacencyMatrix[i, j];

        public ulong HalfSum { get; }

        private static uint[,] PopulateAdjacencyMatrix(IEnumerable<IBitVector> bitVectors, out uint size, out ulong halfSum)
        {
            uint[,] adjacencyMatrix = null;
            size = 0;
            halfSum = 0;

            foreach (var bitVector in bitVectors)
            {
                if (size == 0)
                {
                    size = bitVector.Count;
                    adjacencyMatrix = new uint[size, size];
                }

                if (bitVector.Count == 0 || bitVector.Count != size)
                {
                    throw new ArgumentException(nameof(bitVectors));
                }

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