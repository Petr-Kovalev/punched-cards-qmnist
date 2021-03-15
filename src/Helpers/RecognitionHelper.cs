using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PunchedCards.BitVectors;

namespace PunchedCards.Helpers
{
    internal static class RecognitionHelper
    {
        internal static IEnumerable<KeyValuePair<IBitVector, int>> CountCorrectRecognitions(
            IEnumerable<Tuple<IBitVector, IBitVector>> data,
            IDictionary<string, IDictionary<IBitVector, IReadOnlyCollection<IBitVector>>>
                punchedCardsCollection,
            IPuncher<string, IBitVector, IBitVector> puncher)
        {
            var adjacencyMatrices = CalculateAdjacencyMatrices(punchedCardsCollection);

            var correctRecognitionsPerLabel =
                new ConcurrentDictionary<IBitVector, int>();

            data
                .AsParallel()
                .ForAll(dataItem =>
                {
                    var matchingScoresPerLabelPerPunchedCard =
                        CalculateMatchingScoresPerLabelPerPunchedCard(
                            punchedCardsCollection,
                            dataItem.Item1,
                            puncher,
                            adjacencyMatrices);
                    var topLabel = matchingScoresPerLabelPerPunchedCard
                        .OrderByDescending(p => p.Value.Sum(keyScore => keyScore.Value))
                        .First()
                        .Key;
                    if (topLabel.Equals(dataItem.Item2))
                    {
                        correctRecognitionsPerLabel.AddOrUpdate(
                            dataItem.Item2,
                            _ => 1,
                            (_, value) => value + 1);
                    }
                });

            return correctRecognitionsPerLabel;
        }

        private static IDictionary<int, AdjacencyMatrix> CalculateAdjacencyMatrices(IDictionary<string, IDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> punchedCardsCollection)
        {
            return punchedCardsCollection
                .AsParallel()
                .SelectMany(punchedCardsCollectionItem =>
                    punchedCardsCollectionItem.Value
                        .Select(label =>
                            new Tuple<int, AdjacencyMatrix>(
                                GetAdjacencyMatrixKey(punchedCardsCollectionItem.Key, label.Key),
                                new AdjacencyMatrix(label.Value))))
                .ToDictionary(t => t.Item1, t => t.Item2);
        }

        private static int GetAdjacencyMatrixKey(string punchedCardsCollectionItemKey, IBitVector labelKey)
        {
            return HashCode.Combine(punchedCardsCollectionItemKey, labelKey);
        }

        internal static IDictionary<IBitVector, IDictionary<string, double>>
            CalculateMatchingScoresPerLabelPerPunchedCard(
                IDictionary<string, IDictionary<IBitVector, IReadOnlyCollection<IBitVector>>>
                    punchedCardsCollection,
                IBitVector input,
                IPuncher<string, IBitVector, IBitVector> puncher,
                IDictionary<int, AdjacencyMatrix> adjacencyMatrices)
        {
            var matchingScoresPerLabelPerPunchedCard = new Dictionary<IBitVector, IDictionary<string, double>>();

            foreach (var punchedCardsCollectionItem in punchedCardsCollection)
            {
                var punchedInput = puncher.Punch(punchedCardsCollectionItem.Key, input).Input;
                foreach (var label in punchedCardsCollectionItem.Value)
                {
                    ProcessTheSpecificLabel(
                        matchingScoresPerLabelPerPunchedCard,
                        punchedCardsCollectionItem.Key,
                        label.Key,
                        label.Value.Count,
                        adjacencyMatrices[GetAdjacencyMatrixKey(punchedCardsCollectionItem.Key, label.Key)],
                        punchedInput);
                }
            }

            return matchingScoresPerLabelPerPunchedCard;
        }

        private static void ProcessTheSpecificLabel(
            IDictionary<IBitVector, IDictionary<string, double>> matchingScoresPerLabelPerPunchedCard,
            string punchedCardKey,
            IBitVector key,
            int samplesCount,
            AdjacencyMatrix adjacencyMatrix,
            IBitVector punchedInput)
        {
            var matchingScorePerLabel = CalculateMatchingScore(punchedInput, adjacencyMatrix, samplesCount);

            if (!matchingScoresPerLabelPerPunchedCard.TryGetValue(key, out var dictionary))
            {
                dictionary = new Dictionary<string, double>();
                matchingScoresPerLabelPerPunchedCard[key] = dictionary;
            }

            dictionary.Add(punchedCardKey, matchingScorePerLabel);
        }

        internal static double CalculateMatchingScore(IBitVector punchedInput, AdjacencyMatrix adjacencyMatrix, int samplesCount)
        {
            return (double)CalculateAdjacencyMatrixScore(
                adjacencyMatrix,
                punchedInput.ActiveBitIndices) / samplesCount;
        }

        internal static double CalculateBitVectorsScore(IReadOnlyCollection<IBitVector> bitVectors)
        {
            return (double)CalculateAdjacencyMatrixScore(
                new AdjacencyMatrix(bitVectors)) / bitVectors.Count;
        }

        private static long CalculateAdjacencyMatrixScore(AdjacencyMatrix adjacencyMatrix)
        {
            return adjacencyMatrix.HalfSum;
        }

        private static long CalculateAdjacencyMatrixScore(AdjacencyMatrix adjacencyMatrix, IReadOnlyList<int> activeBitIndices)
        {
            return 2 * ActiveBitConnectionsHalfSum(adjacencyMatrix, activeBitIndices) - adjacencyMatrix.HalfSum;
        }

        private static long ActiveBitConnectionsHalfSum(AdjacencyMatrix adjacencyMatrix, IReadOnlyList<int> activeBitIndices)
        {
            long activeBitConnectionsSum = 0;

            foreach (var activeBitIndex in activeBitIndices)
            {
                for (var i = 0; i < activeBitIndex; i++)
                {
                    activeBitConnectionsSum += adjacencyMatrix.Matrix[i, activeBitIndex];
                }

                for (var j = activeBitIndex; j < adjacencyMatrix.Size; j++)
                {
                    activeBitConnectionsSum += adjacencyMatrix.Matrix[activeBitIndex, j];
                }
            }

            var activeBitIndicesCount = activeBitIndices.Count;
            for (var activeBitIndex = 0; activeBitIndex < activeBitIndicesCount; activeBitIndex++)
            {
                for (var j = activeBitIndex + 1; j < activeBitIndicesCount; j++)
                {
                    activeBitConnectionsSum -= adjacencyMatrix.Matrix[activeBitIndices[activeBitIndex], activeBitIndices[j]];
                }
            }

            return activeBitConnectionsSum;
        }
    }
}