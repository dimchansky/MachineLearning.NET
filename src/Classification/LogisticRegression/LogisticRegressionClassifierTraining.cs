namespace MachineLearning.Classification.LogisticRegression
{
    using System;
    using System.Collections.Generic;

    using MachineLearning.Classification.Interfaces;
    using MachineLearning.Classification.Utils;

    public sealed class LogisticRegressionClassifierTraining
    {
        private readonly int featuresCount;
        private readonly double regularizationParameter;
        private readonly IDataSet<bool, double> dataSet;
        private readonly int samplesCount;

        public LogisticRegressionClassifierTraining(int featuresCount, double regularizationParameter, IDataSet<bool, double> dataSet)
        {
            if (featuresCount <= 0)
            {
                throw new ArgumentOutOfRangeException("featuresCount", featuresCount, "Features count must be positive number.");
            }

            if (regularizationParameter < 0)
            {
                throw new ArgumentOutOfRangeException("regularizationParameter", regularizationParameter, "Regularization parameter must be non negative number.");
            }

            if (dataSet == null)
            {
                throw new ArgumentNullException("dataSet");
            }

            this.featuresCount = featuresCount;
            this.regularizationParameter = regularizationParameter;
            this.dataSet = dataSet;
            this.samplesCount = dataSet.GetTrainingSamplesCount();
            if (this.samplesCount <= 0)
            {
                throw new ArgumentOutOfRangeException("dataSet.GetTrainingSamplesCount()", this.samplesCount, "Samples count must be positive integer.");
            }
        }

        public double[] Train()
        {
            var startingThetas = new double[this.featuresCount + 1]; // add one column for intercept  

            var o = new QuasiNewtonOptimizer();
            return o.Minimize(this.CostFunction, startingThetas);
        }

        public double CostFunction(IList<double> thetas, IList<double> grad)
        {
            if (thetas == null)
            {
                throw new ArgumentNullException("thetas");
            }

            if (grad == null)
            {
                throw new ArgumentNullException("grad");
            }

            if (thetas.Count != this.featuresCount + 1)
            {
                throw new ArgumentOutOfRangeException("thetas.Count", thetas.Count, string.Format("Thetas count must be equal to {0}", this.featuresCount + 1));
            }

            if (grad.Count != this.featuresCount + 1)
            {
                throw new ArgumentOutOfRangeException("grad.Count", grad.Count, string.Format("Grad count must be equal to {0}", this.featuresCount + 1));
            }

            // set initial cost to zero
            var cost = 0.0;

            // set initial grad to zeros
            for (int i = 0; i < grad.Count; i++)
            {
                grad[i] = 0;
            }

            foreach (var trainingSample in this.dataSet.GetData())
            {
                if (trainingSample == null)
                {
                    throw new ArgumentNullException("trainingSample");
                }

                double[] xs = trainingSample.Attributes;
                if (xs == null)
                {
                    throw new ArgumentNullException("trainingSample.Attributes");
                }

                if (xs.Length != this.featuresCount)
                {
                    throw new ArgumentOutOfRangeException("trainingSample.Attributes", xs.Length, String.Format("Attributes count must be equal to {0}", this.featuresCount));
                }

                double weight = trainingSample.Count;

                double dot = LogisticRegression.DotProduct(xs, thetas);
                var h = LogisticRegression.Sigmoid(dot);

                // update cost
                cost = cost + weight * Math.Log(1 + Math.Exp(trainingSample.Category ? -dot : dot));

                // update grad
                var error = weight * (h - (trainingSample.Category ? 1.0 : 0.0));
                grad[0] = grad[0] + error * 1.0;
                for (int i = 1; i < thetas.Count; i++)
                {
                    grad[i] = grad[i] + error * xs[i - 1];
                }
            }

            // divide cost by number of samples and add regularization cost
            cost = cost / this.samplesCount + GetThetasCost(thetas, this.regularizationParameter, this.samplesCount);

            // divide grads by number of samples
            grad[0] = grad[0] / this.samplesCount;
            for (int i = 1; i < grad.Count; i++)
            {
                grad[i] = (grad[i] + this.regularizationParameter * thetas[i]) / this.samplesCount;
            }

            return cost;
        }

        private static double GetThetasCost(IList<double> thetas, double lambda, int samplesCount)
        {
            var thetaCost = 0.0;

            // theta[0] parameter should not be regularized, so we start from theta[1]
            for (var i = 1; i < thetas.Count; i++)
            {
                var ti = thetas[i];
                thetaCost = thetaCost + ti * ti;
            }

            return thetaCost * lambda / 2.0 / samplesCount;
        }
    }
}