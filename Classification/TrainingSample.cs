namespace MachineLearning.Classification
{
    public class TrainingSample<TCategory, TAttribute>
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