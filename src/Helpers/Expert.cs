using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
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

        private IReadOnlyDictionary<IBitVector, IEnumerable<IDictionary<ValueTuple<uint, uint, byte>, int>>> _maxSpanningTreesEdges;
        private IReadOnlyDictionary<IBitVector, uint> _maxSpanningTreesWeightSums;
        private IList<IBitVector> _labels;

        public Expert()
        {
        }

        private Expert(IEnumerable<KeyValuePair<IBitVector, IReadOnlyCollection<IBitVector>>> trainingData)
        {
            MaxSpanningTreesEdges = trainingData.Select(trainingItem => new KeyValuePair<IBitVector, IEnumerable<IEnumerable<Tuple<uint, uint, byte, int>>>>(
                trainingItem.Key,
                GetMaxSpanningTreesEdges(trainingItem.Value, MaxSpanningTreesPerLabel)
                .Select(edges => edges.Select(t => Tuple.Create(t.Item1, t.Item2, t.Item3, (int)t.Item4)))));
            MaxSpanningTreesWeightSums = _maxSpanningTreesEdges.ToDictionary(
                edges => edges.Key,
                edges => (uint)edges.Value.SelectMany(d => d.Values).Sum());
        }

        internal static IExpert Create(IEnumerable<KeyValuePair<IBitVector, IReadOnlyCollection<IBitVector>>> trainingData)
        {
            return new Expert(trainingData);
        }

        [JsonInclude]
        public IEnumerable<KeyValuePair<IBitVector, IEnumerable<IEnumerable<Tuple<uint, uint, byte, int>>>>> MaxSpanningTreesEdges
        {
            get => _maxSpanningTreesEdges.Select(p =>
                new KeyValuePair<IBitVector, IEnumerable<IEnumerable<Tuple<uint, uint, byte, int>>>>(
                    p.Key,
                    p.Value.Select(e => e.Select(p => Tuple.Create(p.Key.Item1, p.Key.Item2, p.Key.Item3, p.Value)))));

            private set
            {
                _maxSpanningTreesEdges = value
                    .ToDictionary(
                        t => t.Key,
                        t => (IEnumerable<IDictionary<ValueTuple<uint, uint, byte>, int>>)
                             t.Value.Select(edges => edges.ToDictionary(
                             t => ValueTuple.Create(t.Item1, t.Item2, t.Item3),
                             t => t.Item4))
                             .ToList());
                _labels = _maxSpanningTreesEdges.Keys.ToArray();
            }
        }

        [JsonInclude]
        public IEnumerable<KeyValuePair<IBitVector, uint>> MaxSpanningTreesWeightSums
        {
            get => _maxSpanningTreesWeightSums;

            private set
            {
                _maxSpanningTreesWeightSums =
                    value as IReadOnlyDictionary<IBitVector, uint> ??
                    value.ToDictionary(p => p.Key, p => p.Value);
            }
        }

        public IReadOnlyDictionary<IBitVector, double> CalculateMatchingScores(IBitVector bitVector)
        {
            return CalculateMatchingScores(GetBitActivityBoolArray(bitVector));
        }

        public bool FineTune(IBitVector bitVector, IBitVector label)
        {
            var bitActivityBoolArray = GetBitActivityBoolArray(bitVector);
            var matchingScores = CalculateMatchingScores(bitActivityBoolArray);
            var rightLabelMatchingScore = matchingScores[label];

            bool anyWrongLabel = false;

            foreach (var wrongLabel in _labels.Where(l => matchingScores[l] > rightLabelMatchingScore))
            {
                anyWrongLabel = true;

                foreach (var wrongLabelEdges in _maxSpanningTreesEdges[wrongLabel])
                {
                    foreach (var commonEdge in wrongLabelEdges.Keys
                        .Where(edge => edge.Item3 == GetEdgeIndexByVertexValues(bitActivityBoolArray[edge.Item1], bitActivityBoolArray[edge.Item2])))
                    {
                        foreach (var rightLabelEdges in _maxSpanningTreesEdges[label])
                        {
                            if (rightLabelEdges.ContainsKey(commonEdge))
                            {
                                rightLabelEdges[commonEdge]++;
                                wrongLabelEdges[commonEdge]--;
                            }
                        }
                    }
                }
            }

            return anyWrongLabel;
        }

        private IReadOnlyDictionary<IBitVector, double> CalculateMatchingScores(bool[] bitActivityBoolArray)
        {
            var matchingScores = new double[_labels.Count];

            for (var labelIndex = 0; labelIndex < _labels.Count; labelIndex++)
            {
                var label = _labels[labelIndex];
                var maxSpanningTreeWeight = _maxSpanningTreesEdges[label]
                    .SelectMany(edges => edges)
                    .Where(edge => edge.Key.Item3 == GetEdgeIndexByVertexValues(bitActivityBoolArray[edge.Key.Item1], bitActivityBoolArray[edge.Key.Item2]))
                    .Sum(edge => edge.Value);
                matchingScores[labelIndex] = (double)maxSpanningTreeWeight / _maxSpanningTreesWeightSums[label];
            }

            SoftMax(matchingScores);

            return Enumerable.Range(0, _labels.Count).ToDictionary(
                labelIndex => _labels[labelIndex],
                labelIndex => matchingScores[labelIndex]);
        }

        private static IEnumerable<IEnumerable<ValueTuple<uint, uint, byte, uint>>> GetMaxSpanningTreesEdges(IReadOnlyCollection<IBitVector> bitVectors, int maxSpanningTreesCount)
        {
            var weightMatrix = CalculateWeightMatrix(bitVectors);

            int maxSpanningTreesCounter = 0;
            while (true)
            {
                var maxSpanningTreeEdges = GetMaxSpanningTreeEdges(weightMatrix);
                yield return maxSpanningTreeEdges;

                maxSpanningTreesCounter++;
                if (maxSpanningTreesCounter >= maxSpanningTreesCount)
                {
                    yield break;
                }

                foreach (var maxSpanningTreeEdge in maxSpanningTreeEdges)
                {
                    weightMatrix[maxSpanningTreeEdge.Item1, maxSpanningTreeEdge.Item2, maxSpanningTreeEdge.Item3] = uint.MinValue;
                }
            }
        }

        private static uint[,,] CalculateWeightMatrix(IReadOnlyCollection<IBitVector> bitVectors)
        {
            var vertexCount = bitVectors.First().Count;
            var bitActivityBoolArray = new bool[vertexCount];

            var weightMatrix = new uint[vertexCount, vertexCount, 4];
            foreach (var bitVector in bitVectors)
            {
                FillBitActivityBoolArray(bitVector, bitActivityBoolArray);

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

        private static IEnumerable<ValueTuple<uint, uint, byte, uint>> GetMaxSpanningTreeEdges(uint[,,] weightMatrix)
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

        private static void AppendEdge(ValueTuple<uint, uint, byte, uint> edge, ISet<uint> connectedVertexIndices, ICollection<uint> notConnectedVertexIndices, bool[] vertexValues)
        {
            vertexValues[edge.Item1] = edge.Item3 == TrueFalseEdgeIndex || edge.Item3 == TrueTrueEdgeIndex;
            vertexValues[edge.Item2] = edge.Item3 == FalseTrueEdgeIndex || edge.Item3 == TrueTrueEdgeIndex;
            connectedVertexIndices.Add(edge.Item1);
            connectedVertexIndices.Add(edge.Item2);
            notConnectedVertexIndices.Remove(edge.Item1);
            notConnectedVertexIndices.Remove(edge.Item2);
        }

        private static bool TryGetNextMaxValidEdge(uint[,,] weightMatrix, IEnumerable<ValueTuple<uint, uint, byte>> validEdges, out ValueTuple<uint, uint, byte, uint> maxValidEdge)
        {
            var maxValidEdgeWeight = uint.MinValue;
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

        private static bool[] GetBitActivityBoolArray(IBitVector bitVector)
        {
            var bitActivityBoolArray = new bool[bitVector.Count];

            FillBitActivityBoolArray(bitVector, bitActivityBoolArray);

            return bitActivityBoolArray;
        }

        private static void FillBitActivityBoolArray(IBitVector bitVector, bool[] bitActivityBoolArray)
        {
            for (uint bitIndex = 0; bitIndex < bitVector.Count; bitIndex++)
            {
                bitActivityBoolArray[bitIndex] = bitVector.IsActive(bitIndex);
            }
        }
    }
}