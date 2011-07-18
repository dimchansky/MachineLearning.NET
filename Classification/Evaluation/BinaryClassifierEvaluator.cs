namespace MachineLearning.Classification.Evaluation
{
    using System;
    using System.Collections.Generic;

    public class BinaryClassifierEvaluator<TAttribute>
        where TAttribute : IEquatable<TAttribute>
    {
        private readonly ConfusionMatrix confusionMatrix = new ConfusionMatrix();

        private readonly IClassifier<bool, TAttribute> classifier;

        public ReadonlyConfusionMatrix ConfusionMatrix
        {
            get
            {
                return this.confusionMatrix;
            }
        }

        public BinaryClassifierEvaluator(IClassifier<bool, TAttribute> classifier)
        {
            if (classifier == null)
            {
                throw new ArgumentNullException("classifier");
            }
            this.classifier = classifier;
        }

        public void UpdateEvaluation(IEnumerable<TrainingSample<bool, TAttribute>> testSamples)
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

        public void UpdateEvaluation(TrainingSample<bool, TAttribute> testSample)
        {
            if (testSample == null)
            {
                throw new ArgumentNullException("testSample");
            }

            bool predictedCategory = this.classifier.Classify(testSample.Attributes);
            bool actualCategory = testSample.Category;

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
