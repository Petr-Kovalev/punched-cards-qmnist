using System;
using System.Collections.Generic;
using System.Linq;
using PunchedCards.BitVectors;
using PunchedCards.Helpers;

namespace PunchedCards
{
    internal static class PunchedCards
    {
        private static readonly IBitVectorFactory BitVectorFactory = new BitVectorFactory();

        private static void Main()
        {
            var trainingData = DataHelper.ReadTrainingData(BitVectorFactory).ToList();
            var testData = DataHelper.ReadTestData(BitVectorFactory).ToList();

            var punchedCardBitLengths = new uint[] {8, 16, 32, 64, 128, 256};

            foreach (var punchedCardBitLength in punchedCardBitLengths)
            {
                Console.WriteLine("Punched card bit length: " + punchedCardBitLength);

                IPuncher<string, IBitVector, IBitVector> puncher = new RandomPuncher(punchedCardBitLength, BitVectorFactory);
                var punchedCardsPerKeyPerLabel = GetPunchedCardsPerKeyPerLabel(trainingData, puncher);
                var experts = RecognitionHelper.CreateExperts(punchedCardsPerKeyPerLabel);

                Console.WriteLine();
                Console.WriteLine("Global top punched card:");
                WriteTrainingAndTestResults(GetGlobalTopPunchedCard(punchedCardsPerKeyPerLabel, experts), trainingData, testData, puncher, experts);

                Console.WriteLine();
                Console.WriteLine("Top punched cards per label:");
                WriteTrainingAndTestResults(GetTopPunchedCardsPerLabel(punchedCardsPerKeyPerLabel, experts, 64), trainingData, testData, puncher, experts);

                Console.WriteLine();
            }

            Console.WriteLine("Press \"Enter\" to exit the program...");
            Console.ReadLine();
        }

        private static void WriteTrainingAndTestResults(
            IReadOnlyDictionary<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> topPunchedCardsPerLabel,
            List<Tuple<IBitVector, IBitVector>> trainingData,
            List<Tuple<IBitVector, IBitVector>> testData,
            IPuncher<string, IBitVector, IBitVector> puncher,
            IReadOnlyDictionary<string, IExpert> experts)
        {
            Console.WriteLine("Unique input combinations per punched card (descending): " +
                              GetPunchedCardsPerLabelString(topPunchedCardsPerLabel));

            var trainingCorrectRecognitionsPerLabel =
                RecognitionHelper.CountCorrectRecognitions(trainingData, topPunchedCardsPerLabel, puncher, experts, BitVectorFactory);
            Console.WriteLine("Training results: " +
                              trainingCorrectRecognitionsPerLabel
                                  .Sum(correctRecognitionsPerLabel => correctRecognitionsPerLabel.Value) +
                              " correct recognitions of " + trainingData.Count);

            var testCorrectRecognitionsPerLabel =
                RecognitionHelper.CountCorrectRecognitions(testData, topPunchedCardsPerLabel, puncher, experts, BitVectorFactory);
            Console.WriteLine("Test results: " +
                              testCorrectRecognitionsPerLabel
                                  .Sum(correctRecognitionsPerLabel => correctRecognitionsPerLabel.Value) +
                              " correct recognitions of " + testData.Count);
        }

        private static string GetPunchedCardsPerLabelString(
            IReadOnlyDictionary<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> punchedCardsPerLabel)
        {
            var punchedCardsPerLabelUniqueLookupCounts = punchedCardsPerLabel
                .Select(punchedCardPerLabel =>
                    Tuple.Create(
                        (IReadOnlyCollection<int>)punchedCardPerLabel.Value
                            .Select(punchedCard => punchedCard.Value.Distinct().Count())
                            .OrderByDescending(count => count)
                            .ToList(),
                        punchedCardPerLabel.Value.Sum(punchedCard => punchedCard.Value.Distinct().Count())))
                .OrderByDescending(countsAndSum => countsAndSum.Item2)
                .ToList();
            return string.Join(", ",
                       punchedCardsPerLabelUniqueLookupCounts.Select(uniqueLookupCounts =>
                           $"{{{GetUniqueLookupsCountsString(uniqueLookupCounts)}}}")) + ": total sum " +
                   punchedCardsPerLabelUniqueLookupCounts.Sum(uniqueLookupCounts => uniqueLookupCounts.Item2);
        }

