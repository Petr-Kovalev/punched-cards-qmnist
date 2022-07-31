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
            IBitVectorFactory bitVectorFactory,
            int? topExpertsCount)
        {
            var counters = DataHelper.GetLabels(bitVectorFactory).ToDictionary(label => label, _ => new int[1]);

            var labels = DataHelper.GetLabels(bitVectorFactory).ToList();
            data
                .AsParallel()
                .ForAll(dataItem =>
                {
                    var tt = new Dictionary<string, double[]>();
                    foreach (var punchedCardsCollectionItem in punchedCardsCollection)
                    {
                        var expert = experts[punchedCardsCollectionItem.Key];
                        var punchedInput = puncher.Punch(punchedCardsCollectionItem.Key, dataItem.Item1).Input;
                        var matchingScores = expert.CalculateMatchingScores(punchedInput);

                        tt.Add(punchedCardsCollectionItem.Key, labels.Select(l => matchingScores[l]).ToArray());
                    }

                    if (topExpertsCount.HasValue)
                    {
                        var topKeys = tt.Keys.OrderByDescending(k => tt[k].Max()).Take(topExpertsCount.Value);
                        tt = topKeys.ToDictionary(k => k, k => tt[k]);
                    }

                    //foreach (var key in tt.Keys)
                    //{
                    //    var arr = tt[key];
                    //    var max = arr.Max();
                    //    for (int i=0; i<arr.Length; i++)
                    //    {
                    //        if (arr[i] < max)
                    //        {
                    //            arr[i] = 0;
                    //        }
                    //    }
                    //}

                    var sums = new Dictionary<IBitVector, double>();
                    for(int i=0; i<labels.Count; i++)
                    {
                        sums[labels[i]] = tt.Keys.Sum(k => tt[k][i]);
                    }
                    var topLabel = sums
                        .MaxBy(p => p.Value)
                        .Key;


                    //var matchingScoresPerLabelPerPunchedCard =
                    //    CalculateMatchingScoresPerLabelPerPunchedCard(
                    //        punchedCardsCollection,
                    //        dataItem.Item1,
                    //        puncher,
                    //        experts);
                    //var topLabel = matchingScoresPerLabelPerPunchedCard
                    //    .MaxBy(p => p.Value.Sum(keyScore => keyScore.Value))
                    //    .Key;

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

        //private static IReadOnlyDictionary<IBitVector, IReadOnlyDictionary<string, double>>
        //    CalculateMatchingScoresPerLabelPerPunchedCard(
        //        IReadOnlyDictionary<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> punchedCardsCollection,
        //        IBitVector input,
        //        IPuncher<string, IBitVector, IBitVector> puncher,
        //        IReadOnlyDictionary<string, IExpert> experts)
        //{
        //    var matchinScoresPerLabelPerPunchedCard = new Dictionary<IBitVector, IReadOnlyDictionary<string, double>>();

        //    foreach (var punchedCardsCollectionItem in punchedCardsCollection)
        //    {
        //        var expert = experts[punchedCardsCollectionItem.Key];
        //        var punchedInput = puncher.Punch(punchedCardsCollectionItem.Key, input).Input;
        //        var matchingScores = expert.CalculateMatchingScores(punchedInput);
        //        foreach (var label in punchedCardsCollectionItem.Value)
        //        {
        //            if (!matchinScoresPerLabelPerPunchedCard.TryGetValue(label.Key, out var dictionary))
        //            {
        //                dictionary = new Dictionary<string, double>();
        //                matchinScoresPerLabelPerPunchedCard.Add(label.Key, dictionary);
        //            }

        //            ((Dictionary<string, double>)dictionary).Add(punchedCardsCollectionItem.Key, matchingScores[label.Key]);
        //        }
        //    }

        //    return matchinScoresPerLabelPerPunchedCard;
        //}
    }
}