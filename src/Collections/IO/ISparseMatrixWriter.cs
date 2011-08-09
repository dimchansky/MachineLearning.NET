namespace MachineLearning.Collections.IO
{
    using System;
    using System.Collections.Generic;

    using MachineLearning.Collections;
    using MachineLearning.Collections.Array;

    public interface ISparseMatrixWriter
    {
        int RowsCount { get; }

        int ColumnsCount { get; }

        long ElementsCount { get; }

        void Write<T>(IEnumerable<SparseVector<T>> rows)
            where T : struct, IEquatable<T>, IFormattable;
    }
}