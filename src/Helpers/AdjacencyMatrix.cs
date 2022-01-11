using System;
using System.Collections.Generic;
using System.Linq;
using PunchedCards.BitVectors;

namespace PunchedCards.Helpers
{
    internal sealed class AdjacencyMatrix : IAdjacencyMatrix
    {
        private readonly uint[,,] _adjacencyMatrix;
        private readonly uint _size;
        private readonly ulong _maxSpanningTree;
        private readonly IEnumerable<Tuple<uint, uint, byte>> _maxSpanningTreeEdges;

        internal AdjacencyMatrix(IEnumerable<IBitVector> bitVectors)
        {
            _adjacencyMatrix = CalculateAdjacencyMatrix(bitVectors, out _size);
            FindMaxSpanningTree(_adjacencyMatrix, out _maxSpanningTreeEdges, out _maxSpanningTree);
        }

        public uint Size => _size;

        public ulong MaxSpanningTree => _maxSpanningTree;

        public ulong CalculateMaxSpanningTreeMatchingScore(IEnumerable<uint> activeBitIndices)
        {
            return CalculateMaxSpanningTreeMatchingScore(_adjacencyMatrix, _maxSpanningTreeEdges, activeBitIndices);
        }

        private static uint[,,] CalculateAdjacencyMatrix(IEnumerable<IBitVector> bitVectors, out uint size)
        {
            uint[,,] adjacencyMatrix = null;
            size = 0;

            foreach (var bitVector in bitVectors)
            {
                if (size == 0)
                {
                    size = bitVector.Count;
                    adjacencyMatrix = new uint[size, size, 4];
                }

                if (bitVector.Count == 0 || bitVector.Count != size)
                {
                    throw new ArgumentException("Invalid Count of bit vector!", nameof(bitVectors));
                }

                var activeBitIndicesHashSet = new HashSet<uint>(bitVector.ActiveBitIndices);
                for (uint i = 0; i < size; i++)
                {
                    var firstVertexValue = activeBitIndicesHashSet.Contains(i);
                    for (uint j = i; j < size; j++)
                    {
                        adjacencyMatrix[i, j, GetEdgeIndexByVertexValues(firstVertexValue, activeBitIndicesHashSet.Contains(j))]++;
                    }
                }
            }

            return adjacencyMatrix;
        }

        private static void FindMaxSpanningTree(uint[,,] adjacencyMatrix, out IEnumerable<Tuple<uint, uint, byte>> maxSpanningTreeEdges, out ulong maxSpanningTree)
        {
            uint size = (uint)adjacencyMatrix.GetLength(0);
            var validVertexValues = new IList<bool>[size];
            for (int i = 0; i < validVertexValues.Length; i++)
            {
                validVertexValues[i] = new List<bool> { true, false };
            }
            var loops = new bool[size];
            var anyEdge = false;
            var maxSpanningTreeEdgesList = new List<Tuple<uint, uint, byte>>();
            while (TryFindNextMaxEdge(adjacencyMatrix, size, validVertexValues, loops, anyEdge, out var maxEdge))
            {
                maxSpanningTreeEdgesList.Add(maxEdge);

                anyEdge = true;
                if (maxEdge.Item1 == maxEdge.Item2)
                {
                    loops[maxEdge.Item1] = true;
                }

                GetVertexValuesByEdgeIndex(maxEdge.Item3, out var firstVertexValue, out var secondVertexValue);
                validVertexValues[maxEdge.Item1].Remove(!firstVertexValue);
                validVertexValues[maxEdge.Item2].Remove(!secondVertexValue);
            }

            maxSpanningTreeEdges = maxSpanningTreeEdgesList;
            maxSpanningTree = CalculateMaxSpanningTree(adjacencyMatrix, maxSpanningTreeEdgesList);
        }

