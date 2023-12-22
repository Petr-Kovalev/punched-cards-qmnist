using PunchedCards.BitVectors;
using PunchedCards.Helpers;

namespace PunchedCards
{
    internal static class PunchedCards
    {
        private static void Main()
        {
            var trainingData = DataHelper.LoadTrainingData();
            var testData = DataHelper.LoadTestData();

            var punchedCardBitLengths = new uint[] {8, 16, 32, 64, 128, 256};

            foreach (var punchedCardBitLength in punchedCardBitLengths)
            {
                Console.WriteLine("Punched card bit length: " + punchedCardBitLength);

                IPuncher<string, IBitVector, IBitVector> puncher = new RandomPuncher(punchedCardBitLength);
                var expertsPerKey = LoadExpertsPerKey(punchedCardBitLength, trainingData, puncher);

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

        private static void FineTune(
            IReadOnlyCollection<ValueTuple<IBitVector, IBitVector>> trainingData,
            IPuncher<string, IBitVector, IBitVector> puncher,
            IReadOnlyDictionary<string, IExpert> expertsPerKey,
            uint maxFineTuneIterationsCount)
        {
            Console.WriteLine();
            Console.Write("Average single-shot correct recognitions on fine-tune iteration: ");

            double oldAverage;
            var newAverage = double.MinValue;

            uint fineTuneIterationIndex = 0;
            do
            {
                oldAverage = newAverage;

                var correctRecognitionCounts = new List<int>();
                expertsPerKey.AsParallel().ForAll(expertPerKey =>
                {
                    var correctRecognitionCounter = 0;
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
            IReadOnlyCollection<ValueTuple<IBitVector, IBitVector>> trainingData,
            IReadOnlyCollection<ValueTuple<IBitVector, IBitVector>> testData,
            IReadOnlyDictionary<string, IExpert> expertsPerKey,
            IPuncher<string, IBitVector, IBitVector> puncher,
            int? topPunchedCardsCount = null)
        {
            var trainingCorrectRecognitionsPerLabel =
                RecognitionHelper.CountCorrectRecognitions(trainingData, puncher, expertsPerKey, topPunchedCardsCount);
            Console.WriteLine("Training results: " +
                              trainingCorrectRecognitionsPerLabel
                                  .Sum(correctRecognitionsPerLabel => correctRecognitionsPerLabel.Value) +
                              " correct recognitions of " + trainingData.Count);

            var testCorrectRecognitionsPerLabel =
                RecognitionHelper.CountCorrectRecognitions(testData, puncher, expertsPerKey, topPunchedCardsCount);
            Console.WriteLine("Test results: " +
                              testCorrectRecognitionsPerLabel
                                  .Sum(correctRecognitionsPerLabel => correctRecognitionsPerLabel.Value) +
                              " correct recognitions of " + testData.Count);
        }

        private static IReadOnlyDictionary<string,
                IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>>>
            GetPunchedCardsPerKeyPerLabel(
                IReadOnlyList<ValueTuple<IBitVector, IBitVector>> trainingData,
                IPuncher<string, IBitVector, IBitVector> puncher)
        {
            var count = trainingData[0].Item1.Count;

            return puncher
                .GetKeys(count)
                .AsParallel()
                .Select(key => ValueTuple.Create(
                            key,
                            GetPunchedCardsPerLabel(trainingData.Select(trainingDataItem =>
                                    ValueTuple.Create(puncher.Punch(key, trainingDataItem.Item1), trainingDataItem.Item2)))))
                .ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
        }

        private static IReadOnlyDictionary<IBitVector, IReadOnlyCollection<IBitVector>> GetPunchedCardsPerLabel(
            IEnumerable<ValueTuple<IPunchedCard<string, IBitVector>, IBitVector>> punchedCardInputsByKeyGrouping)
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

        private static IReadOnlyDictionary<string, IExpert> LoadExpertsPerKey(uint punchedCardBitLength, IReadOnlyList<ValueTuple<IBitVector, IBitVector>> trainingData, IPuncher<string, IBitVector, IBitVector> puncher)
        {
            var fileName = $"Experts{punchedCardBitLength}.json";

            if (File.Exists(fileName))
            {
                // Initialize puncher's map
                puncher.GetKeys(trainingData[0].Item1.Count);

                using var stream = File.OpenRead(fileName);
                return JsonSerializer.Deserialize<IReadOnlyDictionary<string, IExpert>>(stream);
            }
            else
            {
                var expertsPerKey = RecognitionHelper.CreateExperts(GetPunchedCardsPerKeyPerLabel(trainingData, puncher));
                FineTune(trainingData, puncher, expertsPerKey, 500);

                using var stream = File.Create(fileName);
                JsonSerializer.Serialize(expertsPerKey, stream);

                return expertsPerKey;
            }
        }
    }
}