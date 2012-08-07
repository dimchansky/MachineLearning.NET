namespace MachineLearning.Classification.Interfaces
{
    using System;

    public interface IProbabilisticClassifier<TCategory, in TAttribute> : IClassifier<TCategory, TAttribute>
        where TCategory : IEquatable<TCategory>
        where TAttribute : IEquatable<TAttribute>
    {
        double GetCategoryProbability(TCategory category, TAttribute[] attributes);
    }
}