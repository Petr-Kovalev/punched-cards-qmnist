using System;
using System.Collections.Generic;
using System.Linq;
using PunchedCards.BitVectors;

namespace PunchedCards.Helpers
{
    internal sealed class Expert : IExpert
    {
        private const int MaxSpanningTreesPerLabel = 1;

        private static readonly byte FalseFalseEdgeIndex = GetEdgeIndexByVertexValues(false, false);
        private static readonly byte TrueFalseEdgeIndex = GetEdgeIndexByVertexValues(true, false);
        private static readonly byte FalseTrueEdgeIndex = GetEdgeIndexByVertexValues(false, true);
        private static readonly byte TrueTrueEdgeIndex = GetEdgeIndexByVertexValues(true, true);

        private readonly IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IReadOnlyCollection<ValueTuple<uint, uint, byte, int>>>> _maxSpanningTreesEdges;
        private readonly IReadOnlyDictionary<IBitVector, int> _maxSpanningTreesWeightSums;
        private readonly IList<IBitVector> _labels;

        private Expert(IEnumerable<KeyValuePair<IBitVector, IReadOnlyCollection<IBitVector>>> trainingData)
        {
            _maxSpanningTreesEdges = trainingData.ToDictionary(
                trainingItem => trainingItem.Key,
                trainingItem => (IReadOnlyCollection<IReadOnlyCollection<ValueTuple<uint, uint, byte, int>>>)GetMaxSpanningTreesEdges(trainingItem.Value, MaxSpanningTreesPerLabel).ToList());
            _maxSpanningTreesWeightSums = _maxSpanningTreesEdges.ToDictionary(p => p.Key, p => p.Value.SelectMany(edge => edge).Sum(edge => edge.Item4));
            _labels = _maxSpanningTreesEdges.Keys.ToList();
        }

        internal static IExpert Create(IEnumerable<KeyValuePair<IBitVector, IReadOnlyCollection<IBitVector>>> trainingData)
        {
            return new Expert(trainingData);
        }

        public IReadOnlyDictionary<IBitVector, double> CalculateMatchingScores(IBitVector bitVector)
        {
            var bitActivityBoolArray = new bool[bitVector.Count];
            for (uint bitIndex = 0; bitIndex < bitActivityBoolArray.Length; bitIndex++)
            {
                bitActivityBoolArray[bitIndex] = bitVector.IsActive(bitIndex);
            }

            var matchingScores = new double[_labels.Count];

            for (var labelIndex = 0; labelIndex < _labels.Count; labelIndex++)
            {
                var label = _labels[labelIndex];
                var maxSpanningTreeWeight = 0;
                foreach (var edge in _maxSpanningTreesEdges[label].SelectMany(edge => edge))
                {
                    var edgeIndex = GetEdgeIndexByVertexValues(bitActivityBoolArray[edge.Item1], bitActivityBoolArray[edge.Item2]);
                    if (edgeIndex == edge.Item3)
                    {
                        maxSpanningTreeWeight += edge.Item4;
                    }
                }

                matchingScores[labelIndex] = (double)maxSpanningTreeWeight / _maxSpanningTreesWeightSums[label];
            }

            SoftMax(matchingScores);

            return Enumerable.Range(0, _labels.Count).ToDictionary(
                labelIndex => _labels[labelIndex],
                labelIndex => matchingScores[labelIndex]);
        }

        private static IEnumerable<IReadOnlyCollection<ValueTuple<uint, uint, byte, int>>> GetMaxSpanningTreesEdges(IReadOnlyCollection<IBitVector> bitVectors, int maxSpanningTreesCount)
        {
            var weightMatrix = CalculateWeightMatrix(bitVectors);

            int maxSpanningTreesCounter = 0;
            while (true)
            {
                var maxSpanningTreeEdges = GetMaxSpanningTreeEdges(weightMatrix).ToList();
                yield return maxSpanningTreeEdges;

                maxSpanningTreesCounter++;
                if (maxSpanningTreesCounter >= maxSpanningTreesCount)
                {
                    yield break;
                }

                foreach (var maxSpanningTreeEdge in maxSpanningTreeEdges)
                {
                    weightMatrix[maxSpanningTreeEdge.Item1, maxSpanningTreeEdge.Item2, maxSpanningTreeEdge.Item3] = int.MinValue;
                }
            }
        }

        private static int[,,] CalculateWeightMatrix(IReadOnlyCollection<IBitVector> bitVectors)
        {
            var vertexCount = bitVectors.First().Count;
            var bitActivityBoolArray = new bool[vertexCount];

            var weightMatrix = new int[vertexCount, vertexCount, 4];
            foreach (var bitVector in bitVectors)
            {
                for (uint bitIndex = 0; bitIndex < bitActivityBoolArray.Length; bitIndex++)
                {
                    bitActivityBoolArray[bitIndex] = bitVector.IsActive(bitIndex);
                }

                for (uint firstVertexIndex = 0; firstVertexIndex < vertexCount - 1; firstVertexIndex++)
                {
                    for (uint secondVertexIndex = firstVertexIndex + 1; secondVertexIndex < vertexCount; secondVertexIndex++)
                    {
                        weightMatrix[firstVertexIndex, secondVertexIndex, GetEdgeIndexByVertexValues(bitActivityBoolArray[firstVertexIndex], bitActivityBoolArray[secondVertexIndex])]++;
                    }
                }
            }

            return weightMatrix;
        }

