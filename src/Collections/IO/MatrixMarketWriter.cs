namespace MachineLearning.Collections.IO
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    using MachineLearning.Collections;
    using MachineLearning.Collections.Array;

    public class MatrixMarketWriter : IDisposable, ISparseMatrixWriter
    {
        #region Fields and Properties

        private Stream stream;

        #endregion

        #region Constructors

        public MatrixMarketWriter(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (!stream.CanWrite)
            {
                throw new NotSupportedException("Stream does not support writing.");
            }
            if (!stream.CanSeek)
            {
                throw new NotSupportedException("Stream does not support seek operation.");
            }

            this.stream = stream;
        }

        #endregion

        #region Implementation of ISparseMatrixWriter

        public int RowsCount { get; private set; }

        public int ColumnsCount { get; private set; }

        public long ElementsCount { get; private set; }

        public void Write<T>(IEnumerable<SparseVector<T>> rows) where T : struct, IEquatable<T>, IFormattable
        {
            if (this.stream == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            using (var sw = new StreamWriter(this.stream))
            {
                // write header
                sw.WriteLine(MatrixMarketFormat.HeaderLine);
                sw.Flush();
                long matrixStatsPosition = sw.BaseStream.Position;
                // write empty space to be overwritten later                				
                sw.WriteLine(new string(' ', 60));

                this.RowsCount = 0;
                this.ColumnsCount = 0;
                this.ElementsCount = 0L;

                // write rows
                foreach (var row in rows)
                {
                    this.RowsCount++; // update rows count
                    foreach (var element in row)
                    {
                        int column = element.Key + 1; // +1 because Matrix Market format starts counting from 1

                        if (column > this.ColumnsCount)
                        {
                            this.ColumnsCount = column; // update columns count
                        }

                        T value = element.Value;
                        if (!value.Equals(default(T)))
                        {
                            sw.WriteLine("{0} {1} {2}", this.RowsCount.ToString(CultureInfo.InvariantCulture), column.ToString(CultureInfo.InvariantCulture), value.ToString("G", CultureInfo.InvariantCulture));
                            this.ElementsCount++; // update elements count
                        }
                    }
                }

                // write matrix stats
                sw.Flush();
                sw.BaseStream.Seek(matrixStatsPosition, SeekOrigin.Begin);
                sw.Write("{0} {1} {2}", this.RowsCount.ToString(CultureInfo.InvariantCulture), this.ColumnsCount.ToString(CultureInfo.InvariantCulture), this.ElementsCount.ToString(CultureInfo.InvariantCulture));
            }
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            if (this.stream == null)
            {
                return;
            }

            try
            {
                this.stream.Dispose();
            }
            finally
            {
                this.stream = null;
            }
        }

        #endregion
    }
}
