namespace MachineLearning.NonnegativeMatrixFactorization
{
    using System;

    using MachineLearning.Collections.Array;

    public sealed class NMFactorization : IDisposable
    {
        private readonly MemoryMappedArray<double> w;
        private readonly MemoryMappedArray<double> h;
        private readonly double euclideanDistance;

        /// <summary>
        /// Gets the weights matrix.
        /// </summary>
        /// <value>The weights matrix.</value>
        public IArray<double> W
        {
            get
            {
                return this.w;
            }
        }

        /// <summary>
        /// Gets the features matrix.
        /// </summary>
        /// <value>The features matrix.</value>
        public IArray<double> H
        {
            get
            {
                return this.h;
            }
        }

        /// <summary>
        /// Gets the Euclidean distance of factorization.
        /// </summary>
        /// <value>The Euclidean distance of factorization.</value>
        public double EuclideanDistance
        {
            get
            {
                return this.euclideanDistance;
            }
        }

        public NMFactorization(MemoryMappedArray<double> w, MemoryMappedArray<double> h, double euclideanDistance)
        {
            if (w == null)
            {
                throw new ArgumentNullException("w");
            }
            if (h == null)
            {
                throw new ArgumentNullException("h");
            }
            this.w = w;
            this.h = h;
            this.euclideanDistance = euclideanDistance;
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            w.Dispose();
            h.Dispose();
        }

        #endregion
    }
}