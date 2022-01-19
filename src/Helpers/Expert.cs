using System;
using System.Collections.Generic;
using System.Linq;
using PunchedCards.BitVectors;

namespace PunchedCards.Helpers
{
    internal sealed class Expert : IExpert
    {
        private static readonly byte FalseFalseEdgeIndex = GetEdgeIndexByVertexValues(false, false);
        private static readonly byte TrueFalseEdgeIndex = GetEdgeIndexByVertexValues(true, false);
        private static readonly byte FalseTrueEdgeIndex = GetEdgeIndexByVertexValues(false, true);
        private static readonly byte TrueTrueEdgeIndex = GetEdgeIndexByVertexValues(true, true);

        private readonly double _maxSpanningTreeWeightDouble;
        private readonly IList<Tuple<uint, uint, byte, int[]>> _maxSpanningTreeEdges;

        internal Expert(IEnumerable<IBitVector> bitVectors)
        {
            _maxSpanningTreeEdges = GetMaxSpanningTreeEdges(CalculateWeightMatrix(bitVectors));
            _maxSpanningTreeWeightDouble = (double)_maxSpanningTreeEdges.Sum(edge => edge.Item4[edge.Item3]);
        }

        public double CalculateLoss(IEnumerable<uint> activeBitIndices)
        {
            var activeBitIndicesHashSet = new HashSet<uint>(activeBitIndices);

            var maxSpanningTreeMatchingWeightsSum = 0;
            foreach (var edge in _maxSpanningTreeEdges)
            {
                var edgeIndex = GetEdgeIndexByVertexValues(activeBitIndicesHashSet.Contains(edge.Item1), activeBitIndicesHashSet.Contains(edge.Item2));
                if (edgeIndex == edge.Item3)
                {
                    maxSpanningTreeMatchingWeightsSum += edge.Item4[edge.Item3];
                }
            }

            return 1 - maxSpanningTreeMatchingWeightsSum / _maxSpanningTreeWeightDouble;
        }

        private static int[,,] CalculateWeightMatrix(IEnumerable<IBitVector> bitVectors)
        {
            int[,,] weightMatrix = null;
            uint size = 0;

            foreach (var bitVector in bitVectors)
            {
                if (size == 0)
                {
                    size = bitVector.Count;
                    weightMatrix = new int[size, size, 4];
                }

                if (bitVector.Count == 0 || bitVector.Count != size)
                {
                    throw new ArgumentException($"Invalid {nameof(bitVector.Count)} of bit vector!", nameof(bitVectors));
                }

                for (uint i = 0; i < size; i++)
                {
                    var firstVertexValue = bitVector.ActiveBitIndices.Contains(i);
                    for (uint j = i; j < size; j++)
                    {
                        weightMatrix[i, j, GetEdgeIndexByVertexValues(firstVertexValue, bitVector.ActiveBitIndices.Contains(j))]++;
                    }
                }
            }

            return weightMatrix;
        }

        private static IList<Tuple<uint, uint, byte, int[]>> GetMaxSpanningTreeEdges(int[,,] weightMatrix)
        {
            var vertexCount = weightMatrix.GetLength(0);
            var connectedVertexIndices = new List<uint>();
            var notConnectedVertexIndices = new List<uint>(Enumerable.Range(0, vertexCount).Select(vertexIndex => (uint)vertexIndex));
            var vertexValues = new bool[vertexCount];
            var vertexLoops = new bool[vertexCount];

            var maxSpanningTreeEdges = new List<Tuple<uint, uint, byte, int[]>>();
            while (TryGetNextMaxValidEdge(weightMatrix,
                connectedVertexIndices.Count != 0 ? GetValidEdges(connectedVertexIndices, notConnectedVertexIndices, vertexValues, vertexLoops) : GetAllValidEdges(vertexCount),
                out var maxValidEdge))
            {
                maxSpanningTreeEdges.Add(maxValidEdge);

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
            return maxSpanningTreeEdges;
        }

        private static bool TryGetNextMaxValidEdge(int[,,] weightMatrix, IEnumerable<Tuple<uint, uint, byte>> validEdges, out Tuple<uint, uint, byte, int[]> maxValidEdge)
        {
            int maxValidEdgeWeight = int.MinValue;
            Tuple<uint, uint, byte> maxValidEdgeCoords = null;

            foreach (var validEdge in validEdges)
            {
                var edgeWeight = weightMatrix[validEdge.Item1, validEdge.Item2, validEdge.Item3];
                if (edgeWeight > maxValidEdgeWeight)
                {
                    maxValidEdgeWeight = edgeWeight;
                    maxValidEdgeCoords = validEdge;
                }
            }

            if (maxValidEdgeCoords == null)
            {
                maxValidEdge = null;
                return false;
            }

            maxValidEdge = Tuple.Create(
                maxValidEdgeCoords.Item1,
                maxValidEdgeCoords.Item2,
                maxValidEdgeCoords.Item3,
                new int[]
                {
                    weightMatrix[maxValidEdgeCoords.Item1, maxValidEdgeCoords.Item2, FalseFalseEdgeIndex],
                    weightMatrix[maxValidEdgeCoords.Item1, maxValidEdgeCoords.Item2, TrueFalseEdgeIndex],
                    weightMatrix[maxValidEdgeCoords.Item1, maxValidEdgeCoords.Item2, FalseTrueEdgeIndex],
                    weightMatrix[maxValidEdgeCoords.Item1, maxValidEdgeCoords.Item2, TrueTrueEdgeIndex]
                });
            return true;
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