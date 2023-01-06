using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PunchedCards.BitVectors;
using PunchedCards.Helpers.QMNIST;

namespace PunchedCards.Helpers
{
    internal static class DataHelper
    {
        private const int LabelCount = 10;

        internal static IReadOnlyList<Tuple<IBitVector, IBitVector>> ReadTrainingData(IBitVectorFactory bitVectorFactory)
        {
            const string fileName = "trainingData.json";

            if (!File.Exists(fileName))
            {
                var trainingData = ReaData(QmnistReader.ReadTrainingData, bitVectorFactory).ToList();
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

        internal static IReadOnlyList<Tuple<IBitVector, IBitVector>> ReadTestData(IBitVectorFactory bitVectorFactory)
        {
            const string fileName = "testData.json";

            if (!File.Exists(fileName))
            {
                var testData = ReaData(QmnistReader.ReadTestData, bitVectorFactory).ToList();
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

        internal static IEnumerable<IBitVector> GetLabels(IBitVectorFactory bitVectorFactory)
        {
            return Enumerable.Range(0, LabelCount).Select(labelIndex => GetLabelBitVector((byte) labelIndex, bitVectorFactory));
        }

        private static IEnumerable<Tuple<IBitVector, IBitVector>> ReaData(Func<IEnumerable<Image>> readImagesFunction, IBitVectorFactory bitVectorFactory)
        {
            return readImagesFunction()
                .Select(image => Tuple.Create(
                    GetValueBitVector(image.Data, bitVectorFactory),
                    GetLabelBitVector(image.Label, bitVectorFactory)));
        }

        private static IBitVector GetLabelBitVector(byte label, IBitVectorFactory bitVectorFactory)
        {
            return bitVectorFactory.Create(
                GetActiveBitIndices(label).Select(i => 7 - i),
                4);
        }

        private static IBitVector GetValueBitVector(byte[,] imageData, IBitVectorFactory bitVectorFactory)
        {
            const byte height = 28;
            const byte width = 28;
            const uint pixelRepresentationSizeInBits = 8;

            return bitVectorFactory.Create(
                GetActiveBitIndices(imageData, height, width, pixelRepresentationSizeInBits),
                height * width * pixelRepresentationSizeInBits);
        }

        private static IEnumerable<uint> GetActiveBitIndices(
            byte[,] imageData,
            uint height,
            uint width,
            uint pixelRepresentationSizeInBits)
        {
            for (byte rowIndex = 0; rowIndex < height; rowIndex++)
            {
                for (byte columnIndex = 0; columnIndex < width; columnIndex++)
                {
                    var startIndex = (rowIndex * width + columnIndex) * pixelRepresentationSizeInBits;
                    foreach (var activeBitIndex in GetActiveBitIndices(imageData[rowIndex, columnIndex]))
                    {
                        yield return startIndex + activeBitIndex;
                    }
                }
            }
        }

        private static IEnumerable<uint> GetActiveBitIndices(byte b)
        {
            if ((b & 128) != 0)
            {
                yield return 0;
            }

            if ((b & 64) != 0)
            {
                yield return 1;
            }

            if ((b & 32) != 0)
            {
                yield return 2;
            }

            if ((b & 16) != 0)
            {
                yield return 3;
            }

            if ((b & 8) != 0)
            {
                yield return 4;
            }

            if ((b & 4) != 0)
            {
                yield return 5;
            }

            if ((b & 2) != 0)
            {
                yield return 6;
            }

            if ((b & 1) != 0)
            {
                yield return 7;
            }
        }
    }
}