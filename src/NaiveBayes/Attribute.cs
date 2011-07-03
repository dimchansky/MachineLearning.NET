namespace MachineLearning.NaiveBayes
{
    using System;

    public static class Attribute
    {
        public static Attribute<TKey> Create<TKey,TValue>(TKey key, TValue value) 
            where TKey : IEquatable<TKey> 
            where TValue : IEquatable<TValue>
        {
            return new Attribute<TKey, TValue>(key, value);
        }
    }

    public abstract class Attribute<TKey> : IEquatable<Attribute<TKey>> 
        where TKey : IEquatable<TKey>
    {
        private readonly TKey key;

        public TKey Key
        {
            get { return this.key; }
        }

        protected Attribute(TKey key)
        {
            this.key = key;
        }

        public bool Equals(Attribute<TKey> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.key, this.key) && ValueEquals(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof (Attribute<TKey>) && Equals((Attribute<TKey>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.key != null ? this.key.GetHashCode() : 0) * 397) ^ (GetValueHashCode());
            }
        }

        public static bool operator ==(Attribute<TKey> left, Attribute<TKey> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Attribute<TKey> left, Attribute<TKey> right)
        {
            return !Equals(left, right);
        }

        protected abstract bool ValueEquals(Attribute<TKey> other);
        protected abstract int GetValueHashCode();
    }

    public class Attribute<TKey,TValue> : Attribute<TKey>
        where TKey : IEquatable<TKey>
        where TValue : IEquatable<TValue>
    {
        private readonly TValue value;

        public TValue Value
        {
            get { return this.value; }
        }

        public Attribute(TKey key, TValue value) : base(key)
        {
            this.value = value;
        }

        protected override bool ValueEquals(Attribute<TKey> other)
        {
            return !ReferenceEquals(null, other) && Equals(this.value, ((Attribute<TKey, TValue>) other).value);
        }

        protected override int GetValueHashCode()
        {
            return this.value != null ? this.value.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return this.Key + ": " + this.value;
        }
    }
}
