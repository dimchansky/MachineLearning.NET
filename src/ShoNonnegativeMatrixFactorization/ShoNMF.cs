namespace ShoNonnegativeMatrixFactorization
{
    using System;
    using System.Threading.Tasks;

    using ShoNS.Array;

    public class ShoNMF
    {
        private readonly SparseDoubleArray sparseMatrix;

        public ShoNMF(SparseDoubleArray sparseMatrix)
        {
            if (sparseMatrix == null)
            {
                throw new ArgumentNullException("sparseMatrix");
            }
            this.sparseMatrix = sparseMatrix;
        }

        public ShoNMFactorization GetRandomFactorization(int featuresCount)
        {
            if (featuresCount <= 0)
            {
                throw new ArgumentOutOfRangeException("featuresCount", "Maximum features number must be positive integer.");
            }

            var rc = sparseMatrix.size0;
            var cc = sparseMatrix.size1;

            var w = new DoubleArray(rc, featuresCount);
            var h = new DoubleArray(featuresCount, cc);

            try
            {
                // Initialize the weight and feature matrices with random values
                var random = new Random();
                w.FillRandom(random);
                h.FillRandom(random);
            }
            catch (Exception)
            {
                w.Dispose();
                h.Dispose();

                throw;
            }

            return new ShoNMFactorization(w, h);
        }

        public void UpdateFactorization(DoubleArray w, DoubleArray h)
        {
            var rc = sparseMatrix.size0;
            if (rc != w.size0)
            {
                throw new ArgumentOutOfRangeException("w", "Weight matrix has invalid rows count.");
            }

            var cc = sparseMatrix.size1;
            if (cc != h.size1)
            {
                throw new ArgumentOutOfRangeException("h", "Feature matrix has invalid columns count.");
            }

            var featuresCount = w.size1;
            if (featuresCount != h.size0)
            {
                throw new ArgumentException("Weights and features matrix has different number of features.");
            }

            // Update feature matrix

            // hn = w.T * A
            using (var hn = w.T * this.sparseMatrix)
            // wTw = w.T * w
            using (var wTw = w.MultiplyTranspose(true))
            // hd = (w.T * w) * h
            using (var hd = wTw * h)
            {
                // update h = h .* hn ./ hd
                Parallel.For(0, featuresCount, i =>
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

            // wn = A * h.T
            using (var wn = this.sparseMatrix * h.T)
            // hhT = h * h.T
            using (var hhT = h.MultiplyTranspose(false))
            // wd = w * (h * h.T)
            using (var wd = w * hhT)
            {
                // update w = w .* wn ./ wd
                Parallel.For(0, rc, i =>
                {
                    for (int j = 0; j < featuresCount; j++)
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

        public ShoNMFactorization Factorize(int featuresCount = 10, int iterationsCount = 20)
        {
            if (featuresCount <= 0)
            {
                throw new ArgumentOutOfRangeException("featuresCount", "Maximum features number must be positive integer.");
            }

            if (iterationsCount <= 0)
            {
                throw new ArgumentOutOfRangeException("iterationsCount", "Maximum number of iterations must be positive integer.");
            }

            var factorization = this.GetRandomFactorization(featuresCount);

            try
            {
                for (var i = 0; i < iterationsCount; i++)
                {
                    this.UpdateFactorization(factorization.W, factorization.H);
                }
            }
            catch (Exception)
            {
                factorization.Dispose();

                throw;
            }

            return factorization;
        }
    }
}
