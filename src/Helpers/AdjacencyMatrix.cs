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
                    var condition1 = activeBitIndicesHashSet.Contains(i);
                    for (uint j = i; j < size; j++)
                    {
                        adjacencyMatrix[i, j, GetEdgeIndex(condition1, activeBitIndicesHashSet.Contains(j))]++;
                    }
                }
            }

            return adjacencyMatrix;
        }

        private static void FindMaxSpanningTree(uint[,,] adjacencyMatrix, out IEnumerable<Tuple<uint, uint, byte>> maxSpanningTreeEdges, out ulong maxSpanningTree)
        {
            var size = adjacencyMatrix.GetLength(0);
            var maxSpanningTreeEdgesList = new List<Tuple<uint, uint, byte>>();

            var solution = new bool?[size];
            bool anySolved = false;

            do
            {
                double max = double.MinValue;
                uint firstIndex = 0;
                uint secondIndex = 0;
                byte edgeIndex = 0;
                for (uint i = 0; i < size; i++)
                {
                    for (uint j = i; j < size; j++)
                    {
                        if (i == j ||
                            solution[i].HasValue && solution[j].HasValue ||
                            !solution[i].HasValue && !solution[j].HasValue && anySolved)
                        {
                            continue;
                        }

                        for (byte e = 0; e < 4; e++)
                        {
                            if (e == 0U &&
                                (solution[i].HasValue && solution[i].Value ||
                                solution[j].HasValue && solution[j].Value))
                            {
                                continue;
                            }
                            else if (e == 1U &&
                                (solution[i].HasValue && !solution[i].Value ||
                                solution[j].HasValue && solution[j].Value))
                            {
                                continue;
                            }
                            else if (e == 2U &&
                                (solution[i].HasValue && solution[i].Value ||
                                solution[j].HasValue && !solution[j].Value))
                            {
                                continue;
                            }
                            else if (e == 3U &&
                                (solution[i].HasValue && !solution[i].Value ||
                                solution[j].HasValue && !solution[j].Value))
                            {
                                continue;
                            }

                            if (adjacencyMatrix[i, j, e] > max)
                            {
                                max = adjacencyMatrix[i, j, e];
                                firstIndex = i;
                                secondIndex = j;
                                edgeIndex = e;
                            }
                        }
                    }
                }

                if (!solution[firstIndex].HasValue)
                {
                    anySolved = true;
                    solution[firstIndex] = edgeIndex switch
                    {
                        1 or 3 => true,
                        _ => false,
                    };
                }

                if (!solution[secondIndex].HasValue)
                {
                    anySolved = true;
                    solution[secondIndex] = edgeIndex switch
                    {
                        2 or 3 => true,
                        _ => false,
                    };
                }

                maxSpanningTreeEdgesList.Add(Tuple.Create(firstIndex, secondIndex, edgeIndex));

            } while (solution.Any(i => !i.HasValue));

            maxSpanningTreeEdges = maxSpanningTreeEdgesList;
            maxSpanningTree = CalculateMaxSpanningTreeMatchingScore(
                adjacencyMatrix,
                maxSpanningTreeEdgesList,
                Enumerable.Range(0, solution.Length).Where(index => solution[index].Value).Select(index => (uint)index));
        }

        private static ulong CalculateMaxSpanningTreeMatchingScore(uint[,,] adjacencyMatrix, IEnumerable<Tuple<uint, uint, byte>> maxSpanningTreeEdges, IEnumerable<uint> activeBitIndices)
        {
            var activeBitIndicesHashSet = new HashSet<uint>(activeBitIndices);

            ulong matchingScore = 0;
            foreach (var edge in maxSpanningTreeEdges)
            {
                var edgeIndex = GetEdgeIndex(activeBitIndicesHashSet.Contains(edge.Item1), activeBitIndicesHashSet.Contains(edge.Item2));
                if (edgeIndex == edge.Item3)
                {
                    matchingScore += adjacencyMatrix[edge.Item1, edge.Item2, edge.Item3];
                }
            }
            return matchingScore;
        }

        private static byte GetEdgeIndex(bool condition1, bool condition2)
        {
            if (condition1)
            {
                if (condition2)
                {
                    return 3;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (condition2)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}