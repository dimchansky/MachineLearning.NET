using System;
namespace MachineLearning.LatentSemanticAnalysis.IO
{
    using System.Collections.Generic;

    interface ISparseMatrixReader
    {
        int RowsCount { get; }

        int ColumnsCount { get; }

        long ElementsCount { get; }

        IEnumerable<SparseVector<T>> ReadRows<T>()
            where T : struct, IEquatable<T>;
    }
}
