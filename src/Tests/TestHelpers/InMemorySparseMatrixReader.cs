namespace TestHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MachineLearning.Collections.Array;
    using MachineLearning.Collections.IO;

    public class InMemorySparseMatrixReader : ISparseMatrixReader<double>
    {
        private readonly SparseVector<double>[] rows;

        private readonly int columnsCount;

        private readonly int elementsCount;

        public InMemorySparseMatrixReader(params SparseVector<double>[] rows)
        {
            if (rows == null)
            {
                throw new ArgumentNullException("rows");
            }
            this.rows = rows;

            this.columnsCount = (from r in rows from e in r select e.Key).Max() + 1;
            this.elementsCount = Enumerable.Sum((IEnumerable<int>)(from r in rows select r.NonZeroValuesCount));
        }

        #region Implementation of ISparseMatrixReader

        public int RowsCount
        {
            get
            {
                return this.rows.Length;
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

        public IEnumerable<SparseVector<double>> ReadRows() 
        {
            return this.rows;
        }

        #endregion
    }
}