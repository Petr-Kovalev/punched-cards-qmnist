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

        private readonly IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IReadOnlyCollection<Tuple<uint, uint, byte, int>>>> _maxSpanningTreesEdges;
        private readonly IReadOnlyDictionary<IBitVector, int> _maxSpanningTreesWeightSums;
        private readonly IList<IBitVector> _labels;

        private Expert(IEnumerable<KeyValuePair<IBitVector, IReadOnlyCollection<IBitVector>>> trainingData)
        {
            _maxSpanningTreesEdges = trainingData.ToDictionary(
                trainingItem => trainingItem.Key,
                trainingItem => (IReadOnlyCollection<IReadOnlyCollection<Tuple<uint, uint, byte, int>>>)GetMaxSpanningTreesEdges(trainingItem.Value, MaxSpanningTreesPerLabel).ToList());
            _maxSpanningTreesWeightSums = _maxSpanningTreesEdges.ToDictionary(p => p.Key, p => p.Value.SelectMany(edge => edge).Sum(edge => edge.Item4));
            _labels = _maxSpanningTreesEdges.Keys.ToList();
        }

        internal static IExpert Create(IEnumerable<KeyValuePair<IBitVector, IReadOnlyCollection<IBitVector>>> trainingData)
        {
            return new Expert(trainingData);
        }

        public double CalculateLoss(IBitVector bitVector, IBitVector label)
        {
            return CalculateLossPerLabel(bitVector, label);
        }

        public IReadOnlyDictionary<IBitVector, double> CalculateLosses(IBitVector bitVector)
        {
            var lossesPerLabel = CalculateLossesPerLabel(bitVector);
            return Enumerable.Range(0, _labels.Count).ToDictionary(index => _labels[index], index => lossesPerLabel[index]);
        }

        private double[] CalculateLossesPerLabel(IBitVector bitVector)
        {
            var lossesPerLabel = _labels.Select(currentLabel => CalculateLossPerLabel(bitVector, currentLabel)).ToArray();
            //Softmax(lossesPerLabel);
            return lossesPerLabel;
        }

        private double CalculateLossPerLabel(IBitVector bitVector, IBitVector label)
        {
            var maxSpanningTreeWeightLoss = 0;
            foreach (var edge in _maxSpanningTreesEdges[label].SelectMany(edge => edge))
            {
                var edgeIndex = GetEdgeIndexByVertexValues(bitVector.IsActive(edge.Item1), bitVector.IsActive(edge.Item2));
                if (edgeIndex != edge.Item3)
                {
                    maxSpanningTreeWeightLoss += edge.Item4;
                }
            }

            return (double)maxSpanningTreeWeightLoss / _maxSpanningTreesWeightSums[label];
        }

        private static IEnumerable<IReadOnlyCollection<Tuple<uint, uint, byte, int>>> GetMaxSpanningTreesEdges(IReadOnlyCollection<IBitVector> bitVectors, int maxSpanningTreesCount)
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
            };
        }

        private static int[,,] CalculateWeightMatrix(IReadOnlyCollection<IBitVector> bitVectors)
        {
            uint vertexCount = bitVectors.First().Count;

            var weightMatrix = new int[vertexCount, vertexCount, 4];
            foreach (var bitVector in bitVectors)
            {
                for (uint firstVertexIndex = 0; firstVertexIndex < vertexCount - 1; firstVertexIndex++)
                {
                    var firstVertexValue = bitVector.IsActive(firstVertexIndex);
                    for (uint secondVertexIndex = firstVertexIndex + 1; secondVertexIndex < vertexCount; secondVertexIndex++)
                    {
                        weightMatrix[firstVertexIndex, secondVertexIndex, GetEdgeIndexByVertexValues(firstVertexValue, bitVector.IsActive(secondVertexIndex))]++;
                    }
                }
            }

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
                AppendEdge(maxValidEdge, connectedVertexIndices, notConnectedVertexIndices, vertexValues);
                yield return maxValidEdge;
            }
        }

        private static void AppendEdge(Tuple<uint, uint, byte, int> edge, ISet<uint> connectedVertexIndices, ICollection<uint> notConnectedVertexIndices, bool[] vertexValues)
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

        private static void Softmax(double[] values)
        {
            double divisor = 0;
            for (int i = 0; i < values.Length; i++)
            {
                var exp = Math.Exp(values[i]);
                divisor += exp;
                values[i] = exp;
            }

            for (int i = 0; i < values.Length; i++)
            {
                values[i] /= divisor;
            }
        }
    }
}