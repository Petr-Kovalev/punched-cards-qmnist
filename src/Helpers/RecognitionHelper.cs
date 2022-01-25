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
            var experts = CreateExperts(punchedCardsCollection);

            var correctRecognitionsPerLabel =
                new ConcurrentDictionary<IBitVector, int>();

            data
                .AsParallel()
                .ForAll(dataItem =>
                {
                    var lossPerLabelPerPunchedCard =
                        CalculateLossPerLabelPerPunchedCard(
                            punchedCardsCollection,
                            dataItem.Item1,
                            puncher,
                            experts);
                    var topLabel = lossPerLabelPerPunchedCard
                        .MinBy(p => p.Value.Sum(keyScore => keyScore.Value))
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

        private static IDictionary<int, IExpert> CreateExperts(IDictionary<string, IDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> punchedCardsCollection)
        {
            return punchedCardsCollection
                .AsParallel()
                .SelectMany(punchedCardsCollectionItem =>
                    punchedCardsCollectionItem.Value
                        .Select(label =>
                            new Tuple<int, IExpert>(
                                GetExpertKey(punchedCardsCollectionItem.Key, label.Key),
                                Expert.Create(label.Value))))
                .ToDictionary(t => t.Item1, t => t.Item2);
        }

        private static IDictionary<IBitVector, IDictionary<string, double>>
            CalculateLossPerLabelPerPunchedCard(
                IDictionary<string, IDictionary<IBitVector, IReadOnlyCollection<IBitVector>>>
                    punchedCardsCollection,
                IBitVector input,
                IPuncher<string, IBitVector, IBitVector> puncher,
                IDictionary<int, IExpert> expert)
        {
            var lossPerLabelPerPunchedCard = new Dictionary<IBitVector, IDictionary<string, double>>();

            foreach (var punchedCardsCollectionItem in punchedCardsCollection)
            {
                var punchedInput = puncher.Punch(punchedCardsCollectionItem.Key, input).Input;
                foreach (var label in punchedCardsCollectionItem.Value)
                {
                    ProcessTheSpecificLabel(
                        lossPerLabelPerPunchedCard,
                        punchedCardsCollectionItem.Key,
                        label.Key,
                        expert[GetExpertKey(punchedCardsCollectionItem.Key, label.Key)],
                        punchedInput);
                }
            }

            return lossPerLabelPerPunchedCard;
        }

        private static int GetExpertKey(string punchedCardsCollectionItemKey, IBitVector labelKey)
        {
            return HashCode.Combine(punchedCardsCollectionItemKey, labelKey);
        }

        private static void ProcessTheSpecificLabel(
            IDictionary<IBitVector, IDictionary<string, double>> lossPerLabelPerPunchedCard,
            string punchedCardKey,
            IBitVector key,
            IExpert expert,
            IBitVector punchedInput)
        {
            var lossPerLabel = expert.CalculateLoss(punchedInput);

            if (!lossPerLabelPerPunchedCard.TryGetValue(key, out var dictionary))
            {
                dictionary = new Dictionary<string, double>();
                lossPerLabelPerPunchedCard[key] = dictionary;
            }

            dictionary.Add(punchedCardKey, lossPerLabel);
        }

        internal static double CalculateBitVectorsMaxLoss(IReadOnlyCollection<IBitVector> bitVectors)
        {
            var expert = Expert.Create(bitVectors);
            return bitVectors.Max(bitVector => expert.CalculateLoss(bitVector));
        }
    }
}