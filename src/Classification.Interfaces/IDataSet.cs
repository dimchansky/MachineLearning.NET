namespace MachineLearning.Classification.Interfaces
{
    using System;
    using System.Collections.Generic;

    using MachineLearning.Classification.Model;

    public interface IDataSet<TCategory, TAttribute>
        where TCategory : IEquatable<TCategory> 
        where TAttribute : IEquatable<TAttribute>
    {
        IEnumerable<TrainingSample<TCategory, TAttribute>> GetData();
        int GetTrainingSamplesCount();
    }
}
