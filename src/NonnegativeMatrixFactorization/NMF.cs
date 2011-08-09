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

        public NMFactorization Factorize(int maxFeaturesCount = 10, int maxIterations = 20)
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
                    using (var hn = new MemoryMappedArray<double>(maxFeaturesCount, cc))
                    using (var wTw = new MemoryMappedArray<double>(maxFeaturesCount, maxFeaturesCount))
                    using (var hd = new MemoryMappedArray<double>(maxFeaturesCount, cc))
                    {
                        // w.T
                        var wt = w.Transpose();

                        // hn = w.T * A
                        var aRow = 0;
                        foreach (var sparseVector in sparseMatrixReader.ReadRows<double>())
                        {
                            // multiply column (aRow) from w.T by row (aRow) from A (sparse vector)
                            for (int i = 0; i < maxFeaturesCount; i++)
                            {
                                var multiplicationFactor = wt[i, aRow];
                                foreach (var pair in sparseVector)
                                {
                                    hn[i, pair.Key] += multiplicationFactor * pair.Value;
                                }
                            }
                            ++aRow;
                        }

                        // wTw = w.T * w - is symmetric array
                        for (int i = 0; i < maxFeaturesCount; i++)
                        {
                            // optimization: compute right upper half array only
                            for (int j = i; j < maxFeaturesCount; j++)
                            {
                                // compute wTw[i,j] as dot product of row i in wT by column j in w
                                var v = GetMultiplicationElement(wt, w, i, j);
                                wTw[i, j] = v;

                                // optimization: copy result to wTw[j,i] (left bottom)
                                if (i != j)
                                {
                                    wTw[j, i] = v;
                                }
                            }
                        }

                        // hd = (w.T * w) * h
                        for (int i = 0; i < maxFeaturesCount; i++)
                        {
                            for (int j = 0; j < cc; j++)
                            {
                                hd[i, j] = GetMultiplicationElement(wTw, h, i, j);
                            }
                        }
                    }

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

        private static double GetMultiplicationElement(IArray<double> a, IArray<double> b, int row, int column)
        {
            var n = a.Size1;

            if(n != b.Size0)
            {
                throw new ArgumentException("Cannot multiply arrays.");
            }

            var result = 0.0;
            for (int i = 0; i < n; i++)
            {
                result += a[row,i] * b[i, column];
            }

            return result;
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
