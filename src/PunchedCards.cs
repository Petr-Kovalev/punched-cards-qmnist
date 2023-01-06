﻿using System;
using System.Collections.Generic;
using System.IO;
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
            var trainingData = LoadTrainingData();
            var testData = LoadTestData();

            var punchedCardBitLengths = new uint[] {8, 16, 32, 64, 128, 256};

            foreach (var punchedCardBitLength in punchedCardBitLengths)
            {
                Console.WriteLine("Punched card bit length: " + punchedCardBitLength);

                IPuncher<string, IBitVector, IBitVector> puncher = new RandomPuncher(punchedCardBitLength, BitVectorFactory);
                var expertsPerKey = RecognitionHelper.CreateExperts(GetPunchedCardsPerKeyPerLabel(trainingData, puncher));

                FineTune(trainingData, puncher, expertsPerKey, 500);

                Console.WriteLine();
                Console.WriteLine("Top punched card per input:");
                WriteTrainingAndTestResults(trainingData, testData, expertsPerKey, puncher, 1);

                Console.WriteLine();
                var count = (int)(expertsPerKey.Count * 0.05);
                Console.WriteLine($"Top {count} (5%) punched cards per input:");
                WriteTrainingAndTestResults(trainingData, testData, expertsPerKey, puncher, count);

                Console.WriteLine();
                Console.WriteLine("All punched cards:");
                WriteTrainingAndTestResults(trainingData, testData, expertsPerKey, puncher);

                Console.WriteLine();
            }

            Console.WriteLine("Press \"Enter\" to exit the program...");
            Console.ReadLine();
        }

        private static IReadOnlyList<Tuple<IBitVector, IBitVector>> LoadTrainingData()
        {
            const string fileName = "trainingData.json";

            if (!File.Exists(fileName))
            {
                var trainingData = DataHelper.ReadTrainingData(BitVectorFactory).ToList();
                using (var stream = File.Create(fileName))
                {
                    JsonSerializer.Serialize(trainingData, stream);
                }
                return trainingData;
            }
            else
            {
                using (var stream = File.OpenRead(fileName))
                {
                    return JsonSerializer.Deserialize<IReadOnlyList<Tuple<IBitVector, IBitVector>>>(stream);
                }
            }
        }

        private static IReadOnlyList<Tuple<IBitVector, IBitVector>> LoadTestData()
        {
            const string fileName = "testData.json";

            if (!File.Exists(fileName))
            {
                var testData = DataHelper.ReadTestData(BitVectorFactory).ToList();
                using (var stream = File.Create(fileName))
                {
                    JsonSerializer.Serialize(testData, stream);
                }
                return testData;
            }
            else
            {
                using (var stream = File.OpenRead(fileName))
                {
                    return JsonSerializer.Deserialize<IReadOnlyList<Tuple<IBitVector, IBitVector>>>(stream);
                }
            }
        }

        private static void FineTune(
            IReadOnlyCollection<Tuple<IBitVector, IBitVector>> trainingData,
            IPuncher<string, IBitVector, IBitVector> puncher,
            IReadOnlyDictionary<string, IExpert> expertsPerKey,
            uint maxFineTuneIterationsCount)
        {
            Console.WriteLine();
            Console.Write($"Average single-shot correct recognitions on fine-tune iteration: ");

            double oldAverage;
            double newAverage = double.MinValue;

            uint fineTuneIterationIndex = 0;
            do
            {
                oldAverage = newAverage;

                var correctRecognitionCounts = new List<int>();
                expertsPerKey.AsParallel().ForAll(expertPerKey =>
                {
                    int correctRecognitionCounter = 0;
                    foreach (var trainingDataItem in trainingData)
                    {
                        if (!expertPerKey.Value.FineTune(puncher.Punch(expertPerKey.Key, trainingDataItem.Item1).Input, trainingDataItem.Item2))
                        {
                            correctRecognitionCounter++;
                        }
                    }
                    correctRecognitionCounts.Add(correctRecognitionCounter);
                });

                newAverage = correctRecognitionCounts.Average();

                if (fineTuneIterationIndex != 0)
                {
                    Console.Write(", ");
                }
                Console.Write((uint)newAverage);

                fineTuneIterationIndex++;
            } while (newAverage > oldAverage && fineTuneIterationIndex < maxFineTuneIterationsCount);

            Console.WriteLine();
        }

        private static void WriteTrainingAndTestResults(
            IReadOnlyCollection<Tuple<IBitVector, IBitVector>> trainingData,
            IReadOnlyCollection<Tuple<IBitVector, IBitVector>> testData,
            IReadOnlyDictionary<string, IExpert> expertsPerKey,
            IPuncher<string, IBitVector, IBitVector> puncher,
            int? topPunchedCardsCount = null)
        {
            var trainingCorrectRecognitionsPerLabel =
                RecognitionHelper.CountCorrectRecognitions(trainingData, puncher, expertsPerKey, BitVectorFactory, topPunchedCardsCount);
            Console.WriteLine("Training results: " +
                              trainingCorrectRecognitionsPerLabel
                                  .Sum(correctRecognitionsPerLabel => correctRecognitionsPerLabel.Value) +
                              " correct recognitions of " + trainingData.Count);

            var testCorrectRecognitionsPerLabel =
                RecognitionHelper.CountCorrectRecognitions(testData, puncher, expertsPerKey, BitVectorFactory, topPunchedCardsCount);
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