namespace MachineLearning.LatentSemanticAnalysis.IO
{
    using System;
    using System.Collections.Generic;

    public interface ISparceMatrixWriter
    {
        int RowsCount { get; }

        int ColumnsCount { get; }

        long ElementsCount { get; }

        void Write<T>(IEnumerable<SparceVector<T>> rows)
            where T : struct, IEquatable<T>, IFormattable;
    }
}