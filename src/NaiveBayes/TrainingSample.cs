namespace DataMining.NaiveBayes
{
    using System;
    using System.Collections.Generic;

    public static class TrainingSample
    {
        public static TrainingSample<TCategory, TAttributeKey> Create<TCategory, TAttributeKey>(IEnumerable<Attribute<TAttributeKey>> attributes, TCategory category)
            where TCategory : IEquatable<TCategory>
            where TAttributeKey : IEquatable<TAttributeKey>
        {
            return new TrainingSample<TCategory, TAttributeKey>(attributes, category);
        }

        public static TrainingSample<TCategory, TAttributeKey> Create<TCategory, TAttributeKey>(IEnumerable<Attribute<TAttributeKey>> attributes, TCategory category, int incrementCount)
            where TCategory : IEquatable<TCategory>
            where TAttributeKey : IEquatable<TAttributeKey>
        {
            return new TrainingSample<TCategory, TAttributeKey>(attributes, category, incrementCount);
        }
    }

    public sealed class TrainingSample<TCategory, TAttributeKey>
        where TCategory : IEquatable<TCategory>
        where TAttributeKey : IEquatable<TAttributeKey>
    {
        private readonly IEnumerable<Attribute<TAttributeKey>> attributes;
        private readonly TCategory category;
        private readonly int incrementCount;

        public IEnumerable<Attribute<TAttributeKey>> Attributes
        {
            get { return this.attributes; }
        }

        public TCategory Category
        {
            get { return this.category; }
        }

        public int IncrementCount
        {
            get
            {
                return this.incrementCount;
            }
        }

        public TrainingSample(IEnumerable<Attribute<TAttributeKey>> attributes, TCategory category, int incrementCount = 1)
        {
            this.attributes = attributes;
            this.category = category;
            this.incrementCount = incrementCount;
        }
    }

}