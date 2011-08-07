namespace MachineLearning.Collections.Array
{
    using System;
    using System.IO.MemoryMappedFiles;
    using System.Runtime.InteropServices;

    public class MemoryMappedArray<T> : IDisposable
        where T : struct
    {
        private readonly int size0;
        private readonly int size1;
        private readonly long count;
        private readonly int sizeOfT;

        private readonly MemoryMappedFile memoryMappedFile;

        private readonly MemoryMappedViewAccessor accessor;

        public int Size0
        {
            get
            {
                return this.size0;
            }
        }

        public int Size1
        {
            get
            {
                return this.size1;
            }
        }

        public T this[int index]
        {
            get
            {
                var offset = this.GetOffset(index);

                T result;
                accessor.Read(offset, out result);

                return result;
            }
            set
            {
                var offset = this.GetOffset(index);

                accessor.Write(offset, ref value);
            }
        }

        public T this[int rowIndex, int columnIndex]
        {
            get
            {
                var offset = this.GetRowColumnOffset(rowIndex, columnIndex);
                
                T result;
                accessor.Read(offset, out result);

                return result;
            }
            set
            {
                var offset = this.GetRowColumnOffset(rowIndex, columnIndex);

                accessor.Write(offset, ref value);
            }
        }

        public long Count
        {
            get
            {
                return this.count;
            }
        }

        public MemoryMappedArray(int length) : this(1, length)
        {
        }

        public MemoryMappedArray(int size0, int size1)
        {
            if (size0 <= 0)
            {
                throw new ArgumentOutOfRangeException("size0");
            }
            if (size1 <= 0)
            {
                throw new ArgumentOutOfRangeException("size1");
            }

            this.size0 = size0;
            this.size1 = size1;
            this.count = (long)size0 * size1;
            this.sizeOfT = Marshal.SizeOf(typeof(T));
            var bytes = count * this.sizeOfT;
            this.memoryMappedFile = MemoryMappedFile.CreateNew(null, bytes);
            this.accessor = this.memoryMappedFile.CreateViewAccessor(0L, bytes);
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            this.accessor.Dispose();            
            this.memoryMappedFile.Dispose();
        }

        #endregion

        #region Helpers

        private long GetRowColumnOffset(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || rowIndex >= this.size0)
            {
                throw new ArgumentOutOfRangeException(
                    "rowIndex", "Specified argument was out of the range of valid values.");
            }
            if (columnIndex < 0 || columnIndex >= this.size1)
            {
                throw new ArgumentOutOfRangeException(
                    "columnIndex", "Specified argument was out of the range of valid values.");
            }
            return ((long)rowIndex * size1 + columnIndex) * sizeOfT;
        }

        private long GetOffset(int index)
        {
            if (size0 > 1 && size1 > 1)
            {
                throw new InvalidOperationException("The number of slices/indices must equal the number of array dimensions.");
            }
            if (index < 0 || index >= count)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            return (long)index * sizeOfT;
        }

        #endregion
    }
}