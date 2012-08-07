namespace MachineLearning.Classification.LogisticRegression
{
    using System;
    using System.Collections.Generic;

    using MachineLearning.Classification.Interfaces;

    public class LogisticRegressionClassifier :
        ISupervisedClassifier<bool, double>,
        IProbabilisticClassifier<bool, double>
    {
        private readonly int featuresCount;
        private readonly double regularizationParameter;
        private readonly double[] thetas;
        private bool trained;

        public IEnumerable<double> Thetas
        {
            get { return this.thetas; }
        }

        public LogisticRegressionClassifier(int featuresCount, double regularizationParameter = 0.0)
        {
            if (featuresCount <= 0)
            {
                throw new ArgumentOutOfRangeException("featuresCount", featuresCount, "Features count must be greater than zero.");
            }
            if (regularizationParameter < 0)
            {
                throw new ArgumentOutOfRangeException("regularizationParameter", regularizationParameter, "Regularization parameter must be non negative number.");
            }

            this.featuresCount = featuresCount;
            this.regularizationParameter = regularizationParameter;
            this.thetas = new double[featuresCount + 1]; // add one column for intercept
        }

        public void Train(IDataSet<bool, double> trainingDataSet)
        {
            if (trainingDataSet == null)
            {
                throw new ArgumentNullException("trainingDataSet");
            }

            var classifierTraining = new LogisticRegressionClassifierTraining(this.featuresCount, this.regularizationParameter, trainingDataSet);
            var trainedThetas = classifierTraining.Train();

            Array.Copy(trainedThetas, this.thetas, this.thetas.Length);
            this.trained = true;
        }

        public bool Classify(double[] attributes)
        {
            this.CheckConstrains(attributes);

            var h = LogisticRegression.Hypothesis(attributes, this.thetas);

            return h >= 0.5;
        }

        public double GetCategoryProbability(bool category, double[] attributes)
        {
            this.CheckConstrains(attributes);

            var h = LogisticRegression.Hypothesis(attributes, this.thetas);

            return category ? h : 1.0 - h;
        }

        private void CheckConstrains(double[] attributes)
        {
            if (attributes == null)
            {
                throw new ArgumentNullException("attributes");
            }
            if (attributes.Length != this.featuresCount)
            {
                throw new ArgumentOutOfRangeException("attributes.Length", attributes.Length, String.Format("Attributes count must be equal to {0}", this.featuresCount));
            }

            if (!this.trained)
            {
                throw new InvalidOperationException("You have to train classifier first.");
            }
        }
    }
}