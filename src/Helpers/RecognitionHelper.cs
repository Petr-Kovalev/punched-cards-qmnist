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
            int? topPunchedCardsCount)
        {
            var counters = DataHelper.GetLabels(bitVectorFactory).ToDictionary(label => label, _ => new int[1]);

            data
                .AsParallel()
                .ForAll(dataItem =>
                {
                    var matchinScoresPerPunchedCard = new Dictionary<string, IReadOnlyDictionary<IBitVector, double>>();
                    foreach (var punchedCardsCollectionItem in punchedCardsCollection)
                    {
                        var punchedInput = puncher.Punch(punchedCardsCollectionItem.Key, dataItem.Item1).Input;
                        matchinScoresPerPunchedCard.Add(punchedCardsCollectionItem.Key, experts[punchedCardsCollectionItem.Key].CalculateMatchingScores(punchedInput));
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

                    IEnumerable<string> punchedCardKeys;
                    if (!topPunchedCardsCount.HasValue)
                    {
                        punchedCardKeys = matchinScoresPerPunchedCard.Keys;
                    }
                    else
                    {
                        punchedCardKeys = matchinScoresPerPunchedCard.OrderByDescending(p => p.Value.Values.Max()).Take(topPunchedCardsCount.Value).Select(p => p.Key);
                    }

                    var matchingScoresPerLabel = new Dictionary<IBitVector, double>();
                    foreach (var label in counters.Keys)
                    {
                        matchingScoresPerLabel.Add(label, 0);
                    }
                    foreach (var punchedCardKey in punchedCardKeys)
                    {
                        var matchingScores = matchinScoresPerPunchedCard[punchedCardKey];
                        foreach (var label in counters.Keys)
                        {
                            matchingScoresPerLabel[label] += matchingScores[label];
                        }
                    }

                    var topLabel = matchingScoresPerLabel
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