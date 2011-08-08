namespace MachineLearning.Collections.Array
{
    using System;

    internal sealed class TransposedArray<T> : IArray<T>
        where T : struct
    {
        private readonly IArray<T> underlyingArray;

        internal TransposedArray(IArray<T> underlyingArray)
        {
            if (underlyingArray == null)
            {
                throw new ArgumentNullException("underlyingArray");
            }
            this.underlyingArray = underlyingArray;
        }

        #region Implementation of IArrayBase

        public int Size0
        {
            get
            {
                return underlyingArray.Size1;
            }
        }

        public int Size1
        {
            get
            {
                return underlyingArray.Size0;
            }
        }

        public long NumElements
        {
            get
            {
                return underlyingArray.NumElements;
            }
        }

        #endregion

        #region Implementation of IArray<T>

        T IArray<T>.this[int index]
        {
            get
            {
                return underlyingArray[index];
            }
            set
            {
                underlyingArray[index] = value;
            }
        }

        T IArray<T>.this[int index0, int index1]
        {
            get
            {
                this.CheckIndexes(index0, index1);
                return underlyingArray[index1, index0];
            }
            set
            {
                this.CheckIndexes(index0, index1);
                underlyingArray[index1, index0] = value;
            }
        }

        public IArray<T> Transpose()
        {
            return underlyingArray;
        }

        #endregion

        #region Helpers

        private void CheckIndexes(int index0, int index1)
        {
            if (index0 < 0 || index0 >= this.Size0)
            {
                throw new ArgumentOutOfRangeException(
                    "index0", "Specified argument was out of the range of valid values.");
            }
            if (index1 < 0 || index1 >= this.Size1)
            {
                throw new ArgumentOutOfRangeException(
                    "index1", "Specified argument was out of the range of valid values.");
            }

        }

        #endregion

    }
}