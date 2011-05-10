namespace DataMining.NaiveBayes
{
    using System;

    public sealed class EquatableEnum<TEnum> : IEquatable<EquatableEnum<TEnum>>
        where TEnum : struct
    {
        private readonly TEnum enumValue;

        private EquatableEnum(TEnum enumValue)
        {
            this.enumValue = enumValue;
        }

        public bool Equals(EquatableEnum<TEnum> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(other.enumValue, this.enumValue);
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
            if (obj.GetType() != typeof(EquatableEnum<TEnum>))
            {
                return false;
            }
            return Equals((EquatableEnum<TEnum>)obj);
        }

        public override int GetHashCode()
        {
            return this.enumValue.GetHashCode();
        }

        public static bool operator ==(EquatableEnum<TEnum> left, EquatableEnum<TEnum> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EquatableEnum<TEnum> left, EquatableEnum<TEnum> right)
        {
            return !Equals(left, right);
        }

        public static implicit operator EquatableEnum<TEnum>(TEnum enumValue)
        {
            return new EquatableEnum<TEnum>(enumValue);
        }

        public static implicit operator TEnum(EquatableEnum<TEnum> equatableEnum)
        {
            return equatableEnum.enumValue;
        }

        public override string ToString()
        {
            return Enum.GetName(typeof(TEnum), this.enumValue);
        }
    }
}
