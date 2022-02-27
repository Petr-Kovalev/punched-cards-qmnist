using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PunchedCards.BitVectors;

namespace PunchedCards.Helpers
{
    internal static class RecognitionHelper
    {
        internal static IEnumerable<KeyValuePair<IBitVector, int>> CountCorrectRecognitions(
            IEnumerable<Tuple<IBitVector, IBitVector>> data,
            IReadOnlyDictionary<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> punchedCardsCollection,
            IPuncher<string, IBitVector, IBitVector> puncher,
            IReadOnlyDictionary<string, IExpert> experts,
            IBitVectorFactory bitVectorFactory)
        {
            var counters = DataHelper.GetLabels(bitVectorFactory).ToDictionary(label => label, label => new int[1]);

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
                        Interlocked.Increment(ref counters[topLabel][0]);
                    }
                });

            return counters.ToDictionary(p => p.Key, p => p.Value[0]);
        }

        internal static IReadOnlyDictionary<string, IExpert> CreateExperts(IReadOnlyDictionary<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> punchedCardsCollection)
        {
            return punchedCardsCollection
                .AsParallel()
                .Select(punchedCardsCollectionItem =>
                    Tuple.Create(punchedCardsCollectionItem.Key, Expert.Create(punchedCardsCollectionItem.Value)))
                .ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
        }

        internal static double CalculateMaxLoss(KeyValuePair<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> punchedCardPerLabel, IReadOnlyDictionary<string, IExpert> experts, IBitVector label)
        {
            var expert = experts[punchedCardPerLabel.Key];
            return punchedCardPerLabel.Value[label].Max(input => expert.CalculateLoss(input, label));
        }

        internal static double CalculateMaxLossSum(KeyValuePair<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> punchedCardPerLabel, IReadOnlyDictionary<string, IExpert> experts)
        {
            var expert = experts[punchedCardPerLabel.Key];
            return punchedCardPerLabel.Value.Sum(labelAndInputs => labelAndInputs.Value.Max(input => expert.CalculateLoss(input, labelAndInputs.Key)));
        }

        private static IReadOnlyDictionary<IBitVector, IReadOnlyDictionary<string, double>>
            CalculateLossPerLabelPerPunchedCard(
                IReadOnlyDictionary<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> punchedCardsCollection,
                IBitVector input,
                IPuncher<string, IBitVector, IBitVector> puncher,
                IReadOnlyDictionary<string, IExpert> experts)
        {
            var lossPerLabelPerPunchedCard = new Dictionary<IBitVector, IReadOnlyDictionary<string, double>>();

            foreach (var punchedCardsCollectionItem in punchedCardsCollection)
            {
                var expert = experts[punchedCardsCollectionItem.Key];
                var punchedInput = puncher.Punch(punchedCardsCollectionItem.Key, input).Input;
                var losses = expert.CalculateLosses(punchedInput);
                foreach (var label in punchedCardsCollectionItem.Value)
                {
                    if (!lossPerLabelPerPunchedCard.TryGetValue(label.Key, out var dictionary))
                    {
                        dictionary = new Dictionary<string, double>();
                        lossPerLabelPerPunchedCard.Add(label.Key, dictionary);
                    }

                    ((Dictionary<string, double>)dictionary).Add(punchedCardsCollectionItem.Key, losses[label.Key]);
                }
            }

            return lossPerLabelPerPunchedCard;
        }
    }
}