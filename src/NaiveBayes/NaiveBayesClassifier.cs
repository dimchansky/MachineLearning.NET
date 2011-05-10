namespace DataMining.NaiveBayes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class NaiveBayesClassifier
    {
        public static NaiveBayesClassifier<TCategory, TAttributeKey> Create<TCategory, TAttributeKey>()
            where TCategory : IEquatable<TCategory>
            where TAttributeKey : IEquatable<TAttributeKey>
        {
            return new NaiveBayesClassifier<TCategory, TAttributeKey>();
        }

        public static NaiveBayesClassifier<TCategory, TAttributeKey> Create<TCategory, TAttributeKey>(
            IEnumerable<TrainingSample<TCategory, TAttributeKey>> trainingSamples)
            where TCategory : IEquatable<TCategory>
            where TAttributeKey : IEquatable<TAttributeKey>
        {
            return new NaiveBayesClassifier<TCategory, TAttributeKey>(trainingSamples);
        }
    }

    public class NaiveBayesClassifier<TCategory, TAttributeKey>
        where TCategory : IEquatable<TCategory>
        where TAttributeKey : IEquatable<TAttributeKey>
    {
        private long totalSamples;
        private readonly HashSet<TCategory> knownConceptSet = new HashSet<TCategory>();
        private readonly IDictionary<TCategory, long> categoryCount = new Dictionary<TCategory, long>();
        private readonly IDictionary<TCategory, IDictionary<Attribute<TAttributeKey>, long>> categoryAttributeCount = new Dictionary<TCategory, IDictionary<Attribute<TAttributeKey>, long>>();

        public NaiveBayesClassifier()
            : this(Enumerable.Empty<TrainingSample<TCategory, TAttributeKey>>())
        {
        }

        public NaiveBayesClassifier(IEnumerable<TrainingSample<TCategory, TAttributeKey>> trainingSamples)
        {
            if (trainingSamples == null) throw new ArgumentNullException("trainingSamples");
            foreach (var trainingSample in trainingSamples)
            {
                Train(trainingSample);
            }
        }

        public void Train(TrainingSample<TCategory, TAttributeKey> trainingSample)
        {
            Train(trainingSample.Attributes, trainingSample.Category, trainingSample.IncrementCount);
        }

        public void Train(IEnumerable<Attribute<TAttributeKey>> attributes, TCategory category, int incrementCount = 1)
        {
            // add category to known concept set
            this.knownConceptSet.Add(category);

            // increase category count
            long catCount;
            if (!this.categoryCount.TryGetValue(category, out catCount))
            {
                catCount = 0L;
            }
            this.categoryCount[category] = catCount + incrementCount;

            // increase attributes count in specified category
            IDictionary<Attribute<TAttributeKey>, long> attributeCount;
            if (!this.categoryAttributeCount.TryGetValue(category, out attributeCount))
            {
                attributeCount = new Dictionary<Attribute<TAttributeKey>, long>();
                this.categoryAttributeCount.Add(category, attributeCount);
            }
            foreach (var attribute in attributes.Where(attribute => attribute != null))
            {
                long count;
                if (!attributeCount.TryGetValue(attribute, out count))
                {
                    count = 0L;
                }
                attributeCount[attribute] = count + incrementCount;
            }

            // increase total samples count
            this.totalSamples += incrementCount;
        }

        public double GetProbability(TCategory category, IEnumerable<Attribute<TAttributeKey>> attributes)
        {
            return this.knownConceptSet.Contains(category)
                       ? GetProbability(attributes, category) * GetProbability(category) / GetProbability(attributes)
                       : 1.0 / (this.knownConceptSet.Count + 1.0);
        }

        public double GetProbability(IEnumerable<Attribute<TAttributeKey>> attributes, TCategory category)
        {
            double probability = 1.0;

            var catCount = this.categoryCount[category];
            var attributeCount = this.categoryAttributeCount[category];

            foreach (var attribute in attributes.Where(attribute => attribute != null))
            {
                long count;
                if (!attributeCount.TryGetValue(attribute, out count))
                {
                    probability *= 1.0 / (this.totalSamples + 1);
                }
                else
                {
                    probability *= (double)count / catCount;
                }
            }

            return probability == 1.0 ? 1.0 / this.knownConceptSet.Count : probability;
        }

        public double GetProbability(TCategory category)
        {
            long catCount;
            if (!this.categoryCount.TryGetValue(category, out catCount))
            {
                catCount = 0L;
            }

            return (double)catCount / this.totalSamples;
        }

        public double GetProbability(IEnumerable<Attribute<TAttributeKey>> attributes)
        {
            double probability = this.knownConceptSet.Sum(category => GetProbability(attributes, category) * GetProbability(category));

            return probability == 0.0 ? 1.0 / this.totalSamples : probability;
        }

        public TCategory Classify(IEnumerable<Attribute<TAttributeKey>> attributes)
        {
            if (this.totalSamples == 0 || this.knownConceptSet.Count == 0)
            {
                throw new InvalidOperationException("You have to train classifier first.");
            }

            var bestCategory = default(TCategory);
            var bestProbability = 0.0;

            foreach (var category in this.knownConceptSet)
            {
                double probability = GetProbability(category, attributes);
                if (probability >= bestProbability)
                {
                    bestCategory = category;
                    bestProbability = probability;
                }
            }

            return bestCategory;
        }
    }
}
