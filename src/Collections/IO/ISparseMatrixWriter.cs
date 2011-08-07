namespace MachineLearning.LatentSemanticAnalysis.IO
{
    using System;
    using System.Collections.Generic;

    using MachineLearning.Collections;

    public interface ISparseMatrixWriter
    {
        int RowsCount { get; }

        int ColumnsCount { get; }

        long ElementsCount { get; }

        void Write<T>(IEnumerable<SparseVector<T>> rows)
            where T : struct, IEquatable<T>, IFormattable;
    }
}