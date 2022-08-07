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
            IPuncher<string, IBitVector, IBitVector> puncher,
            IReadOnlyDictionary<string, IExpert> experts,
            IBitVectorFactory bitVectorFactory,
            int? topPunchedCardsCount)
        {
            var counters = DataHelper.GetLabels(bitVectorFactory).ToDictionary(label => label, _ => new int[1]);

            data
                .AsParallel()
                .ForAll(dataItem =>
                {
                    var matchingScoresPerPunchedCard = new Dictionary<string, IReadOnlyDictionary<IBitVector, double>>();
                    foreach (var expert in experts)
                    {
                        var punchedInput = puncher.Punch(expert.Key, dataItem.Item1).Input;
                        matchingScoresPerPunchedCard.Add(expert.Key, expert.Value.CalculateMatchingScores(punchedInput));
                    }

                    var punchedCardKeys = !topPunchedCardsCount.HasValue ? matchingScoresPerPunchedCard.Keys : matchingScoresPerPunchedCard.OrderByDescending(p => p.Value.Values.Max()).Take(topPunchedCardsCount.Value).Select(p => p.Key);

                    var matchingScoresPerLabel = new Dictionary<IBitVector, double>();
                    foreach (var label in counters.Keys)
                    {
                        matchingScoresPerLabel.Add(label, 0);
                    }
                    foreach (var punchedCardKey in punchedCardKeys)
                    {
                        var matchingScores = matchingScoresPerPunchedCard[punchedCardKey];
                        foreach (var label in counters.Keys)
                        {
                            matchingScoresPerLabel[label] += matchingScores[label];
                        }
                    }

                    var topLabel = matchingScoresPerLabel
                        .MaxBy(p => p.Value)
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
    }
}