namespace MachineLearning.Classification
{
    using System;
    using System.Collections.Generic;

    public static class InMemoryDataSet
    {
        public static IDataSet<TCategory, TAttribute> Create<TCategory, TAttribute>(TrainingSample<TCategory, TAttribute>[] data)
            where TCategory : IEquatable<TCategory> 
            where TAttribute : IEquatable<TAttribute>
        {
            return new InMemoryDataSet<TCategory, TAttribute>(data);
        }
    }

    public sealed class InMemoryDataSet<TCategory, TAttribute>: IDataSet<TCategory, TAttribute>
        where TCategory : IEquatable<TCategory> 
        where TAttribute : IEquatable<TAttribute>
    {
        #region Field and properties

        private readonly TrainingSample<TCategory, TAttribute>[] data;

        #endregion

        #region Constructors

        public InMemoryDataSet(TrainingSample<TCategory, TAttribute>[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            this.data = data;
        }

        #endregion

        #region Implementation of IDataSet<TCategory,TAttribute>

        public IEnumerable<TrainingSample<TCategory, TAttribute>> GetData()
        {
            return data;
        }

        #endregion
    }
}
