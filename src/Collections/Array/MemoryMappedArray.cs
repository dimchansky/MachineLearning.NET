namespace MachineLearning.Collections.Array
{
    using System;
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using System.Runtime.InteropServices;

    public class MemoryMappedArray<T> : IDisposable
        where T : struct
    {
        #region Properties and Fields

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

        #endregion

        #region Constructors

        public MemoryMappedArray(int length)
            : this(1, length)
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
            this.count = checked((long)size0 * size1);
            this.sizeOfT = Marshal.SizeOf(typeof(T));
            var bytes = checked(count * this.sizeOfT);
            this.memoryMappedFile = MemoryMappedFile.CreateNew(null, bytes);
            // create accessor for data
            this.accessor = this.memoryMappedFile.CreateViewAccessor(0L, bytes);
        }

        private MemoryMappedArray(string filename, int length) : this(filename, 1, length)
        {            
        }

        private MemoryMappedArray(string filename, int size0, int size1)
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
            this.count = checked((long)size0 * size1);
            this.sizeOfT = Marshal.SizeOf(typeof(T));
            var headerSizeInBytes = Marshal.SizeOf(typeof(Header));
            var fileSizeInBytes = checked(count * this.sizeOfT + headerSizeInBytes);
            // create mapped file
            this.memoryMappedFile = MemoryMappedFile.CreateFromFile(
                new FileStream(filename, FileMode.CreateNew), 
                null, 
                fileSizeInBytes, 
                MemoryMappedFileAccess.ReadWrite, 
                null, 
                HandleInheritability.None, 
                false);
            // write header
            using (var headerAccessor = this.memoryMappedFile.CreateViewAccessor(0L, headerSizeInBytes))
            {
                var header = new Header
                    {
                        TypeCode = Type.GetTypeCode(typeof(T)), 
                        Size0 = size0, 
                        Size1 = size1
                    };
                headerAccessor.Write(0L, ref header);
            }
            // create accessor for data
            this.accessor = this.memoryMappedFile.CreateViewAccessor(headerSizeInBytes, fileSizeInBytes - headerSizeInBytes);
        }

        private MemoryMappedArray(string filename)
        {
            var headerSizeInBytes = Marshal.SizeOf(typeof(Header));
            var fileSizeInBytes = new FileInfo(filename).Length;
            if (fileSizeInBytes < headerSizeInBytes)
            {
                throw new FormatException("File size is smaller than header size.");
            }

            this.memoryMappedFile = MemoryMappedFile.CreateFromFile(filename, FileMode.Open, null, fileSizeInBytes);

            // read header
            using (var headerAccessor = this.memoryMappedFile.CreateViewAccessor(0L, headerSizeInBytes))
            {
                Header header;
                headerAccessor.Read(0L, out header);

                this.size0 = header.Size0;
                this.size1 = header.Size1;
                this.count = checked((long)this.size0 * this.size1);
                this.sizeOfT = Marshal.SizeOf(typeof(T));
            }

            // check parameters
            if (this.size0 <= 0)
            {
                this.memoryMappedFile.Dispose();
                throw new FormatException("size0 is not positive.");
            }

            if (this.size1 <= 0)
            {
                this.memoryMappedFile.Dispose();
                throw new FormatException("size1 is not positive.");
            }
            // create accessor for data
            this.accessor = this.memoryMappedFile.CreateViewAccessor(headerSizeInBytes, fileSizeInBytes - headerSizeInBytes);
        }

        #endregion

        #region Public Methods

        public static MemoryMappedArray<T> CreateNew(string filename, int length)
        {
            return new MemoryMappedArray<T>(filename, length);
        }

        public static MemoryMappedArray<T> CreateNew(string filename, int size0, int size1)
        {
            return new MemoryMappedArray<T>(filename, size0, size1);
        }

        public static MemoryMappedArray<T> Open(string filename)
        {
            return new MemoryMappedArray<T>(filename);
        }

        #endregion

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

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Header
    {
        public TypeCode TypeCode;

        public int Size0;

        public int Size1;
    }
}