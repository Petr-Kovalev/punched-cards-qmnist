using System;
using System.Collections.Generic;
using System.Linq;
using PunchedCards.BitVectors;

namespace PunchedCards.Helpers
{
    internal sealed class AdjacencyMatrix : IAdjacencyMatrix
    {
        private static readonly byte FalseFalseEdgeIndex = GetEdgeIndexByVertexValues(false, false);
        private static readonly byte FalseTrueEdgeIndex = GetEdgeIndexByVertexValues(false, true);
        private static readonly byte TrueFalseEdgeIndex = GetEdgeIndexByVertexValues(true, false);
        private static readonly byte TrueTrueEdgeIndex = GetEdgeIndexByVertexValues(true, true);

        private readonly uint[,,] _adjacencyMatrix;
        private readonly uint _size;
        private readonly ulong _maxSpanningTree;
        private readonly IList<Tuple<uint, uint, byte>> _maxSpanningTreeEdges;

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
                    throw new ArgumentException($"Invalid {nameof(bitVector.Count)} of bit vector!", nameof(bitVectors));
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

        private static void FindMaxSpanningTree(uint[,,] adjacencyMatrix, out IList<Tuple<uint, uint, byte>> maxSpanningTreeEdges, out ulong maxSpanningTree)
        {
            var vertexCount = adjacencyMatrix.GetLength(0);
            var connectedVertexIndices = new HashSet<uint>();
            var notConnectedVertexIndices = new HashSet<uint>(Enumerable.Range(0, vertexCount).Select(vertexIndex => (uint)vertexIndex));
            var vertexValues = new bool[vertexCount];
            var vertexLoops = new bool[vertexCount];

            maxSpanningTreeEdges = new List<Tuple<uint, uint, byte>>();
            maxSpanningTree = 0;
            while (TryGetNextMaxValidEdge(adjacencyMatrix,
                connectedVertexIndices.Count != 0 ? GetValidEdges(connectedVertexIndices, notConnectedVertexIndices, vertexValues, vertexLoops) : GetAllValidEdges(vertexCount),
                out var maxValidEdge))
            {
                maxSpanningTreeEdges.Add(maxValidEdge);
                maxSpanningTree += adjacencyMatrix[maxValidEdge.Item1, maxValidEdge.Item2, maxValidEdge.Item3];

                vertexValues[maxValidEdge.Item1] = GetFirstVertexValueByEdgeIndex(maxValidEdge.Item3);
                connectedVertexIndices.Add(maxValidEdge.Item1);
                notConnectedVertexIndices.Remove(maxValidEdge.Item1);

                if (maxValidEdge.Item1 == maxValidEdge.Item2)
                {
                    vertexLoops[maxValidEdge.Item1] = true;
                }
                else
                {
                    vertexValues[maxValidEdge.Item2] = GetSecondVertexValueByEdgeIndex(maxValidEdge.Item3);
                    connectedVertexIndices.Add(maxValidEdge.Item2);
                    notConnectedVertexIndices.Remove(maxValidEdge.Item2);
                }
            }
        }

        private static bool TryGetNextMaxValidEdge(uint[,,] adjacencyMatrix, IEnumerable<Tuple<uint, uint, byte>> validEdges, out Tuple<uint, uint, byte> maxValidEdge)
        {
            uint maxValidEdgeWeight = uint.MinValue;
            maxValidEdge = null;

            foreach (var validEdge in validEdges)
            {
                var edgeWeight = adjacencyMatrix[validEdge.Item1, validEdge.Item2, validEdge.Item3];
                if (edgeWeight > maxValidEdgeWeight)
                {
                    maxValidEdgeWeight = edgeWeight;
                    maxValidEdge = validEdge;
                }
            }

            return maxValidEdge != null;
        }

        private static IEnumerable<Tuple<uint, uint, byte>> GetValidEdges(IEnumerable<uint> connectedVertexIndices, IEnumerable<uint> notConnectedVertexIndices, bool[] vertexValues, bool[] vertexLoops)
        {
            foreach (var connectedVertexIndex in connectedVertexIndices)
            {
                var connectedVertexValue = vertexValues[connectedVertexIndex];

                if (!vertexLoops[connectedVertexIndex])
                {
                    yield return Tuple.Create(connectedVertexIndex, connectedVertexIndex, connectedVertexValue ? TrueTrueEdgeIndex : FalseFalseEdgeIndex);
                }

                foreach (var notConnectedVertexIndex in notConnectedVertexIndices)
                {
                    if (connectedVertexIndex < notConnectedVertexIndex)
                    {
                        yield return Tuple.Create(connectedVertexIndex, notConnectedVertexIndex, connectedVertexValue ? TrueFalseEdgeIndex : FalseFalseEdgeIndex);
                        yield return Tuple.Create(connectedVertexIndex, notConnectedVertexIndex, connectedVertexValue ? TrueTrueEdgeIndex : FalseTrueEdgeIndex);
                    }
                    else
                    {
                        yield return Tuple.Create(notConnectedVertexIndex, connectedVertexIndex, connectedVertexValue ? FalseTrueEdgeIndex : FalseFalseEdgeIndex);
                        yield return Tuple.Create(notConnectedVertexIndex, connectedVertexIndex, connectedVertexValue ? TrueTrueEdgeIndex : TrueFalseEdgeIndex);
                    }
                }
            }
        }

        private static IEnumerable<Tuple<uint, uint, byte>> GetAllValidEdges(int vertexCount)
        {
            for (uint cycleVertexIndex = 0; cycleVertexIndex < vertexCount; cycleVertexIndex++)
            {
                yield return Tuple.Create(cycleVertexIndex, cycleVertexIndex, FalseFalseEdgeIndex);
                yield return Tuple.Create(cycleVertexIndex, cycleVertexIndex, TrueTrueEdgeIndex);
            }

            for (uint firstVertexIndex = 0; firstVertexIndex < vertexCount - 1; firstVertexIndex++)
            {
                for (uint secondVertexIndex = firstVertexIndex + 1; secondVertexIndex < vertexCount; secondVertexIndex++)
                {
                    yield return Tuple.Create(firstVertexIndex, secondVertexIndex, FalseFalseEdgeIndex);
                    yield return Tuple.Create(firstVertexIndex, secondVertexIndex, TrueFalseEdgeIndex);
                    yield return Tuple.Create(firstVertexIndex, secondVertexIndex, FalseTrueEdgeIndex);
                    yield return Tuple.Create(firstVertexIndex, secondVertexIndex, TrueTrueEdgeIndex);
                }
            }
        }

        private static bool GetFirstVertexValueByEdgeIndex(byte edgeIndex) => edgeIndex == TrueFalseEdgeIndex || edgeIndex == TrueTrueEdgeIndex;

        private static bool GetSecondVertexValueByEdgeIndex(byte edgeIndex) => edgeIndex == FalseTrueEdgeIndex || edgeIndex == TrueTrueEdgeIndex;

        private static byte GetEdgeIndexByVertexValues(bool firstVertexValue, bool secondVertexValue) =>
            firstVertexValue switch
            {
                false => secondVertexValue switch
                {
                    false => 0,
                    true => 2
                },
                true => secondVertexValue switch
                {
                    false => 1,
                    true => 3
                }
            };
    }
}