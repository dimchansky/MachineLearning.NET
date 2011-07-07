﻿namespace MachineLearning.LatentSemanticAnalysis
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public sealed class SparceVector<T> : IEnumerable<KeyValuePair<int, T>>
        where T : struct, IEquatable<T>
    {
        #region Fields and Properties

        private readonly SortedDictionary<int, T> innerVector;

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
                    this.innerVector.Remove(idx);
                }
                else
                {
                    this.innerVector[idx] = value;
                }
            }
        }

        public int NonZeroValuesCount
        {
            get
            {
                return this.innerVector.Count;
            }
        }

        #endregion

        #region Constructors

        public SparceVector()
        {
            this.innerVector = new SortedDictionary<int, T>();
        }

        public SparceVector(IDictionary<int, T> dictionary)
        {
            this.innerVector = new SortedDictionary<int, T>(dictionary);
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
            return this.innerVector.GetEnumerator();
        }

        #endregion

        #region Public Methods

        public void Add(int index, T value)
        {
            this[index] = value;
        }

        #endregion
    }
}