using System;
using System.Collections.Generic;
using System.Linq;
using PunchedCards.BitVectors;

namespace PunchedCards.Helpers
{
    internal sealed class AdjacencyMatrix : IAdjacencyMatrix
    {
        private readonly uint[,] _adjacencyMatrix;

        internal AdjacencyMatrix(IEnumerable<IBitVector> bitVectors)
        {
            _adjacencyMatrix = CalculateAdjacencyMatrix(bitVectors, out var size, out var halfSum);
            Size = size;
            HalfSum = halfSum;
        }

        public uint Size { get; }

        public uint this[int i, int j] => _adjacencyMatrix[i, j];

        public ulong HalfSum { get; }

        public ulong CalculateActiveBitConnectionsHalfSum(IEnumerable<uint> activeBitIndices)
        {
            return CalculateActiveBitConnectionsHalfSum(this, activeBitIndices.Distinct().OrderBy(index => index));
        }

        private static uint[,] CalculateAdjacencyMatrix(IEnumerable<IBitVector> bitVectors, out uint size, out ulong halfSum)
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
                    throw new ArgumentException("Invalid Count of bit vector!", nameof(bitVectors));
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

        private static ulong CalculateActiveBitConnectionsHalfSum(IAdjacencyMatrix adjacencyMatrix, IEnumerable<uint> activeBitIndicesOrdered)
        {
            ulong activeBitConnectionsHalfSum = 0;

            var usedIndices = new List<uint>();
            foreach (var activeBitIndex in activeBitIndicesOrdered)
            {
                for (var i = 0U; i < activeBitIndex; i++)
                {
                    activeBitConnectionsHalfSum += adjacencyMatrix[(int)i, (int)activeBitIndex];
                }

                foreach (var i in usedIndices)
                {
                    activeBitConnectionsHalfSum -= adjacencyMatrix[(int)i, (int)activeBitIndex];
                }

                usedIndices.Add(activeBitIndex);

                for (var j = activeBitIndex; j < adjacencyMatrix.Size; j++)
                {
                    activeBitConnectionsHalfSum += adjacencyMatrix[(int)activeBitIndex, (int)j];
                }
            }

            return activeBitConnectionsHalfSum;
        }
    }
}