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
            IEnumerable<KeyValuePair<string, IExpert>> expertsPerKey,
            IBitVectorFactory bitVectorFactory,
            int? topPunchedCardsCount)
        {
            var counters = DataHelper.GetLabels(bitVectorFactory).ToDictionary(label => label, _ => new int[1]);

            data
                .AsParallel()
                .ForAll(dataItem =>
                {
                    var matchingScoresPerKey = CalculateMatchingScoresPerKey(dataItem.Item1, expertsPerKey, puncher).ToList();
                    var topMatchingScoresPerKey = GetTopMatchingScoresPerKey(matchingScoresPerKey, topPunchedCardsCount);
                    var topMatchingScoresPerLabel = CalculateMatchingScoresPerLabel(topMatchingScoresPerKey, bitVectorFactory);

                    var topLabel = topMatchingScoresPerLabel.MaxBy(p => p.Value).Key;
                    if (topLabel.Equals(dataItem.Item2))
                    {
                        Interlocked.Increment(ref counters[topLabel][0]);
                    }

                });

            return counters.ToDictionary(p => p.Key, p => p.Value[0]);
        }

        internal static IReadOnlyDictionary<string, IExpert> CreateExperts(IReadOnlyDictionary<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> trainingPunchedCardsPerKeyPerLabel)
        {
            return trainingPunchedCardsPerKeyPerLabel
                .AsParallel()
                .Select(punchedCardsPerKeyPerLabel =>
                    Tuple.Create(punchedCardsPerKeyPerLabel.Key, Expert.Create(punchedCardsPerKeyPerLabel.Value)))
                .ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
        }

        private static IEnumerable<KeyValuePair<string, IReadOnlyDictionary<IBitVector, double>>> CalculateMatchingScoresPerKey(
            IBitVector bitVector,
            IEnumerable<KeyValuePair<string, IExpert>> expertsPerKey,
            IPuncher<string, IBitVector, IBitVector> puncher)
        {
            return expertsPerKey.Select(expertPerKey => KeyValuePair.Create(
                expertPerKey.Key,
                expertPerKey.Value.CalculateMatchingScores(puncher.Punch(expertPerKey.Key, bitVector).Input)));
        }

        private static IReadOnlyCollection<KeyValuePair<string, IReadOnlyDictionary<IBitVector, double>>> GetTopMatchingScoresPerKey(
            IReadOnlyCollection<KeyValuePair<string, IReadOnlyDictionary<IBitVector, double>>> matchingScoresPerKey,
            int? topPunchedCardsCount)
        {
            return !topPunchedCardsCount.HasValue
                    ? matchingScoresPerKey
                    : matchingScoresPerKey
                        .OrderByDescending(p => p.Value.Values.Max())
                        .Take(topPunchedCardsCount.Value)
                        .ToList();
        }

        private static IEnumerable<KeyValuePair<IBitVector, double>> CalculateMatchingScoresPerLabel(
            IReadOnlyCollection<KeyValuePair<string, IReadOnlyDictionary<IBitVector, double>>> matchingScoresPerKey,
            IBitVectorFactory bitVectorFactory)
        {
            return DataHelper.GetLabels(bitVectorFactory).Select(label => KeyValuePair.Create(
                label,
                matchingScoresPerKey.Sum(matchingScores => matchingScores.Value[label])));
        }
    }
}