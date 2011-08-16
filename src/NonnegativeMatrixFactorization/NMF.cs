namespace MachineLearning.NonnegativeMatrixFactorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using MachineLearning.Collections.Array;
    using MachineLearning.Collections.IO;

    public class NMF
    {
        private readonly ISparseMatrixReader<double> sparseMatrixReader;

        public NMF(ISparseMatrixReader<double> sparseMatrixReader)
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

            var w = new MemoryMappedArray<double>(rc, maxFeaturesCount);
            var h = new MemoryMappedArray<double>(maxFeaturesCount, cc);

            double euclideanDistance = 0.0;

            try
            {
                // Initialize the weight and feature matrices with random values
                FillRandom(w);
                FillRandom(h);

                // Perform operation a maximum of maxIterations times
                for (int iteration = 0; iteration < maxIterations; iteration++)
                {
                    // Calculate current difference cost
                    euclideanDistance = EuclideanDistance(this.sparseMatrixReader, w, h);

                    // Terminate if the matrix has been fully factorized
                    if (euclideanDistance <= double.Epsilon)
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
                        foreach (var rowIdxSparseVector in GetNumberedRows(sparseMatrixReader.ReadRows()))
                        {
                            // multiply column (aRow) from w.T by row (aRow) from A (sparse vector)
                            for (int i = 0; i < maxFeaturesCount; i++)
                            {
                                var multiplicationFactor = wT[i, rowIdxSparseVector.Key];
                                foreach (var pair in rowIdxSparseVector.Value)
                                {
                                    hn[i, pair.Key] += multiplicationFactor * pair.Value;
                                }
                            }
                        }

                        // wTw = w.T * w - is symmetric array
                        Parallel.For(0, maxFeaturesCount, i =>
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
                            });

                        // hd = (w.T * w) * h
                        Parallel.For(0, maxFeaturesCount, i =>
                                {
                                    for (int j = 0; j < cc; j++)
                                    {
                                        hd[i, j] = GetMultiplicationElement(wTw, h, i, j);
                                    }
                                });

                        // update h = h .* hn ./ hd
                        Parallel.For(0, maxFeaturesCount, i =>
                            {
                                for (int j = 0; j < cc; j++)
                                {
                                    var nom = hn[i, j];
                                    var den = hd[i, j];
                                    if (nom != den)
                                    {
                                        h[i, j] = h[i, j] * nom / den;
                                    }
                                }                                
                            });
                    }

                    // Update weights matrix
                    using (var wn = new MemoryMappedArray<double>(rc, maxFeaturesCount))
                    using (var hhT = new MemoryMappedArray<double>(maxFeaturesCount, maxFeaturesCount))
                    using (var wd = new MemoryMappedArray<double>(rc, maxFeaturesCount))
                    {
                        // h.T
                        var hT = h.Transpose();

                        // wn = A * h.T
                        foreach (var rowIdxSparseVector in GetNumberedRows(sparseMatrixReader.ReadRows()))
                        {
                            for (int hTColumn = 0; hTColumn < maxFeaturesCount; hTColumn++)
                            {
                                wn[rowIdxSparseVector.Key, hTColumn] = rowIdxSparseVector.Value.Sum(pair => pair.Value * hT[pair.Key, hTColumn]);
                            } 
                        }

                        // hhT = h * h.T - symmetric array
                        Parallel.For(0, maxFeaturesCount, i =>
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
                            });

                        // wd = w * (h * h.T)
                        Parallel.For(0, rc, i =>
                            {
                                for (int j = 0; j < maxFeaturesCount; j++)
                                {
                                    wd[i, j] = GetMultiplicationElement(w, hhT, i, j);
                                }
                            });

                        // update w = w .* wn ./ wd
                        Parallel.For(0, rc, i =>
                            {
                                for (int j = 0; j < maxFeaturesCount; j++)
                                {
                                    var nom = wn[i, j];
                                    var den = wd[i, j];
                                    if (nom != den)
                                    {
                                        w[i, j] = w[i, j] * nom / den;
                                    }
                                }                                
                            });
                    }
                }
            }
            catch (Exception)
            {
                w.Dispose();
                h.Dispose();

                throw;
            }

            return new NMFactorization(w, h, euclideanDistance);
        }

        private static double EuclideanDistance(ISparseMatrixReader<double> matrixReader, IArray<double> w, IArray<double> h)
        {
            var cc = h.Size1;

            return
                GetNumberedRows(matrixReader.ReadRows()).AsParallel()
                .Select(rowIdxSparseVector =>
                            ToDenseVector(rowIdxSparseVector.Value, cc)
                                .Select(element => element.Value - GetMultiplicationElement(w, h, rowIdxSparseVector.Key, element.Key))
                                .Select(diff => diff * diff)
                                .Sum())
                .Sum();
        }

        #region Helpers

        private static IEnumerable<KeyValuePair<int, SparseVector<double>>> GetNumberedRows(IEnumerable<SparseVector<double>> rows)
        {
            var idx = 0;
            foreach (var row in rows)
            {
                yield return new KeyValuePair<int, SparseVector<double>>(idx, row);
                idx++;
            }
        }

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

        private static IEnumerable<KeyValuePair<int, T>> ToDenseVector<T>(IEnumerable<KeyValuePair<int, T>> sparseVector, int denseVectorLength) 
            where T : struct, IEquatable<T>
        {
            var i = 0;
            foreach (var pair in sparseVector)
            {
                while (i < pair.Key && i < denseVectorLength)
                {
                    yield return new KeyValuePair<int, T>(i, default(T));
                    i++;
                }

                if (i < denseVectorLength)
                {
                    yield return new KeyValuePair<int, T>(i, pair.Value);
                    i++;
                }
            }

            // the rest
            while (i < denseVectorLength)
            {
                yield return new KeyValuePair<int, T>(i, default(T));
                i++;
            }
        }

        private static void FillRandom(IArray<double> array)
        {
            var random = new Random();

            Parallel.For(0, array.Size0, i =>
                {
                    for (int j = 0; j < array.Size1; j++)
                    {
                        double nextDouble;
                        lock(random)
                        {
                            nextDouble = random.NextDouble();
                        }
                        array[i, j] = nextDouble;
                    }
                });
        }

        #endregion
    }
}
