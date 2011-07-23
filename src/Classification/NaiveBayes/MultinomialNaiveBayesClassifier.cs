namespace MachineLearning.Classification.NaiveBayes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MultinomialNaiveBayesClassifier<TCategory, TAttribute> : ISupervisedClassifier<TCategory, TAttribute>
        where TCategory : IEquatable<TCategory> where TAttribute : IEquatable<TAttribute>
    {
        #region Fields and properties

        private long totalTrainingSamplesCount;

        private readonly Dictionary<TCategory, long> categorySamplesCount = new Dictionary<TCategory, long>();

        private readonly Dictionary<TCategory, Dictionary<TAttribute, long>> categoryAttributesSamplesCount =
            new Dictionary<TCategory, Dictionary<TAttribute, long>>();

        private readonly HashSet<TAttribute> knownAttributes = new HashSet<TAttribute>();

        #endregion

        #region Implementation of ISupervisedClassifier<TCategory,TAttribute>

        public void Train(IDataSet<TCategory, TAttribute> trainingDataSet)
        {
            if (trainingDataSet == null)
            {
                throw new ArgumentNullException("trainingDataSet");
            }

            Train(trainingDataSet.GetData());
        }

        #endregion

        #region Implementation of IClassifier<out TCategory,in TAttribute>

        public TCategory Classify(TAttribute[] attributes)
        {
            if (attributes == null)
            {
                throw new ArgumentNullException("attributes");
            }

            if (this.totalTrainingSamplesCount == 0 || this.categorySamplesCount.Count == 0 || knownAttributes.Count == 0)
            {
                throw new InvalidOperationException("You have to train classifier first.");
            }

            var bestCategory = default(TCategory);
            var bestScore = double.NegativeInfinity;

            foreach (var pair in this.categorySamplesCount)
            {
                TCategory category = pair.Key;
                long categoryCount = pair.Value;

                // initial score log(P(category))
                double score = Math.Log((double)categoryCount / totalTrainingSamplesCount);

                // get all attributes count in category
                Dictionary<TAttribute, long> attributesCount;
                if (!this.categoryAttributesSamplesCount.TryGetValue(category, out attributesCount))
                {
                    attributesCount = null;
                }

                // using Laplace smoothing
                double denominator = knownAttributes.Count +
                    (attributesCount != null ? attributesCount.Sum(p => p.Value) : 0L);

                // updating score from given attributes
                foreach (var attributeGroup in from attribute in attributes group attribute by attribute)
                {
                    var attribute = attributeGroup.Key;
                    var attributeCount = attributeGroup.Count();

                    long attributeSamplesCount;
                    if (attributesCount == null || !attributesCount.TryGetValue(attribute, out attributeSamplesCount))
                    {
                        attributeSamplesCount = 0L;
                    }

                    double nominator = (attributeSamplesCount + 1L); // using Laplace smoothing                        

                    // updating score
                    score += attributeCount * Math.Log(nominator / denominator);
                }
                
                if (score <= bestScore)
                {
                    continue;
                }

                bestCategory = category;
                bestScore = score;
            }

            return bestCategory;
        }

        #endregion

        #region Public Methods

        public void Train(IEnumerable<TrainingSample<TCategory, TAttribute>> trainingSamples)
        {
            foreach (var trainingSample in trainingSamples)
            {
                this.Train(trainingSample);
            }
        }

        public void Train(TrainingSample<TCategory, TAttribute> trainingSample)
        {
            if (trainingSample == null)
            {
                throw new ArgumentNullException("trainingSample");
            }

            TCategory category = trainingSample.Category;
            TAttribute[] attributes = trainingSample.Attributes;
            int count = trainingSample.Count;

            this.Train(category, attributes, count);
        }

        public void Train(TCategory category, IEnumerable<TAttribute> attributes, int count = 1)
        {
            if (attributes == null)
            {
                throw new ArgumentNullException("attributes");
            }

            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException("count", "count must be a positive integer.");
            }

            // update total training samples count
            this.totalTrainingSamplesCount += count;

            // update total training samples count in given category
            long categoryCount;
            this.categorySamplesCount[category] = this.categorySamplesCount.TryGetValue(category, out categoryCount)
                                                                ? categoryCount + count
                                                                : count;

            // update attributes count for given category
            Dictionary<TAttribute, long> attributesCount;
            if (!this.categoryAttributesSamplesCount.TryGetValue(category, out attributesCount))
            {
                this.categoryAttributesSamplesCount[category] = attributesCount = new Dictionary<TAttribute, long>();
            }
            
            foreach (var attributeGroup in from attribute in attributes group attribute by attribute)
            {
                TAttribute attribute = attributeGroup.Key;
                long totalAttributeCount = (long)count * attributeGroup.Count();

                // update attribute count
                long attributeCount;
                attributesCount[attribute] = attributesCount.TryGetValue(attribute, out attributeCount)
                                                 ? attributeCount + totalAttributeCount
                                                 : totalAttributeCount;

                // update known attributes set
                knownAttributes.Add(attribute);
            }
        }

        #endregion
    }
}
