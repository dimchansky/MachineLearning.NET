namespace MachineLearning.Classification
{
    using System;

    interface IClassifier<out TCategory, in TAttribute>
        where TCategory : IEquatable<TCategory>
        where TAttribute : IEquatable<TAttribute>
    {
        TCategory Classify(TAttribute[] attributes);
    }
}