        private static string GetUniqueLookupsCountsString(Tuple<IReadOnlyCollection<int>, int> uniqueLookupCounts)
        {
            var valuesString = string.Join(", ", uniqueLookupCounts.Item1);
            return uniqueLookupCounts.Item1.Count <= 1
                ? valuesString
                : valuesString + ": sum " + uniqueLookupCounts.Item2;
        }

        private static IReadOnlyDictionary<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> GetGlobalTopPunchedCard(
            IReadOnlyDictionary<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> punchedCardsPerKeyPerLabel,
            IReadOnlyDictionary<string, IExpert> experts)
        {
            var globalTopPunchedCard = punchedCardsPerKeyPerLabel
                .AsParallel()
                .Select(punchedCardsPerLabel =>
                    Tuple.Create(punchedCardsPerLabel, RecognitionHelper.CalculateMaxLossSum(punchedCardsPerLabel, experts)))
                .MaxBy(tuple => tuple.Item2).Item1;

            return new Dictionary<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>>
                {{globalTopPunchedCard.Key, globalTopPunchedCard.Value}};
        }

        private static IReadOnlyDictionary<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>>
            GetTopPunchedCardsPerLabel(
                IReadOnlyDictionary<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> punchedCardsPerKeyPerLabel,
                IReadOnlyDictionary<string, IExpert> experts,
                int topPunchedCardsPerKeyPerLabelCount)
        {
            var topPunchedCardsPerKeyPerLabel =
                new Dictionary<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>>();

            foreach (var label in DataHelper.GetLabels(BitVectorFactory))
            {
                var topPunchedCardsPerSpecificLabel = punchedCardsPerKeyPerLabel
                    .AsParallel()
                    .Select(punchedCardsPerLabel =>
                        Tuple.Create(punchedCardsPerLabel, RecognitionHelper.CalculateMaxLoss(punchedCardsPerLabel, experts, label)))
                    .OrderByDescending(tuple => tuple.Item2)
                    .Take(topPunchedCardsPerKeyPerLabelCount)
                    .Select(tuple => tuple.Item1);

                foreach (var topPunchedCardPerSpecificLabel in topPunchedCardsPerSpecificLabel)
                {
                    if (!topPunchedCardsPerKeyPerLabel.TryGetValue(topPunchedCardPerSpecificLabel.Key, out var dictionary))
                    {
                        dictionary = new Dictionary<IBitVector, IReadOnlyCollection<IBitVector>>();
                        topPunchedCardsPerKeyPerLabel.Add(topPunchedCardPerSpecificLabel.Key, dictionary);
                    }

                    ((Dictionary<IBitVector, IReadOnlyCollection<IBitVector>>)dictionary).Add(label, topPunchedCardPerSpecificLabel.Value[label]);
                }
            }

            return topPunchedCardsPerKeyPerLabel;
        }

        private static IReadOnlyDictionary<string,
                IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>>
            GetPunchedCardsPerKeyPerLabel(
                IReadOnlyList<Tuple<IBitVector, IBitVector>> trainingData,
                IPuncher<string, IBitVector, IBitVector> puncher)
        {
            var count = trainingData[0].Item1.Count;
            return puncher
                .GetKeys(count)
                .AsParallel()
                .Select(key => Tuple.Create(
                            key,
                            GetPunchedCardsPerLabel(trainingData.Select(trainingDataItem =>
                                    Tuple.Create(puncher.Punch(key, trainingDataItem.Item1), trainingDataItem.Item2)))))
                .ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
        }

        private static IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>> GetPunchedCardsPerLabel(
            IEnumerable<Tuple<IPunchedCard<string, IBitVector>, IBitVector>> punchedCardInputsByKeyGrouping)
        {
            return punchedCardInputsByKeyGrouping
                .GroupBy(punchedCardAndLabel => punchedCardAndLabel.Item2)
                .ToDictionary(
                    punchedCardByLabelGrouping => punchedCardByLabelGrouping.Key,
                    punchedCardByLabelGrouping =>
                        (IReadOnlyCollection<IBitVector>) punchedCardByLabelGrouping
                            .Select(punchedCardAndLabel => punchedCardAndLabel.Item1.Input)
                        .ToList());
        }
    }
}