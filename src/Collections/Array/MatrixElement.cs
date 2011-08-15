namespace MachineLearning.Collections.Array
{
    using System;
    using System.Collections.Generic;

    internal struct MatrixElement<T>
        where T : struct, IEquatable<T>
    {
        private readonly int row;
        private readonly int column;
        private readonly T value;

        internal int Row
        {
            get
            {
                return this.row;
            }
        }

        internal int Column
        {
            get
            {
                return this.column;
            }
        }

        internal T Value
        {
            get
            {
                return this.value;
            }
        }

        internal MatrixElement(int row, int column, T value)
        {
            this.row = row;
            this.column = column;
            this.value = value;
        }

        internal static IEnumerable<SparseVector<T>> ToSparceVectorsFromOrderedMatrixElements(
                                                        int rowsCount, int columnsCount, long elementsCount,
                                                        IEnumerable<MatrixElement<T>> parsedMatrixElements,
                                                        int rowIndexStartsFrom, int columnIndexStartsFrom)
        {
            var totalElementsCount = 0L;
            var currentRowIdx = -1L;
            SparseVector<T> currentRow = null;

            foreach (var parsedLine in parsedMatrixElements)
            {
                int rowIdx = parsedLine.Row - rowIndexStartsFrom;
                int columnIdx = parsedLine.Column - columnIndexStartsFrom;

                // check row
                if (rowIdx >= rowsCount)
                {
                    throw new FormatException("Bad total rows count.");
                }
                if (rowIdx < 0)
                {
                    throw new FormatException("Row index should be greater than 0.");
                }

                // check column
                if (columnIdx >= columnsCount)
                {
                    throw new FormatException("Bad total columns count.");
                }
                if (columnIdx < 0)
                {
                    throw new FormatException("Column index should be greater than 0.");
                }

                // check elements count
                if (++totalElementsCount > elementsCount)
                {
                    throw new FormatException("Bad total elements count.");
                }

                if (rowIdx != currentRowIdx)
                {
                    if (currentRowIdx >= 0)
                    {
                        // return last row
                        yield return currentRow;
                    }

                    // return all empty rows between last and current row
                    for (var i = currentRowIdx + 1L; i < rowIdx; i++)
                    {
                        yield return new SparseVector<T>();
                    }

                    // create new current row
                    currentRowIdx = rowIdx;
                    currentRow = new SparseVector<T>();
                }

                // ReSharper disable PossibleNullReferenceException
                currentRow[columnIdx] = parsedLine.Value;
                // ReSharper restore PossibleNullReferenceException
            }

            if (currentRowIdx >= 0)
            {
                // return last row
                yield return currentRow;
            }

            // return empty rows between last and row and the total rows
            for (long i = currentRowIdx + 1; i < rowsCount; i++)
            {
                yield return new SparseVector<T>();
            }
        }
    }
}