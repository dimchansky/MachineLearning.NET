using System;
namespace MachineLearning.LatentSemanticAnalysis.IO
{
    using System.Collections.Generic;

    interface ISparceMatrixReader
    {
        long RowsCount { get; }

        long ColumnsCount { get; }

        long ElementsCount { get; }

        IEnumerable<SparceVector<T>> ReadRows<T>()
            where T : struct, IEquatable<T>;
    }
}
