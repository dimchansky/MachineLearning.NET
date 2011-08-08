namespace MachineLearning.NonnegativeMatrixFactorization
{
    using System;

    using MachineLearning.Collections.Array;

    public sealed class NMFactorization : IDisposable
    {
        private readonly MemoryMappedArray<double> w;
        private readonly MemoryMappedArray<double> h;

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

        public NMFactorization(MemoryMappedArray<double> w, MemoryMappedArray<double> h)
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