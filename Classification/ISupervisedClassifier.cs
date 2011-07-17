namespace MachineLearning.Classification
{
    using System;
    using System.Collections.Generic;

    public interface ISupervisedClassifier<TCategory, TAttribute> : IClassifier<TCategory, TAttribute>
        where TCategory : IEquatable<TCategory>
        where TAttribute : IEquatable<TAttribute>
    {
        void Train(IEnumerable<TrainingSample<TCategory, TAttribute>> trainingSamples);
    }
}
