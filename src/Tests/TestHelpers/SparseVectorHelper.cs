namespace TestHelpers
{
    using System;
    using System.Collections.Generic;

    using MachineLearning.Collections.Array;

    public static class SparseVectorHelper
    {
        public static IEnumerable<KeyValuePair<int, double>> GenerateRandomVector(int columns, double rowDensity, Func<double> randomGenerator)
        {
            if (rowDensity < 0 || rowDensity > 1)
            {
                throw new ArgumentOutOfRangeException("rowDensity");
            }

            var nonZeroElementsInRow = Poisson(rowDensity);
            var result = new Dictionary<int, double>();

            for (var j = 0; j < nonZeroElementsInRow; j++)
            {
                result[Random.Next(0, columns)] = randomGenerator();
            }

            return result;
        }

        public static SparseVector<double> GenerateSparseVector(int columns, double density, Func<double> randomGenerator)
        {
            if (density < 0 || density > 1)
            {
                throw new ArgumentOutOfRangeException("rowDensity");
            }

            var rowDensity = columns * density;
            var nonZeroElementsInRow = Poisson(rowDensity);
            var result = new SparseVector<double>();

            for (var j = 0; j < nonZeroElementsInRow; j++)
            {
                result[Random.Next(0, columns)] = randomGenerator();
            }

            return result;
        }

        public static IEnumerable<SparseVector<double>> GenerateSparseVectors(int rows, int columns, double density, Func<double> randomGenerator)
        {
            if (randomGenerator == null)
            {
                throw new ArgumentNullException("randomGenerator");
            }
            if (density < 0 || density > 1)
            {
                throw new ArgumentOutOfRangeException("density");
            }

            for (var i = 0; i < rows; i++)
            {
                yield return GenerateSparseVector(columns, density, randomGenerator);
            }
        }

        public static double RandomInInterval(double fromX, double toX, int digits)
        {
            return Math.Round(IntervalToInterval(Random.NextDouble(), 0, 1, fromX, toX), digits, MidpointRounding.AwayFromZero);
        }

        public static double UniformRandom()
        {
            return Random.NextDouble();
        }

        #region Helpers

        private static readonly Random Random = new Random();

        private static int Poisson(double mean)
        {
            if (mean < 0.0) throw new ArgumentOutOfRangeException("mean");
            if (mean == 0.0) return 0;
            double p = Math.Exp(-mean);
            double pm = 1.0;
            int result = 0;
            do
            {
                ++result;
                pm *= Random.NextDouble();
            }
            while (pm > p);
            return result - 1;
        }

        private static double IntervalToInterval(double x, double x1, double x2, double y1, double y2)
        {
            return (x - x1) / (x2 - x1) * (y2 - y1) + y1;
        }

        #endregion
    }
}
