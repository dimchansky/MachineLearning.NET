namespace MachineLearning.Classification.Evaluation
{
    /// <summary>
    /// The <see cref="ConfusionMatrix"/> contains information about actual and predicted classifications done by a classification system.
    /// </summary>
    public class ConfusionMatrix : ReadonlyConfusionMatrix
    {
        /// <summary>
        /// Gets or sets the true positives count.
        /// </summary>
        /// <value>The true positives count.</value>
        public int TruePositivesCount
        {
            get
            {
                return this.truePositivesCount;
            }
            set
            {
                this.truePositivesCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the false positives count.
        /// </summary>
        /// <value>The false positives count.</value>
        public int FalsePositivesCount
        {
            get
            {
                return this.falsePositivesCount;
            }
            set
            {
                this.falsePositivesCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the true negatives count.
        /// </summary>
        /// <value>The true negatives count.</value>
        public int TrueNegativesCount
        {
            get
            {
                return this.trueNegativesCount;
            }
            set
            {
                this.trueNegativesCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the false negatives count.
        /// </summary>
        /// <value>The false negatives count.</value>
        public int FalseNegativesCount
        {
            get
            {
                return this.falseNegativesCount;
            }
            set
            {
                this.falseNegativesCount = value;
            }
        }
    }
}
