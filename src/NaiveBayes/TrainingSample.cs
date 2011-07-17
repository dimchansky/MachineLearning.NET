namespace MachineLearning.NaiveBayes
{
    using System;
    using System.Collections.Generic;

    public static class TrainingSample
    {
        public static TrainingSample<TCategory, TAttributeKey> Create<TCategory, TAttributeKey>(Attribute<TAttributeKey>[] attributes, TCategory category)
            where TCategory : IEquatable<TCategory>
            where TAttributeKey : IEquatable<TAttributeKey>
        {
            return new TrainingSample<TCategory, TAttributeKey>(attributes, category);
        }

        public static TrainingSample<TCategory, TAttributeKey> Create<TCategory, TAttributeKey>(Attribute<TAttributeKey>[] attributes, TCategory category, int incrementCount)
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
        private readonly Attribute<TAttributeKey>[] attributes;
        private readonly TCategory category;
        private readonly int incrementCount;

        public Attribute<TAttributeKey>[] Attributes
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

        public TrainingSample(Attribute<TAttributeKey>[] attributes, TCategory category, int incrementCount = 1)
        {
            this.attributes = attributes;
            this.category = category;
            this.incrementCount = incrementCount;
        }
    }

}