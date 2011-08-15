namespace MachineLearning.Collections.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO.MemoryMappedFiles;
    using System.Linq;
    using System.Runtime.InteropServices;

    using MachineLearning.Collections.Array;

    public sealed class CachedMatrixMarketReader<T> : ISparseMatrixReader<T>, IDisposable
        where T : struct, IEquatable<T>
    {
        #region Fields and Properties

        private readonly ISparseMatrixReader<T> reader;

        private readonly int rowsCount;

        private readonly int columnsCount;

        private readonly long elementsCount;

        private bool fullyCached;

        private readonly int sizeOfEntryInBytes;

        private readonly MemoryMappedFile memoryMappedFile;

        private readonly MemoryMappedViewAccessor accessor;

        #endregion

        #region Constructors

        public CachedMatrixMarketReader(ISparseMatrixReader<T> reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            this.reader = reader;
            this.rowsCount = reader.RowsCount;
            this.columnsCount = reader.ColumnsCount;
            this.elementsCount = reader.ElementsCount;

            if (this.elementsCount > 0)
            {
                this.sizeOfEntryInBytes = sizeof(int) + sizeof(int) + Marshal.SizeOf(typeof(T));
                var bytes = (long)sizeOfEntryInBytes * this.elementsCount;
                this.memoryMappedFile = MemoryMappedFile.CreateNew(null, bytes);
                this.accessor = this.memoryMappedFile.CreateViewAccessor(0L, bytes);
            }
        }

        #endregion

        #region Implementation of ISparseMatrixReader<T>

        public int RowsCount
        {
            get
            {
                return this.rowsCount;
            }
        }

        public int ColumnsCount
        {
            get
            {
                return this.columnsCount;
            }
        }

        public long ElementsCount
        {
            get
            {
                return this.elementsCount;
            }
        }

        public IEnumerable<SparseVector<T>> ReadRows()
        {            
            if (fullyCached)
            {
                if (this.ElementsCount <= 0)
                {
                    return Enumerable.Empty<SparseVector<T>>();
                }

                return MatrixElement<T>.ToSparceVectorsFromOrderedMatrixElements(
                    this.RowsCount, 
                    this.ColumnsCount, 
                    this.ElementsCount, 
                    CachedMatrixElements(), 
                    0, 0);
            }
            
            return CacheAndYield();
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            if (this.accessor != null)
            {
                this.accessor.Dispose();
            }

            if (this.memoryMappedFile != null)
            {
                this.memoryMappedFile.Dispose();
            }
        }

        #endregion

        #region Helpers

        IEnumerable<SparseVector<T>> CacheAndYield()
        {
            var rowIdx = 0;
            var offset = 0L;

            foreach (var row in reader.ReadRows())
            {
                foreach (var pair in row)
                {
                    var value = pair.Value;
                    accessor.Write(offset, (int)rowIdx);
                    accessor.Write(offset + sizeof(int), (int)pair.Key);
                    accessor.Write(offset + sizeof(int) + sizeof(int), ref value);
                    offset += this.sizeOfEntryInBytes;
                }

                rowIdx++;
                yield return row;
            }

            fullyCached = true;            
        }

        private IEnumerable<MatrixElement<T>> CachedMatrixElements()
        {
            for (long entryIdx = 0L; entryIdx < this.elementsCount; entryIdx++)
            {
                var offset = entryIdx * this.sizeOfEntryInBytes;
                
                var row = accessor.ReadInt32(offset);
                var column = accessor.ReadInt32(offset + sizeof(int));
                T value;
                accessor.Read(offset + sizeof(int) + sizeof(int), out value);

                yield return new MatrixElement<T>(row, column, value);
            }
        }

        #endregion
    }
}
