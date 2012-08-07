namespace MachineLearning.Classification.LogisticRegression
{
    using System;
    using System.Collections.Generic;

    public static class LogisticRegression
    {
        public static double Hypothesis(IList<double> xs, IList<double> thetas)
        {
            var hl = DotProduct(xs, thetas);

            return Sigmoid(hl);
        }

        public static double DotProduct(IList<double> xs, IList<double> thetas)
        {
            if (xs == null)
            {
                throw new ArgumentNullException("xs");
            }
            if (thetas == null)
            {
                throw new ArgumentNullException("thetas");
            }
            if (xs.Count + 1 != thetas.Count)
            {
                throw new ArgumentOutOfRangeException("xs.Count", xs.Count, string.Format("xs.Count must be equal to {0}", thetas.Count));
            }

            var hl = thetas[0];
            for (var i = 1; i < thetas.Count; i++)
            {
                hl = hl + xs[i - 1] * thetas[i];
            }

            return hl;
        }

        public static double Sigmoid(double value)
        {
            return 1.0 / (1.0 + Math.Exp(-value));
        }
    }
}