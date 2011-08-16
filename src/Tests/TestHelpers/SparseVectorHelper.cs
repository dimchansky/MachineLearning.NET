namespace TestHelpers
{
    using System;
    using System.Collections.Generic;

    using MachineLearning.Collections.Array;

    public static class SparseVectorHelper
    {
        private static readonly Random Rnd = new Random();

        public static IDictionary<int, double> GenerateRandomVector(int count, double zeroProbability, bool onlyPositiveValues = false)
        {
            var dictionary = new Dictionary<int, double>(count);

            for (int i = 0; i < count; i++)
            {
                dictionary[i] = Rnd.NextDouble() < zeroProbability
                                ? 0.0
                                : Math.Round(
                                    IntervalToInterval(Rnd.NextDouble(), 0, 1, 0.01, 100) *
                                    (onlyPositiveValues || Rnd.NextDouble() < 0.5 ? 1 : -1),
                                    2,
                                    MidpointRounding.AwayFromZero);
            }

            return dictionary;
        }

        public static SparseVector<double> GenerateRandomSparseVector(int count, double zeroProbability, bool onlyPositiveValues = false)
        {
            var vector = new SparseVector<double>();

            for (int i = 0; i < count; i++)
            {
                vector[i] = Rnd.NextDouble() < zeroProbability
                                ? 0.0
                                : Math.Round(
                                    IntervalToInterval(Rnd.NextDouble(), 0, 1, 0.01, 100) *
                                    (onlyPositiveValues || Rnd.NextDouble() < 0.5 ? 1 : -1),
                                    2,
                                    MidpointRounding.AwayFromZero);
            }

            return vector;
        }

        private static double IntervalToInterval(double x, double x1, double x2, double y1, double y2)
        {
            return (x - x1) / (x2 - x1) * (y2 - y1) + y1;
        }

        public static IEnumerable<SparseVector<double>> GenerateSparceVectors(int rows, int columns, double zeroProbability, bool onlyPositiveValues = false)
        {
            for (int i = 0; i < rows; i++)
            {
                yield return GenerateRandomSparseVector(columns, zeroProbability, onlyPositiveValues);
            }
        }
    }
}
