namespace LatentSemanticAnalysis.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MachineLearning.LatentSemanticAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SparceVectorTests
    {
        [TestMethod]
        public void IndexedPropertyGetAccessorReturnsWhatWasSavedThroughSetAccessors()
        {
            // arrange           
            var originalVector = GenerateRandomVector(1000, 0.7);

            var sv = new SparceVector<double>();

            // act
            foreach (var element in originalVector)
            {
                sv[element.Key] = element.Value;
            }

            // assert
            foreach (var element in originalVector)
            {
                Assert.AreEqual(element.Value, sv[element.Key]);
            }
        }

        [TestMethod]
        public void IndexedPropertyGetAccessorReturnsWhatWasSavedThroughAddMethod()
        {
            // arrange           
            var originalVector = GenerateRandomVector(1000, 0.7);

            var sv = new SparceVector<double>();

            // act
            foreach (var element in originalVector)
            {
                sv.Add(element.Key, element.Value);
            }

            // assert
            foreach (var element in originalVector)
            {
                Assert.AreEqual(element.Value, sv[element.Key]);
            }            
        }

        [TestMethod]
        public void IndexedPropertyGetAccessorReturnsWhatWasSavedThroughConstructor()
        {
            // arrange           
            var originalVector = GenerateRandomVector(1000, 0.7);

            // act
            var sv = new SparceVector<double>(originalVector);

            // assert
            foreach (var element in originalVector)
            {
                Assert.AreEqual(element.Value, sv[element.Key]);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsArgumentNullExceptionIfNullArgumentPassed()
        {
            // arrange

            // act
            var sv = new SparceVector<double>(null);
            
            // assert
            Assert.Fail("SparceVector ctor must throw ArgumentNullException if null argument passed.");
        }

        [TestMethod]
        public void NonZeroValuesCountReturnsZeroForEmptyVector()
        {
            // arrange
            var sv = new SparceVector<double>();

            // act

            // assert
            Assert.AreEqual(0, sv.NonZeroValuesCount);
            
        }

        [TestMethod]
        public void NonZeroValuesCountReturnsOnlyNonZeroElementsCount()
        {
            // arrange
            var originalVector = GenerateRandomVector(1000, 0.7);            

            var nonZeroElementsCount = originalVector.Count(arg => arg.Value != 0.0);

            var sv = new SparceVector<double>();

            // act
            foreach (var element in originalVector)
            {
                sv[element.Key] = element.Value;
            }

            // assert
            Assert.AreEqual(nonZeroElementsCount, sv.NonZeroValuesCount);
        }

        [TestMethod]
        public void EnumeratorReturnsOnlyNonZeroValuePairsOrderedByIndexAscending()
        {
            // arrange
            var originalVector = GenerateRandomVector(1000, 0.7);
            var sv = new SparceVector<double>(originalVector);
            var nonZeroValuesPairs = originalVector.Where(pair => pair.Value != 0.0).OrderBy(pair => pair.Key).ToArray();

            // act

            // assert
            Assert.IsTrue(sv.SequenceEqual(nonZeroValuesPairs));
        }

        #region Helpers

        private static Dictionary<int, double> GenerateRandomVector(int count, double zeroProbability)
        {
            var rnd = new Random();

            return Enumerable.Range(0, count)
                .Select(i => new
                    {
                        Index = i, 
                        Value = rnd.NextDouble() < zeroProbability ? 0.0 : (double.Epsilon + rnd.NextDouble()) * (rnd.NextDouble() < 0.5 ? -1 : 1)
                    })
                .ToDictionary(arg => arg.Index, arg => arg.Value);
        }

        #endregion
    }
}
