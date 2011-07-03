namespace MachineLearning.LatentSemanticAnalysis
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public sealed class SparceVector<T> : IEnumerable<KeyValuePair<int, T>>
        where T : struct, IEquatable<T>
    {
        #region Fields and Properties

        private readonly SortedDictionary<int, T> innerVector = new SortedDictionary<int, T>();

        public T this[int idx]
        {
            get
            {
                T value;
                return this.innerVector.TryGetValue(idx, out value) ? value : default(T);
            }
            set
            {
                if (value.Equals(default(T)))
                {
                    innerVector.Remove(idx);
                }
                else
                {
                    innerVector[idx] = value;
                }
            }
        }

        public int NonZeroValuesCount
        {
            get
            {
                return innerVector.Count;
            }
        }

        #endregion

        #region Implementation of IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Implementation of IEnumerable<out KeyValuePair<int,double>>

        public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
        {
            return innerVector.GetEnumerator();
        }

        #endregion
    }
}