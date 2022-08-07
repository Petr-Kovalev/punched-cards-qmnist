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
            IReadOnlyDictionary<string, IExpert> expertsPerKey,
            IBitVectorFactory bitVectorFactory,
            int? topPunchedCardsCount)
        {
            var counters = DataHelper.GetLabels(bitVectorFactory).ToDictionary(label => label, _ => new int[1]);

            data
                .AsParallel()
                .ForAll(dataItem =>
                {
                    var matchingScoresPerKey = new Dictionary<string, IReadOnlyDictionary<IBitVector, double>>();
                    foreach (var expertPerKey in expertsPerKey)
                    {
                        var punchedInput = puncher.Punch(expertPerKey.Key, dataItem.Item1).Input;
                        matchingScoresPerKey.Add(expertPerKey.Key, expertPerKey.Value.CalculateMatchingScores(punchedInput));
                    }

                    var punchedCardKeys = 
                        !topPunchedCardsCount.HasValue ?
                            matchingScoresPerKey.Keys : 
                            matchingScoresPerKey
                                .OrderByDescending(p => p.Value.Values.Max())
                                .Take(topPunchedCardsCount.Value)
                                .Select(p => p.Key);

                    var matchingScoresPerLabel = CalculateMatchingScoresPerLabel(counters.Keys, punchedCardKeys, matchingScoresPerKey);
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

        private static Dictionary<IBitVector, double> CalculateMatchingScoresPerLabel(IReadOnlyCollection<IBitVector> labels, IEnumerable<string> punchedCardKeys,
            IReadOnlyDictionary<string, IReadOnlyDictionary<IBitVector, double>> matchingScoresPerKey)
        {
            var matchingScoresPerLabel = labels.ToDictionary(label => label, _ => 0d);

            foreach (var punchedCardKey in punchedCardKeys)
            {
                var matchingScores = matchingScoresPerKey[punchedCardKey];
                foreach (var label in labels)
                {
                    matchingScoresPerLabel[label] += matchingScores[label];
                }
            }

            return matchingScoresPerLabel;
        }

        internal static IReadOnlyDictionary<string, IExpert> CreateExperts(IReadOnlyDictionary<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> trainingPunchedCardsPerKeyPerLabel)
        {
            return trainingPunchedCardsPerKeyPerLabel
                .AsParallel()
                .Select(punchedCardsPerKeyPerLabel =>
                    Tuple.Create(punchedCardsPerKeyPerLabel.Key, Expert.Create(punchedCardsPerKeyPerLabel.Value)))
                .ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
        }
    }
}