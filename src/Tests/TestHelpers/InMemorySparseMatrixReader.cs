namespace TestHelpers
{
    using System;
    using System.Collections.Generic;
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

            var cc = 0;
            var ec = 0;
            foreach (var row in rows)
            {
                ec += row.NonZeroValuesCount;
                foreach (var pair in row)
                {
                    if(pair.Key >= cc)
                    {
                        cc = pair.Key + 1;
                    }
                }
            }

            this.columnsCount = cc;
            this.elementsCount = ec;
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