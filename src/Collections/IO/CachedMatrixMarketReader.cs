namespace MachineLearning.Collections.IO
{
    using System;
    using System.Collections.Generic;

    using MachineLearning.Collections.Array;

    public class CachedMatrixMarketReader<T> : ISparseMatrixReader<T>, IDisposable
        where T : struct, IEquatable<T>
    {
        #region Fields and Properties

        private readonly ISparseMatrixReader<T> reader;

        #endregion

        #region Constructors

        public CachedMatrixMarketReader(ISparseMatrixReader<T> reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            this.reader = reader;
        }

        #endregion

        #region Implementation of ISparseMatrixReader<T>

        public int RowsCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int ColumnsCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public long ElementsCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<SparseVector<T>> ReadRows()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
