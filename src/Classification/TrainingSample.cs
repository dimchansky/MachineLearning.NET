namespace MachineLearning.Classification
{
    using System;

    public static class TrainingSample
    {
        public static TrainingSample<TCategory, TAttribute> Create<TCategory, TAttribute>(TCategory category, TAttribute[] attributes) 
            where TCategory : IEquatable<TCategory>
            where TAttribute : IEquatable<TAttribute>
        {
            return new TrainingSample<TCategory, TAttribute>(category, attributes);
        }

        public static TrainingSample<TCategory, TAttribute> Create<TCategory, TAttribute>
            (TCategory category, TAttribute[] attributes, int count) where TCategory : IEquatable<TCategory>
            where TAttribute : IEquatable<TAttribute>
        {
            return new TrainingSample<TCategory, TAttribute>(category, attributes, count);
        }
    }

    public sealed class TrainingSample<TCategory, TAttribute>
        where TCategory : IEquatable<TCategory> where TAttribute : IEquatable<TAttribute>

    {
        private readonly TCategory category;

        private readonly TAttribute[] attributes;

        private readonly int count;

        public TCategory Category
        {
            get
            {
                return this.category;
            }
        }

        public TAttribute[] Attributes
        {
            get
            {
                return this.attributes;
            }
        }

        public int Count
        {
            get
            {
                return this.count;
            }
        }

        public TrainingSample(TCategory category, TAttribute[] attributes, int count = 1)
        {
            this.category = category;
            this.attributes = attributes;
            this.count = count;
        }
    }
}