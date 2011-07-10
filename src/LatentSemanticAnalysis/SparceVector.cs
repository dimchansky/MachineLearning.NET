namespace MachineLearning.LatentSemanticAnalysis
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

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
        }

        public SparceVector(IEnumerable<KeyValuePair<int, T>> pairs)
        {
            if (pairs == null)
            {
                throw new ArgumentNullException("pairs");
            }
            foreach (var pair in pairs)
            {
                this[pair.Key] = pair.Value;
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
            return this.innerVector.GetEnumerator();
        }

        #endregion

        #region Public Methods

        public void Add(int index, T value)
        {
            this[index] = value;
        }

        #endregion

        #region Equality Members

        public bool Equals(SparceVector<T> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return SortedDictionaryEquals(other.innerVector, this.innerVector);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != typeof(SparceVector<T>))
            {
                return false;
            }
            return Equals((SparceVector<T>)obj);
        }

        public override int GetHashCode()
        {
            return SortedDictionaryGetHashCode(this.innerVector);
        }

        public static bool operator ==(SparceVector<T> left, SparceVector<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SparceVector<T> left, SparceVector<T> right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region Helpers

        private static bool SortedDictionaryEquals(ICollection<KeyValuePair<int, T>> dict1, ICollection<KeyValuePair<int, T>> dict2)
        {
            if (ReferenceEquals(dict1, dict2))
            {
                return true;
            }
            if (ReferenceEquals(dict1, null) || ReferenceEquals(dict2, null))
            {
                return false;
            }

            return dict1.Count == dict2.Count && dict1.SequenceEqual(dict2);
        }

        private static int SortedDictionaryGetHashCode(ICollection<KeyValuePair<int, T>> dict)
        {
            return dict == null ? 0 : dict.Aggregate(dict.Count, (acc, pair) => CombineHashCodes(acc, CombineHashCodes(pair.Key.GetHashCode(), pair.Value.GetHashCode())));
        }

        private static int CombineHashCodes(int h1, int h2)
        {
            return (((h1 << 5) + h1) ^ h2);
        }

        #endregion
    }
}