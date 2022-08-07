﻿using System;
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
                var trainingPunchedCardsPerKeyPerLabel = GetPunchedCardsPerKeyPerLabel(trainingData, puncher);
                var experts = RecognitionHelper.CreateExperts(trainingPunchedCardsPerKeyPerLabel);

                Console.WriteLine();
                Console.WriteLine("Top punched card per input:");
                WriteTrainingAndTestResults(trainingData, testData, experts, puncher, 1);

                Console.WriteLine();
                var count = (int)(experts.Count * 0.05);
                Console.WriteLine($"Top {count} (5%) punched cards per input:");
                WriteTrainingAndTestResults(trainingData, testData, experts, puncher, count);

                Console.WriteLine();
                Console.WriteLine("All punched cards:");
                WriteTrainingAndTestResults(trainingData, testData, experts, puncher);

                Console.WriteLine();
            }

            Console.WriteLine("Press \"Enter\" to exit the program...");
            Console.ReadLine();
        }

        private static void WriteTrainingAndTestResults(
            IReadOnlyCollection<Tuple<IBitVector, IBitVector>> trainingData,
            IReadOnlyCollection<Tuple<IBitVector, IBitVector>> testData,
            IReadOnlyDictionary<string, IExpert> experts,
            IPuncher<string, IBitVector, IBitVector> puncher,
            int? topPunchedCardsCount = null)
        {
            var trainingCorrectRecognitionsPerLabel =
                RecognitionHelper.CountCorrectRecognitions(trainingData, puncher, experts, BitVectorFactory, topPunchedCardsCount);
            Console.WriteLine("Training results: " +
                              trainingCorrectRecognitionsPerLabel
                                  .Sum(correctRecognitionsPerLabel => correctRecognitionsPerLabel.Value) +
                              " correct recognitions of " + trainingData.Count);

            var testCorrectRecognitionsPerLabel =
                RecognitionHelper.CountCorrectRecognitions(testData, puncher, experts, BitVectorFactory, topPunchedCardsCount);
            Console.WriteLine("Test results: " +
                              testCorrectRecognitionsPerLabel
                                  .Sum(correctRecognitionsPerLabel => correctRecognitionsPerLabel.Value) +
                              " correct recognitions of " + testData.Count);
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