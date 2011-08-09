namespace MachineLearning.NonnegativeMatrixFactorization
{
    using System;
    using System.Collections.Generic;

    using MachineLearning.Collections.Array;
    using MachineLearning.Collections.IO;

    public class NMF
    {
        private readonly ISparseMatrixReader sparseMatrixReader;

        public NMF(ISparseMatrixReader sparseMatrixReader)
        {
            if (sparseMatrixReader == null)
            {
                throw new ArgumentNullException("sparseMatrixReader");
            }
            this.sparseMatrixReader = sparseMatrixReader;
        }

        public NMFactorization Factorize(int maxFeaturesCount = 10, int maxIterations = 50)
        {
            if (maxFeaturesCount <= 0)
            {
                throw new ArgumentOutOfRangeException("maxFeaturesCount", "Maximum features number must be positive integer.");
            }

            if (maxIterations <= 0)
            {
                throw new ArgumentOutOfRangeException("maxIterations", "Maximum number of iterations must be positive integer.");
            }

            var rc = sparseMatrixReader.RowsCount;
            var cc = sparseMatrixReader.ColumnsCount;
            maxFeaturesCount = Math.Min(maxFeaturesCount, Math.Min(rc, cc));

            var w = new MemoryMappedArray<double>(rc, maxFeaturesCount);
            var h = new MemoryMappedArray<double>(maxFeaturesCount, cc);

            try
            {
                // Initialize the weight and feature matrices with random values
                FillRandom(w);
                FillRandom(h);

                // Perform operation a maximum of maxIterations times
                for (int iteration = 0; iteration < maxIterations; iteration++)
                {
                    // Calculate current difference cost
                    var diffCost = 0.0;
                    var row = 0;
                    foreach (var sparseVector in sparseMatrixReader.ReadRows<double>())
                    {
                        var column = 0;

                        foreach (var element in ToDenseVector(sparseVector, cc))
                        {
                            var diff = element - GetMultiplicationElement(w, h, row, column++);
                            diffCost += diff * diff;
                        }

                        ++row;
                    }                   

                    // Terminate if the matrix has been fully factorized
                    if (diffCost <= double.Epsilon)
                    {
                        break;
                    }

                    // Update feature matrix

                    // Update weights matrix

                    // TODO: implement

                    throw new NotImplementedException();
                }
            }
            catch (Exception)
            {
                if (w != null)
                {
                    w.Dispose();
                }
                if (h != null)
                {
                    h.Dispose();
                }

                throw;
            }

            return new NMFactorization(w, h);
        }

        #region Helpers

        private static double GetMultiplicationElement(MemoryMappedArray<double> w, MemoryMappedArray<double> h, int row, int column)
        {
            throw new NotImplementedException();
        }

        static IEnumerable<T> ToDenseVector<T>(IEnumerable<KeyValuePair<int, T>> sparseVector, int denseVectorLength) 
            where T : struct, IEquatable<T>
        {
            var i = 0;
            foreach (var pair in sparseVector)
            {
                while (i < pair.Key && i < denseVectorLength)
                {
                    yield return default(T);
                    i++;
                }

                if (i < denseVectorLength)
                {
                    yield return pair.Value;
                    i++;
                }
            }

            // the rest
            while (i < denseVectorLength)
            {
                yield return default(T);
                i++;
            }
        }

        private static void FillRandom(IArray<double> array)
        {
            var random = new Random();

            for (int i = 0; i < array.Size0; i++)
            {
                for (int j = 0; j < array.Size1; j++)
                {
                    array[i, j] = random.NextDouble();
                }
            }
        }

        #endregion
    }
}
