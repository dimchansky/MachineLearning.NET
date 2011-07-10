using System;
namespace MachineLearning.LatentSemanticAnalysis.IO
{
    using System.Collections.Generic;

    interface ISparceMatrixReader
    {
        int RowsCount { get; }

        int ColumnsCount { get; }

        long ElementsCount { get; }

        IEnumerable<SparceVector<T>> ReadRows<T>()
            where T : struct, IEquatable<T>;
    }
}
