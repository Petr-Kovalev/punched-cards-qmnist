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
                var punchedCardsPerLabel = GetPunchedCardsPerLabel(punchedCardsPerKeyPerLabel);
                var experts = RecognitionHelper.CreateExperts(punchedCardsPerKeyPerLabel);

                Console.WriteLine();
                Console.WriteLine("Top punched card per input:");
                WriteTrainingAndTestResults(punchedCardsPerLabel, trainingData, testData, puncher, experts, 1);

                Console.WriteLine();
                var count = (int)(experts.Count * 0.05);
                Console.WriteLine($"Top {count} (5%) punched cards per input:");
                WriteTrainingAndTestResults(punchedCardsPerLabel, trainingData, testData, puncher, experts, count);

                Console.WriteLine();
                Console.WriteLine("All punched cards:");
                WriteTrainingAndTestResults(punchedCardsPerLabel, trainingData, testData, puncher, experts);

                Console.WriteLine();
            }

            Console.WriteLine("Press \"Enter\" to exit the program...");
            Console.ReadLine();
        }

        private static void WriteTrainingAndTestResults(
            IReadOnlyDictionary<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> punchedCardsPerLabel,
            List<Tuple<IBitVector, IBitVector>> trainingData,
            List<Tuple<IBitVector, IBitVector>> testData,
            IPuncher<string, IBitVector, IBitVector> puncher,
            IReadOnlyDictionary<string, IExpert> experts,
            int? topPunchedCardsCount = null)
        {
            var trainingCorrectRecognitionsPerLabel =
                RecognitionHelper.CountCorrectRecognitions(trainingData, punchedCardsPerLabel, puncher, experts, BitVectorFactory, topPunchedCardsCount);
            Console.WriteLine("Training results: " +
                              trainingCorrectRecognitionsPerLabel
                                  .Sum(correctRecognitionsPerLabel => correctRecognitionsPerLabel.Value) +
                              " correct recognitions of " + trainingData.Count);

            var testCorrectRecognitionsPerLabel =
                RecognitionHelper.CountCorrectRecognitions(testData, punchedCardsPerLabel, puncher, experts, BitVectorFactory, topPunchedCardsCount);
            Console.WriteLine("Test results: " +
                              testCorrectRecognitionsPerLabel
                                  .Sum(correctRecognitionsPerLabel => correctRecognitionsPerLabel.Value) +
                              " correct recognitions of " + testData.Count);
        }

        private static IReadOnlyDictionary<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>>
            GetPunchedCardsPerLabel(IReadOnlyDictionary<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>> punchedCardsPerKeyPerLabel)
        {
            var topPunchedCardsPerKeyPerLabel =
                new Dictionary<string, IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>>();

            foreach (var label in DataHelper.GetLabels(BitVectorFactory))
            {
                foreach (var punchedCardPerSpecificLabel in punchedCardsPerKeyPerLabel)
                {
                    if (!topPunchedCardsPerKeyPerLabel.TryGetValue(punchedCardPerSpecificLabel.Key, out var dictionary))
                    {
                        dictionary = new Dictionary<IBitVector, IReadOnlyCollection<IBitVector>>();
                        topPunchedCardsPerKeyPerLabel.Add(punchedCardPerSpecificLabel.Key, dictionary);
                    }

                    ((Dictionary<IBitVector, IReadOnlyCollection<IBitVector>>)dictionary).Add(label, punchedCardPerSpecificLabel.Value[label]);
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