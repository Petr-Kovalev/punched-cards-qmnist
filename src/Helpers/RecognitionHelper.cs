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
                    var matchingScores = CalculateMatchingScores(dataItem.Item1, expertsPerKey, puncher).ToList();
                    var matchingScoresPerLabel = CalculateMatchingScoresPerLabel(matchingScores, bitVectorFactory, topPunchedCardsCount);
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

        internal static IReadOnlyDictionary<string, IExpert> CreateExperts(IReadOnlyDictionary<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> trainingPunchedCardsPerKeyPerLabel)
        {
            return trainingPunchedCardsPerKeyPerLabel
                .AsParallel()
                .Select(punchedCardsPerKeyPerLabel =>
                    Tuple.Create(punchedCardsPerKeyPerLabel.Key, Expert.Create(punchedCardsPerKeyPerLabel.Value)))
                .ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
        }

        private static IEnumerable<KeyValuePair<IBitVector, double>> CalculateMatchingScoresPerLabel(
            IReadOnlyCollection<IReadOnlyDictionary<IBitVector, double>> matchingScoresCollection,
            IBitVectorFactory bitVectorFactory,
            int? topPunchedCardsCount)
        {
            var topMatchingScores =
                !topPunchedCardsCount.HasValue
                    ? matchingScoresCollection
                    : matchingScoresCollection
                        .OrderByDescending(p => p.Values.Max())
                        .Take(topPunchedCardsCount.Value)
                        .ToList();

            return DataHelper.GetLabels(bitVectorFactory).Select(label =>
                KeyValuePair.Create(label, topMatchingScores.Sum(matchingScores => matchingScores[label])));
        }

        private static IEnumerable<IReadOnlyDictionary<IBitVector, double>> CalculateMatchingScores(
            IBitVector bitVector,
            IEnumerable<KeyValuePair<string, IExpert>> expertsPerKey,
            IPuncher<string, IBitVector, IBitVector> puncher)
        {
            return expertsPerKey.Select(expertPerKey => expertPerKey.Value.CalculateMatchingScores(puncher.Punch(expertPerKey.Key, bitVector).Input));
        }
    }
}