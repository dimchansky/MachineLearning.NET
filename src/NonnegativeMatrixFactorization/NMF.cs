namespace MachineLearning.NonnegativeMatrixFactorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
                        var wT = w.Transpose();

                        // hn = w.T * A
                        var aRow = 0;
                        foreach (var sparseVector in sparseMatrixReader.ReadRows<double>())
                        {
                            // multiply column (aRow) from w.T by row (aRow) from A (sparse vector)
                            for (int i = 0; i < maxFeaturesCount; i++)
                            {
                                var multiplicationFactor = wT[i, aRow];
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
                                var v = GetMultiplicationElement(wT, w, i, j);
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

                        // update h = h .* hn ./ hd
                        for (int i = 0; i < maxFeaturesCount; i++)
                        {
                            for (int j = 0; j < cc; j++)
                            {
                                h[i, j] = h[i, j] * hn[i, j] / hd[i, j];
                            }
                        }
                    }

                    // Update weights matrix
                    using (var wn = new MemoryMappedArray<double>(rc, maxFeaturesCount))
                    using (var hhT = new MemoryMappedArray<double>(maxFeaturesCount, maxFeaturesCount))
                    using (var wd = new MemoryMappedArray<double>(rc, maxFeaturesCount))
                    {
                        // h.T
                        var hT = h.Transpose();

                        // wn = A * h.T
                        var aRow = 0;
                        foreach (var sparseVector in sparseMatrixReader.ReadRows<double>())
                        {                                                                                   
                            for (int hTColumn = 0; hTColumn < maxFeaturesCount; hTColumn++)
                            {
                                wn[aRow, hTColumn] = sparseVector.Sum(pair => pair.Value * hT[pair.Key, hTColumn]);
                            }                            
                            ++aRow;
                        }

                        // hhT = h * h.T - symmetric array
                        for (int i = 0; i < maxFeaturesCount; i++)
                        {
                            // optimization: compute right upper half array only
                            for (int j = i; j < maxFeaturesCount; j++)
                            {
                                // compute hhT[i,j] as dot product of row i in h by column j in hT
                                var v = GetMultiplicationElement(h, hT, i, j);
                                hhT[i, j] = v;

                                // optimization: copy result to hhT[j,i] (left bottom)
                                if (i != j)
                                {
                                    hhT[j, i] = v;
                                }
                            }
                        }

                        // wd = w * (h * h.T)
                        for (int i = 0; i < rc; i++)
                        {
                            for (int j = 0; j < maxFeaturesCount; j++)
                            {
                                wd[i, j] = GetMultiplicationElement(w, hhT, i, j);
                            }
                        }

                        // update w = w .* wn ./ wd
                        for (int i = 0; i < rc; i++)
                        {
                            for (int j = 0; j < maxFeaturesCount; j++)
                            {
                                w[i, j] = w[i, j] * wn[i, j] / wd[i, j];
                            }
                        }
                    }
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
