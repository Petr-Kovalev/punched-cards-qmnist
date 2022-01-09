﻿using System;
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
                        .MaxBy(p => p.Value.Sum(keyScore => keyScore.Value))
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

        private static IDictionary<int, IAdjacencyMatrix> CalculateAdjacencyMatrices(IDictionary<string, IDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> punchedCardsCollection)
        {
            return punchedCardsCollection
                .AsParallel()
                .SelectMany(punchedCardsCollectionItem =>
                    punchedCardsCollectionItem.Value
                        .Select(label =>
                            new Tuple<int, IAdjacencyMatrix>(
                                GetAdjacencyMatrixKey(punchedCardsCollectionItem.Key, label.Key),
                                new AdjacencyMatrix(label.Value))))
                .ToDictionary(t => t.Item1, t => t.Item2);
        }

        internal static IDictionary<IBitVector, IDictionary<string, double>>
            CalculateMatchingScoresPerLabelPerPunchedCard(
                IDictionary<string, IDictionary<IBitVector, IReadOnlyCollection<IBitVector>>>
                    punchedCardsCollection,
                IBitVector input,
                IPuncher<string, IBitVector, IBitVector> puncher,
                IDictionary<int, IAdjacencyMatrix> adjacencyMatrices)
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
                        adjacencyMatrices[GetAdjacencyMatrixKey(punchedCardsCollectionItem.Key, label.Key)],
                        punchedInput);
                }
            }

            return matchingScoresPerLabelPerPunchedCard;
        }

        private static int GetAdjacencyMatrixKey(string punchedCardsCollectionItemKey, IBitVector labelKey)
        {
            return HashCode.Combine(punchedCardsCollectionItemKey, labelKey);
        }

        private static void ProcessTheSpecificLabel(
            IDictionary<IBitVector, IDictionary<string, double>> matchingScoresPerLabelPerPunchedCard,
            string punchedCardKey,
            IBitVector key,
            IAdjacencyMatrix adjacencyMatrix,
            IBitVector punchedInput)
        {
            var matchingScorePerLabel = CalculateMatchingScore(punchedInput, adjacencyMatrix);

            if (!matchingScoresPerLabelPerPunchedCard.TryGetValue(key, out var dictionary))
            {
                dictionary = new Dictionary<string, double>();
                matchingScoresPerLabelPerPunchedCard[key] = dictionary;
            }

            dictionary.Add(punchedCardKey, matchingScorePerLabel);
        }

        internal static double CalculateMatchingScore(IBitVector punchedInput, IAdjacencyMatrix adjacencyMatrix)
        {
            return (double)adjacencyMatrix.CalculateMaxSpanningTreeMatchingScore(punchedInput.ActiveBitIndices) / adjacencyMatrix.MaxSpanningTree;
        }

        internal static double CalculateBitVectorsScore(IReadOnlyCollection<IBitVector> bitVectors)
        {
            var adjacencyMatrix = new AdjacencyMatrix(bitVectors);
            return 1 - (double)adjacencyMatrix.MaxSpanningTree / (bitVectors.Count * (adjacencyMatrix.Size - 1));
        }
    }
}