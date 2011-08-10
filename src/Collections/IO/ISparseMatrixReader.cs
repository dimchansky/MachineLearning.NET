namespace MachineLearning.Collections.IO
{
    using System;
    using System.Collections.Generic;

    using MachineLearning.Collections.Array;

    public interface ISparseMatrixReader<T>
        where T : struct, IEquatable<T>
    {
        int RowsCount { get; }

        int ColumnsCount { get; }

        long ElementsCount { get; }

        IEnumerable<SparseVector<T>> ReadRows();
    }
}