        private static bool TryFindNextMaxEdge(uint[,,] adjacencyMatrix, uint size, IList<bool>[] validVertexValues, bool[] loops, bool anyEdge, out Tuple<uint, uint, byte> maxEdge)
        {
            uint firstVertexIndex = 0;
            uint secondVertexIndex = 0;
            byte edgeIndex = 0;

            bool found = false;
            uint maxEdgeValue = uint.MinValue;
            for (uint i = 0; i < size; i++)
            {
                for (uint j = i; j < size; j++)
                {
                    for (byte e = 0; e < 4; e++)
                    {
                        if (adjacencyMatrix[i, j, e] <= maxEdgeValue || anyEdge && !IsValidEdge(i, j, e, validVertexValues, loops))
                        {
                            continue;
                        }

                        maxEdgeValue = adjacencyMatrix[i, j, e];
                        firstVertexIndex = i;
                        secondVertexIndex = j;
                        edgeIndex = e;
                        found = true;
                    }
                }
            }

            maxEdge = found ? Tuple.Create(firstVertexIndex, secondVertexIndex, edgeIndex) : null;
            return found;
        }

        private static bool IsValidEdge(uint firstVertexIndex, uint secondVertexIndex, byte edgeIndex, IList<bool>[] validVertexValues, bool[] loops)
        {
            var isLoop = firstVertexIndex == secondVertexIndex;
            if (!isLoop)
            {
                var firstVertexConnected = validVertexValues[firstVertexIndex].Count != 2;
                var secondVertexConnected = validVertexValues[secondVertexIndex].Count != 2;
                if (firstVertexConnected && secondVertexConnected || !firstVertexConnected && !secondVertexConnected)
                {
                    return false;
                }
            }
            else
            {
                if (edgeIndex == 1 || edgeIndex == 2 ||
                    loops[firstVertexIndex] ||
                    validVertexValues[firstVertexIndex].Count == 2)
                {
                    return false;
                }
            }

            GetVertexValuesByEdgeIndex(edgeIndex, out var firstVertexValue, out var secondVertexValue);
            return validVertexValues[firstVertexIndex].Contains(firstVertexValue) && validVertexValues[secondVertexIndex].Contains(secondVertexValue);
        }

        private static ulong CalculateMaxSpanningTree(uint[,,] adjacencyMatrix, IEnumerable<Tuple<uint, uint, byte>> maxSpanningTreeEdges)
        {
            ulong maxSpanningTree = 0;
            foreach (var edge in maxSpanningTreeEdges)
            {
                maxSpanningTree += adjacencyMatrix[edge.Item1, edge.Item2, edge.Item3];
            }
            return maxSpanningTree;
        }

        private static ulong CalculateMaxSpanningTreeMatchingScore(uint[,,] adjacencyMatrix, IEnumerable<Tuple<uint, uint, byte>> maxSpanningTreeEdges, IEnumerable<uint> activeBitIndices)
        {
            var activeBitIndicesHashSet = new HashSet<uint>(activeBitIndices);

            ulong matchingScore = 0;
            foreach (var edge in maxSpanningTreeEdges)
            {
                var edgeIndex = GetEdgeIndexByVertexValues(activeBitIndicesHashSet.Contains(edge.Item1), activeBitIndicesHashSet.Contains(edge.Item2));
                if (edgeIndex == edge.Item3)
                {
                    matchingScore += adjacencyMatrix[edge.Item1, edge.Item2, edge.Item3];
                }
            }
            return matchingScore;
        }

        private static void GetVertexValuesByEdgeIndex(byte edgeIndex, out bool firstVertexValue, out bool secondVertexValue)
        {
            firstVertexValue = edgeIndex == 1 || edgeIndex == 3;
            secondVertexValue = edgeIndex == 2 || edgeIndex == 3;
        }

        private static byte GetEdgeIndexByVertexValues(bool firstVertexValue, bool secondVertexValue)
        {
            return (byte)(firstVertexValue ? secondVertexValue ? 3 : 1 : secondVertexValue ? 2 : 0);
        }
    }
}