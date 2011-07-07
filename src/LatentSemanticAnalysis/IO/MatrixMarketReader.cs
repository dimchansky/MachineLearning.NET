namespace MachineLearning.LatentSemanticAnalysis.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class MatrixMarketReader : IDisposable, ISparceMatrixReader
    {
        #region Fields and Properties

        private StreamReader streamReader;

        #endregion

        #region Constructors

        public MatrixMarketReader(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (!stream.CanRead)
            {
                throw new NotSupportedException("Stream does not support reading.");
            }

            this.streamReader = new StreamReader(stream);
        }

        #endregion

        #region Implementation of ISparceMatrixReader

        public long RowsCount { get; private set; }

        public long ColumnsCount { get; private set; }

        public long ElementsCount { get; private set; }

        public IEnumerable<SparceVector<T>> ReadRows<T>() where T : struct, IEquatable<T>
        {
            if (this.streamReader == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            
            throw new NotImplementedException();
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            if (this.streamReader == null)
            {
                return;
            }

            try
            {
                this.streamReader.Dispose();
            }
            finally
            {
                this.streamReader = null;
            }
        }

        #endregion
    }
}
