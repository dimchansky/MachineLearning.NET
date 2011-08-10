namespace MachineLearning.Collections.IO
{
    using System;
    using System.Collections.Generic;

    using MachineLearning.Collections.Array;

    public interface ISparseMatrixWriter<T>
        where T : struct, IEquatable<T>, IFormattable
    {
        int RowsCount { get; }

        int ColumnsCount { get; }

        long ElementsCount { get; }

        void Write(IEnumerable<SparseVector<T>> rows);
    }
}