        private static IEnumerable<ValueTuple<uint, uint, byte, int>> GetMaxSpanningTreeEdges(int[,,] weightMatrix)
        {
            var vertexCount = weightMatrix.GetLength(0);
            var connectedVertexIndices = new HashSet<uint>();
            var notConnectedVertexIndices = new List<uint>(Enumerable.Range(0, vertexCount).Select(vertexIndex => (uint)vertexIndex));
            var vertexValues = new bool[vertexCount];

            while (TryGetNextMaxValidEdge(weightMatrix,
                connectedVertexIndices.Count != 0 ? GetValidEdges(connectedVertexIndices, notConnectedVertexIndices, vertexValues) : GetAllValidEdges(vertexCount),
                out var maxValidEdge))
            {
                AppendEdge(maxValidEdge, connectedVertexIndices, notConnectedVertexIndices, vertexValues);
                yield return maxValidEdge;
            }
        }

        private static void AppendEdge(ValueTuple<uint, uint, byte, int> edge, ISet<uint> connectedVertexIndices, ICollection<uint> notConnectedVertexIndices, bool[] vertexValues)
        {
            vertexValues[edge.Item1] = edge.Item3 == TrueFalseEdgeIndex || edge.Item3 == TrueTrueEdgeIndex;
            vertexValues[edge.Item2] = edge.Item3 == FalseTrueEdgeIndex || edge.Item3 == TrueTrueEdgeIndex;
            connectedVertexIndices.Add(edge.Item1);
            connectedVertexIndices.Add(edge.Item2);
            notConnectedVertexIndices.Remove(edge.Item1);
            notConnectedVertexIndices.Remove(edge.Item2);
        }

        private static bool TryGetNextMaxValidEdge(int[,,] weightMatrix, IEnumerable<ValueTuple<uint, uint, byte>> validEdges, out ValueTuple<uint, uint, byte, int> maxValidEdge)
        {
            var maxValidEdgeWeight = int.MinValue;
            ValueTuple<uint, uint, byte> maxValidEdgeCoords = default;
            bool nextMaxValidEdgeFound = false;

            foreach (var validEdge in validEdges)
            {
                var edgeWeight = weightMatrix[validEdge.Item1, validEdge.Item2, validEdge.Item3];
                if (edgeWeight > maxValidEdgeWeight)
                {
                    maxValidEdgeWeight = edgeWeight;
                    maxValidEdgeCoords = validEdge;
                    nextMaxValidEdgeFound = true;
                }
            }

            if (!nextMaxValidEdgeFound)
            {
                maxValidEdge = default;
                return false;
            }

            maxValidEdge = ValueTuple.Create(maxValidEdgeCoords.Item1, maxValidEdgeCoords.Item2, maxValidEdgeCoords.Item3, maxValidEdgeWeight);
            return true;
        }

        private static IEnumerable<ValueTuple<uint, uint, byte>> GetValidEdges(IEnumerable<uint> connectedVertexIndices, IReadOnlyCollection<uint> notConnectedVertexIndices, bool[] vertexValues)
        {
            foreach (var connectedVertexIndex in connectedVertexIndices)
            {
                var connectedVertexValue = vertexValues[connectedVertexIndex];
                foreach (var notConnectedVertexIndex in notConnectedVertexIndices)
                {
                    var noSwap = connectedVertexIndex < notConnectedVertexIndex;
                    var firstVertexIndex = noSwap ? connectedVertexIndex : notConnectedVertexIndex;
                    var secondVertexIndex = noSwap ? notConnectedVertexIndex : connectedVertexIndex;

                    yield return ValueTuple.Create(firstVertexIndex, secondVertexIndex, GetEdgeIndexByVertexValues(!noSwap || connectedVertexValue, noSwap || connectedVertexValue));
                    yield return ValueTuple.Create(firstVertexIndex, secondVertexIndex, GetEdgeIndexByVertexValues(noSwap && connectedVertexValue, !noSwap && connectedVertexValue));
                }
            }
        }

        private static IEnumerable<ValueTuple<uint, uint, byte>> GetAllValidEdges(int vertexCount)
        {
            for (uint firstVertexIndex = 0; firstVertexIndex < vertexCount - 1; firstVertexIndex++)
            {
                for (uint secondVertexIndex = firstVertexIndex + 1; secondVertexIndex < vertexCount; secondVertexIndex++)
                {
                    yield return ValueTuple.Create(firstVertexIndex, secondVertexIndex, FalseFalseEdgeIndex);
                    yield return ValueTuple.Create(firstVertexIndex, secondVertexIndex, TrueFalseEdgeIndex);
                    yield return ValueTuple.Create(firstVertexIndex, secondVertexIndex, FalseTrueEdgeIndex);
                    yield return ValueTuple.Create(firstVertexIndex, secondVertexIndex, TrueTrueEdgeIndex);
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

        private static void SoftMax(IList<double> values)
        {
            double divisor = 0;
            for (var i = 0; i < values.Count; i++)
            {
                var exp = Math.Exp(values[i]);
                values[i] = exp;
                divisor += exp;
            }

            for (var i = 0; i < values.Count; i++)
            {
                values[i] /= divisor;
            }
        }
    }
}