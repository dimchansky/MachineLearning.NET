namespace MachineLearning.Collections.IO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MachineLearning.Collections.Array;

    public sealed class CachedMatrixMarketReader<T> : ISparseMatrixReader<T>, IDisposable
        where T : struct, IEquatable<T>
    {
        #region Fields and Properties

        private readonly ISparseMatrixReader<T> reader;

        private readonly int rowsCount;

        private readonly int columnsCount;

        private readonly long elementsCount;

        #endregion

        #region Constructors

        public CachedMatrixMarketReader(ISparseMatrixReader<T> reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            this.reader = reader;
            this.rowsCount = reader.RowsCount;
            this.columnsCount = reader.ColumnsCount;
            this.elementsCount = reader.ElementsCount;
        }

        #endregion

        #region Implementation of ISparseMatrixReader<T>

        public int RowsCount
        {
            get
            {
                return this.rowsCount;
            }
        }

        public int ColumnsCount
        {
            get
            {
                return this.columnsCount;
            }
        }

        public long ElementsCount
        {
            get
            {
                return this.elementsCount;
            }
        }

        public IEnumerable<SparseVector<T>> ReadRows()
        {
            return Enumerable.Empty<SparseVector<T>>();
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
        }

        #endregion
    }
}
