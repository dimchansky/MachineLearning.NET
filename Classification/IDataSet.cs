namespace MachineLearning.Classification
{
    using System;
    using System.Collections.Generic;

    public interface IDataSet<TCategory, TAttribute> : IClassifier<TCategory, TAttribute>
        where TCategory : IEquatable<TCategory>
        where TAttribute : IEquatable<TAttribute>
    {
        IEnumerable<TrainingSample<TCategory, TAttribute>> GetData();
    }
}
