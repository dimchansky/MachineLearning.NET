namespace MachineLearning.LatentSemanticAnalysis
{
    using System;

    public static class DocumentTerms
    {
        public static DocumentTerms<TKey, TTerm> Create<TKey, TTerm>(TKey key, TTerm[] terms)
            where TKey : IEquatable<TKey>
            where TTerm : IEquatable<TTerm>
        {
            return new DocumentTerms<TKey, TTerm>(key, terms);
        }
    }

    public sealed class DocumentTerms<TKey, TTerm>
        where TKey : IEquatable<TKey>
        where TTerm : IEquatable<TTerm>
    {
        #region Fields and Properties

        private readonly TKey key;

        private readonly TTerm[] terms;

        public TKey Key
        {
            get
            {
                return this.key;
            }
        }

        public TTerm[] Terms
        {
            get
            {
                return this.terms;
            }
        }

        #endregion

        #region Constructors

        public DocumentTerms(TKey key, TTerm[] terms)
        {
            if (terms == null)
            {
                throw new ArgumentNullException("terms");
            }
            if (terms.Length == 0)
            {
                throw new ArgumentException("At least one term expected","terms");
            }
            this.key = key;
            this.terms = terms;
        }

        #endregion
    }
}
