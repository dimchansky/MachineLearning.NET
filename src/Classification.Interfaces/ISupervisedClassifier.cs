namespace MachineLearning.Classification.Interfaces
{
    using System;

    public interface ISupervisedClassifier<TCategory, TAttribute> : IClassifier<TCategory, TAttribute>
        where TCategory : IEquatable<TCategory> 
        where TAttribute : IEquatable<TAttribute>
    {
        void Train(IDataSet<TCategory, TAttribute> trainingDataSet);
    }
}
