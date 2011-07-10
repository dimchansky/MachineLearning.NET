namespace LatentSemanticAnalysis.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MachineLearning.LatentSemanticAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SparseVectorTests
    {
        [TestMethod]
        public void IndexedPropertyGetAccessorReturnsWhatWasSavedThroughSetAccessors()
        {
            // arrange           
            var originalVector = GenerateRandomVector(1000, 0.7);

            var sv = new SparseVector<double>();

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

            var sv = new SparseVector<double>();

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
            var sv = new SparseVector<double>(originalVector);

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
            var sv = new SparseVector<double>(null);
            
            // assert
            Assert.Fail("SparseVector ctor must throw ArgumentNullException if null argument passed.");
        }

        [TestMethod]
        public void NonZeroValuesCountReturnsZeroForEmptyVector()
        {
            // arrange
            var sv = new SparseVector<double>();

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

            var sv = new SparseVector<double>();

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
            var sv = new SparseVector<double>(originalVector);
            var nonZeroValuesPairs = originalVector.Where(pair => pair.Value != 0.0).OrderBy(pair => pair.Key).ToArray();

            // act

            // assert
            Assert.IsTrue(sv.SequenceEqual(nonZeroValuesPairs));
        }

        [TestMethod]
        public void EqualOperatorReturnsTrueForEqualSparseVectors()
        {
            // arrange
            var originalVector = GenerateRandomVector(1000, 0.7);
            var originalVectorCopy = originalVector.ToArray();
            var sv1 = new SparseVector<double>(originalVector);
            var sv2 = new SparseVector<double>(originalVectorCopy);

            // act
            var trueResult = sv1 == sv2;
            var falseResult = sv1 != sv2;

            // assert
            Assert.IsTrue(trueResult);
            Assert.IsFalse(falseResult);
        }

        [TestMethod]
        public void EqualsReturnsFalseForNullArgument()
        {
            // arrange
            var originalVector = GenerateRandomVector(1000, 0.7);
            var sv1 = new SparseVector<double>(originalVector);
            SparseVector<double> sv2 = null;

            // act
            var falseResult1 = sv1.Equals(sv2);
            var falseResult2 = sv1.Equals((object)sv2);

            // assert
            Assert.IsFalse(falseResult1);
            Assert.IsFalse(falseResult2);
        }

        [TestMethod]
        public void EqualsReturnsFalseForDifferentTypeArgument()
        {
            // arrange
            var originalVector = GenerateRandomVector(1000, 0.7);
            var sv = new SparseVector<double>(originalVector);

            // act
            var falseResult1 = sv.Equals(string.Empty);

            // assert
            Assert.IsFalse(falseResult1);
        }

        [TestMethod]
        public void EqualsReturnsTrueForSameReferenceObjects()
        {
            // arrange
            var originalVector = GenerateRandomVector(1000, 0.7);
            var sv1 = new SparseVector<double>(originalVector);
            SparseVector<double> sv2 = sv1;

            // act
            var trueResult1 = sv1.Equals(sv2);
            var trueResult2 = sv1.Equals((object)sv2);

            // assert
            Assert.IsTrue(trueResult1);
            Assert.IsTrue(trueResult2);
        }

        [TestMethod]
        public void GetHashCodeReturnsSameHashCodeForEqualSparseVectors()
        {
            // arrange
            var originalVector = GenerateRandomVector(1000, 0.7);
            var originalVectorCopy = originalVector.ToArray();
            var sv1 = new SparseVector<double>(originalVector);
            var sv2 = new SparseVector<double>(originalVectorCopy);
            
            // act
            var hc1 = sv1.GetHashCode();
            var hc2 = sv2.GetHashCode();

            // assert
            Assert.AreEqual(hc1, hc2);
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
