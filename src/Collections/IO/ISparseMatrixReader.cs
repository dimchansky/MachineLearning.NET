namespace MachineLearning.Collections.IO
{
    using System;
    using System.Collections.Generic;

    using MachineLearning.Collections;

    interface ISparseMatrixReader
    {
        int RowsCount { get; }

        int ColumnsCount { get; }

        long ElementsCount { get; }

        IEnumerable<SparseVector<T>> ReadRows<T>()
            where T : struct, IEquatable<T>;
    }
}
