using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PunchedCards.BitVectors;

namespace PunchedCards.Helpers
{
    internal sealed class Expert : IExpert
    {
        private static readonly byte FalseFalseEdgeIndex = GetEdgeIndexByVertexValues(false, false);
        private static readonly byte TrueFalseEdgeIndex = GetEdgeIndexByVertexValues(true, false);
        private static readonly byte FalseTrueEdgeIndex = GetEdgeIndexByVertexValues(false, true);
        private static readonly byte TrueTrueEdgeIndex = GetEdgeIndexByVertexValues(true, true);

        private readonly IReadOnlyDictionary<IBitVector, IReadOnlyCollection<Tuple<uint, uint, byte, int>>> _maxSpanningTreeEdges;

        private Expert(IEnumerable<KeyValuePair<IBitVector, IReadOnlyCollection<IBitVector>>> trainingData)
        {
            _maxSpanningTreeEdges = trainingData.ToDictionary(
                trainingItem => trainingItem.Key,
                trainingItem => (IReadOnlyCollection<Tuple<uint, uint, byte, int>>)GetMaxSpanningTreeEdges(CalculateWeightMatrix(trainingItem.Value)).ToList());
        }

        internal static IExpert Create(IEnumerable<KeyValuePair<IBitVector, IReadOnlyCollection<IBitVector>>> trainingData)
        {
            return new Expert(trainingData);
        }

        public double CalculateLoss(IBitVector bitVector, IBitVector label)
        {
            var maxSpanningTreeWeight = 0;
            var maxSpanningTreeWeightLoss = 0;
            foreach (var edge in _maxSpanningTreeEdges[label])
            {
                var edgeIndex = GetEdgeIndexByVertexValues(bitVector.IsActive(edge.Item1), bitVector.IsActive(edge.Item2));
                if (edgeIndex != edge.Item3)
                {
                    maxSpanningTreeWeightLoss += edge.Item4;
                }

                maxSpanningTreeWeight += edge.Item4;
            }

            return (double)maxSpanningTreeWeightLoss / maxSpanningTreeWeight;
        }

        private static int[,,] CalculateWeightMatrix(IReadOnlyCollection<IBitVector> bitVectors)
        {
            uint vertexCount = bitVectors.First().Count;

            var weightMatrix = new int[vertexCount, vertexCount, 4];
            bitVectors
                .AsParallel()
                .ForAll(bitVector =>
                {
                    for (uint firstVertexIndex = 0; firstVertexIndex < vertexCount - 1; firstVertexIndex++)
                    {
                        var firstVertexValue = bitVector.IsActive(firstVertexIndex);
                        for (uint secondVertexIndex = firstVertexIndex + 1; secondVertexIndex < vertexCount; secondVertexIndex++)
                        {
                            Interlocked.Increment(ref weightMatrix[firstVertexIndex, secondVertexIndex, GetEdgeIndexByVertexValues(firstVertexValue, bitVector.IsActive(secondVertexIndex))]);
                        }
                    }
                });

            return weightMatrix;
        }

        private static IEnumerable<Tuple<uint, uint, byte, int>> GetMaxSpanningTreeEdges(int[,,] weightMatrix)
        {
            var vertexCount = weightMatrix.GetLength(0);
            var connectedVertexIndices = new HashSet<uint>();
            var notConnectedVertexIndices = new List<uint>(Enumerable.Range(0, vertexCount).Select(vertexIndex => (uint)vertexIndex));
            var vertexValues = new bool[vertexCount];

            while (TryGetNextMaxValidEdge(weightMatrix,
                connectedVertexIndices.Count != 0 ? GetValidEdges(connectedVertexIndices, notConnectedVertexIndices, vertexValues) : GetAllValidEdges(vertexCount),
                out var maxValidEdge))
            {
                AddEdge(maxValidEdge, connectedVertexIndices, notConnectedVertexIndices, vertexValues);
                yield return maxValidEdge;
            }
        }

        private static void AddEdge(Tuple<uint, uint, byte, int> edge, ISet<uint> connectedVertexIndices, ICollection<uint> notConnectedVertexIndices, bool[] vertexValues)
        {
            vertexValues[edge.Item1] = edge.Item3 == TrueFalseEdgeIndex || edge.Item3 == TrueTrueEdgeIndex;
            vertexValues[edge.Item2] = edge.Item3 == FalseTrueEdgeIndex || edge.Item3 == TrueTrueEdgeIndex;
            connectedVertexIndices.Add(edge.Item1);
            connectedVertexIndices.Add(edge.Item2);
            notConnectedVertexIndices.Remove(edge.Item1);
            notConnectedVertexIndices.Remove(edge.Item2);
        }

        private static bool TryGetNextMaxValidEdge(int[,,] weightMatrix, IEnumerable<Tuple<uint, uint, byte>> validEdges, out Tuple<uint, uint, byte, int> maxValidEdge)
        {
            var maxValidEdgeWeight = int.MinValue;
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

            maxValidEdge = Tuple.Create(maxValidEdgeCoords.Item1, maxValidEdgeCoords.Item2, maxValidEdgeCoords.Item3, maxValidEdgeWeight);
            return true;
        }

        private static IEnumerable<Tuple<uint, uint, byte>> GetValidEdges(IEnumerable<uint> connectedVertexIndices, IReadOnlyCollection<uint> notConnectedVertexIndices, bool[] vertexValues)
        {
            foreach (var connectedVertexIndex in connectedVertexIndices)
            {
                var connectedVertexValue = vertexValues[connectedVertexIndex];
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