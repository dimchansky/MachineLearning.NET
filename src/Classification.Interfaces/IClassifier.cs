namespace MachineLearning.Classification.Interfaces
{
    using System;

    public interface IClassifier<out TCategory, in TAttribute>
        where TCategory : IEquatable<TCategory> 
        where TAttribute : IEquatable<TAttribute>
    {
        #region Public Methods

        TCategory Classify(TAttribute[] attributes);

        #endregion
    }
}