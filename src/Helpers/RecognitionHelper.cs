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
            IDictionary<string, IDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> punchedCardsCollection,
            IPuncher<string, IBitVector, IBitVector> puncher,
            IReadOnlyDictionary<string, IExpert> experts)
        {
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

        internal static IReadOnlyDictionary<string, IExpert> CreateExperts(IDictionary<string, IDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> punchedCardsCollection)
        {
            return punchedCardsCollection
                .AsParallel()
                .Select(punchedCardsCollectionItem =>
                    Tuple.Create(punchedCardsCollectionItem.Key, Expert.Create(punchedCardsCollectionItem.Value)))
                .ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
        }

        internal static double CalculateMaxLoss(KeyValuePair<string, IDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> punchedCardPerLabel, IReadOnlyDictionary<string, IExpert> experts, IBitVector label)
        {
            var expert = experts[punchedCardPerLabel.Key];
            return punchedCardPerLabel.Value[label].Max(input => expert.CalculateLoss(input, label));
        }

        internal static double CalculateMaxLossSum(KeyValuePair<string, IDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> punchedCardPerLabel, IReadOnlyDictionary<string, IExpert> experts)
        {
            var expert = experts[punchedCardPerLabel.Key];
            return punchedCardPerLabel.Value.Sum(labelAndInputs => labelAndInputs.Value.Max(input => expert.CalculateLoss(input, labelAndInputs.Key)));
        }

        private static IDictionary<IBitVector, IDictionary<string, double>>
            CalculateLossPerLabelPerPunchedCard(
                IDictionary<string, IDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> punchedCardsCollection,
                IBitVector input,
                IPuncher<string, IBitVector, IBitVector> puncher,
                IReadOnlyDictionary<string, IExpert> experts)
        {
            var lossPerLabelPerPunchedCard = new Dictionary<IBitVector, IDictionary<string, double>>();

            foreach (var punchedCardsCollectionItem in punchedCardsCollection)
            {
                var expert = experts[punchedCardsCollectionItem.Key];
                var punchedInput = puncher.Punch(punchedCardsCollectionItem.Key, input).Input;
                foreach (var label in punchedCardsCollectionItem.Value)
                {
                    var lossPerLabel = expert.CalculateLoss(punchedInput, label.Key);

                    if (!lossPerLabelPerPunchedCard.TryGetValue(label.Key, out var dictionary))
                    {
                        dictionary = new Dictionary<string, double>();
                        lossPerLabelPerPunchedCard[label.Key] = dictionary;
                    }

                    dictionary.Add(punchedCardsCollectionItem.Key, lossPerLabel);
                }
            }

            return lossPerLabelPerPunchedCard;
        }
    }
}