﻿namespace MachineLearning.LatentSemanticAnalysis.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class MatrixMarketWriter : IDisposable, ISparceMatrixWriter
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

        #region Implementation of ISparceMatrixWriter

        public long RowsCount { get; private set; }

        public long ColumnsCount { get; private set; }

        public long ElementsCount { get; private set; }

        public void Write<T>(IEnumerable<SparceVector<T>> rows) where T : struct, IEquatable<T>
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
                sw.WriteLine("{0,60}", ' ');

                this.RowsCount = 0;
                this.ColumnsCount = 0;
                this.ElementsCount = 0;

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
                            sw.WriteLine("{0} {1} {2}", this.RowsCount, column, value);
                            this.ElementsCount++; // update elements count
                        }
                    }
                }

                // write matrix stats
                sw.Flush();
                sw.BaseStream.Seek(matrixStatsPosition, SeekOrigin.Begin);
                sw.Write("{0} {1} {2}", this.RowsCount, this.ColumnsCount, this.ElementsCount);
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
