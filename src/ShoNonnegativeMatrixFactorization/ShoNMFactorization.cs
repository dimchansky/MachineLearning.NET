namespace ShoNonnegativeMatrixFactorization
{
    using System;

    using ShoNS.Array;

    public sealed class ShoNMFactorization : IDisposable
    {
        private readonly DoubleArray w;

        private readonly DoubleArray h;

        /// <summary>
        /// Gets the weights matrix.
        /// </summary>
        /// <value>The weights matrix.</value>
        public DoubleArray W
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
        public DoubleArray H
        {
            get
            {
                return this.h;
            }
        }

        public ShoNMFactorization(DoubleArray w, DoubleArray h)
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

        public void Dispose()
        {
            w.Dispose();
            h.Dispose();
        }
    }
}