namespace MachineLearning.Classification.Evaluation
{
    /// <summary>
    /// The <see cref="ConfusionMatrix"/> contains information about actual and predicted classifications done by a classification system.
    /// </summary>
    public class ConfusionMatrix
    {
        /// <summary>
        /// Gets or sets the true positives count.
        /// </summary>
        /// <value>The true positives count.</value>
        public int TruePositivesCount { get; set; }

        /// <summary>
        /// Gets or sets the false positives count.
        /// </summary>
        /// <value>The false positives count.</value>
        public int FalsePositivesCount { get; set; }

        /// <summary>
        /// Gets or sets the true negatives count.
        /// </summary>
        /// <value>The true negatives count.</value>
        public int TrueNegativesCount { get; set; }

        /// <summary>
        /// Gets or sets the false negatives count.
        /// </summary>
        /// <value>The false negatives count.</value>
        public int FalseNegativesCount { get; set; }

        /// <summary>
        /// Gets the percentage of positive predictions that are correct (P). 
        /// P = P(positive|positive predicted)
        /// </summary>
        /// <value>The precision.</value>
        public double GetPrecision()
        {
            return this.TruePositivesCount / ((double)this.TruePositivesCount + this.FalsePositivesCount);
        }

        /// <summary>
        /// Gets the percentage of positive labeled instances that were predicted as positive (R).
        /// R = P(positive predicted|positive)
        /// </summary>
        /// <value>The recall.</value>
        public double GetRecall()
        {
            return this.TruePositivesCount / ((double)this.TruePositivesCount + this.FalseNegativesCount);
        }

        /// <summary>
        /// Gets the proportion of the total number of predictions that were correct (AC).
        /// AC = P(correct prediction)
        /// The accuracy may not be an adequate performance measure when the number of negative cases is much greater than the number of positive cases.
        /// Suppose there are 1000 cases, 995 of which are negative cases and 5 of which are positive cases. 
        /// If the system classifies them all as negative, the accuracy would be 99.5%, even though the classifier missed all positive cases.
        /// </summary>
        /// <value>The accuracy.</value>
        public double GetAccuracy()
        {
            return ((double)this.TruePositivesCount + this.TrueNegativesCount) /
                    ((double)this.TruePositivesCount + this.TrueNegativesCount + this.FalsePositivesCount + this.FalseNegativesCount);
        }

        /// <summary>
        /// Gets the harmonic mean (F measure).
        /// </summary>
        /// <param name="beta">The beta.</param>
        /// <returns></returns>
        public double GetHarmonicMean()
        {
            double recall = this.GetRecall();
            double precision = this.GetPrecision();
            return 2.0 * precision * recall / (precision + recall);
        }

        /// <summary>
        /// Gets the harmonic mean (F measure).
        /// Values of <paramref name="beta"/> &lt; 1 emphasize precision, whereas values of <paramref name="beta"/> &gt; 1 emphasize recall.
        /// </summary>
        /// <param name="beta">The beta.</param>
        /// <returns></returns>
        public double GetHarmonicMean(double beta)
        {
            double recall = this.GetRecall();
            double precision = this.GetPrecision();
            double beta2 = beta * beta;
            return (beta2 + 1.0) * precision * recall / (beta2 * precision + recall);
        }
    }
}
