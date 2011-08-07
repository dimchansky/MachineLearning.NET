﻿namespace Classification.Tests
{
    using MachineLearning.Classification;
    using MachineLearning.Classification.NaiveBayes;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MultinomialNaiveBayesClassifierTests
    {
        [TestMethod]
        public void CorrectlyClassifiesTestDataSetFromExample13Dot1()
        {
            var trainingData = InMemoryDataSet.Create(new[]
                        {
                            TrainingSample.Create("China", new[] { "Chinese", "Beijing", "Chinese" }),
                            TrainingSample.Create("China", new[] { "Chinese", "Chinese", "Shanghai" }),
                            TrainingSample.Create("China", new[] { "Chinese", "Makao" }),
                            TrainingSample.Create("Not China", new[] { "Tokio", "Japan", "Chinese" }),
                        });

            var classifier = MultinomialNaiveBayesClassifier.Create(trainingData);

            Assert.AreEqual("China", classifier.Classify(new[] { "Chinese", "Chinese", "Chinese", "Tokio", "Japan" }));
            Assert.AreEqual("Not China", classifier.Classify(new[] { "Tokio" }));
            Assert.AreEqual("China", classifier.Classify(new[] { "Chinese", "Tokio" }));
            Assert.AreEqual("China", classifier.Classify(new[] { "Unknown", "Chinese", "Tokio" }));
            Assert.AreEqual("Not China", classifier.Classify(new[] { "Chinese", "Tokio", "Japan" }));
        }
    }
}