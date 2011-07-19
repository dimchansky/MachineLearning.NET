namespace MachineLearning.Classification.Evaluation
{
    using System;
    using System.Collections.Generic;

    public class BinaryClassifierEvaluator<TCategory, TAttribute>
        where TCategory : IEquatable<TCategory> 
        where TAttribute : IEquatable<TAttribute>
    {
        private readonly ConfusionMatrix confusionMatrix = new ConfusionMatrix();

        private readonly IClassifier<TCategory, TAttribute> classifier;

        private readonly Func<TCategory, bool> categorySelector;

        public ReadonlyConfusionMatrix ConfusionMatrix
        {
            get
            {
                return this.confusionMatrix;
            }
        }

        public BinaryClassifierEvaluator(IClassifier<TCategory, TAttribute> classifier, Func<TCategory, bool> categorySelector)
        {
            if (classifier == null)
            {
                throw new ArgumentNullException("classifier");
            }
            if (categorySelector == null)
            {
                throw new ArgumentNullException("categorySelector");
            }
            this.classifier = classifier;
            this.categorySelector = categorySelector;
        }

        public void UpdateEvaluation(IEnumerable<TrainingSample<TCategory, TAttribute>> testSamples)
        {
            if (testSamples == null)
            {
                throw new ArgumentNullException("testSamples");
            }

            foreach (var testSample in testSamples)
            {
                UpdateEvaluation(testSample);
            }
        }

        public void UpdateEvaluation(TrainingSample<TCategory, TAttribute> testSample)
        {
            if (testSample == null)
            {
                throw new ArgumentNullException("testSample");
            }

            bool predictedCategory = categorySelector(this.classifier.Classify(testSample.Attributes));
            bool actualCategory = categorySelector(testSample.Category);

            if (predictedCategory)
            {
                // positive predicted
                if (actualCategory)
                {
                    this.confusionMatrix.TruePositivesCount += testSample.Count;
                }
                else
                {
                    this.confusionMatrix.FalsePositivesCount += testSample.Count;
                }
            }
            else
            {
                // negative predicted
                if (actualCategory)
                {
                    this.confusionMatrix.FalseNegativesCount += testSample.Count;
                }
                else
                {
                    this.confusionMatrix.TrueNegativesCount += testSample.Count;
                }
            }
        }
    }
}
