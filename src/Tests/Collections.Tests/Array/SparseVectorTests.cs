namespace Collections.Tests.Array
{
    using System;
    using System.Linq;

    using Collections.Tests.Helpers;

    using MachineLearning.Collections.Array;

    using NUnit.Framework;

    using TestHelpers;

    [TestFixture]
    public class SparseVectorTests
    {
        [Test]
        public void IndexedPropertyGetAccessorReturnsWhatWasSavedThroughSetAccessors()
        {
            // arrange           
            var originalVector = SparseVectorHelper.GenerateRandomVector(1000, 0.7, () => SparseVectorHelper.RandomInInterval(-100, 100, 2));

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

        [Test]
        public void IndexedPropertyGetAccessorReturnsWhatWasSavedThroughAddMethod()
        {
            // arrange           
            var originalVector = SparseVectorHelper.GenerateRandomVector(1000, 0.7, () => SparseVectorHelper.RandomInInterval(-100, 100, 2));

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

        [Test]
        public void IndexedPropertyGetAccessorReturnsWhatWasSavedThroughConstructor()
        {
            // arrange           
            var originalVector = SparseVectorHelper.GenerateRandomVector(1000, 0.7, () => SparseVectorHelper.RandomInInterval(-100, 100, 2));

            // act
            var sv = new SparseVector<double>(originalVector);

            // assert
            foreach (var element in originalVector)
            {
                Assert.AreEqual(element.Value, sv[element.Key]);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsArgumentNullExceptionIfNullArgumentPassed()
        {
            // arrange

            // act
            var sv = new SparseVector<double>(null);
            
            // assert
            Assert.Fail("SparseVector ctor must throw ArgumentNullException if null argument passed.");
        }

        [Test]
        public void NonZeroValuesCountReturnsZeroForEmptyVector()
        {
            // arrange
            var sv = new SparseVector<double>();

            // act

            // assert
            Assert.AreEqual(0, sv.NonZeroValuesCount);
            
        }

        [Test]
        public void NonZeroValuesCountReturnsOnlyNonZeroElementsCount()
        {
            // arrange
            var originalVector = SparseVectorHelper.GenerateRandomVector(1000, 0.7, () => SparseVectorHelper.RandomInInterval(-100, 100, 2));

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

        [Test]
        public void EnumeratorReturnsOnlyNonZeroValuePairsOrderedByIndexAscending()
        {
            // arrange
            var originalVector = SparseVectorHelper.GenerateRandomVector(1000, 0.7, () => SparseVectorHelper.RandomInInterval(-100, 100, 2));
            var sv = new SparseVector<double>(originalVector);
            var nonZeroValuesPairs = originalVector.Where(pair => pair.Value != 0.0).OrderBy(pair => pair.Key).ToArray();

            // act

            // assert
            Assert.IsTrue(sv.SequenceEqual(nonZeroValuesPairs));
        }

        [Test]
        public void EqualOperatorReturnsTrueForEqualSparseVectors()
        {
            // arrange
            var originalVector = SparseVectorHelper.GenerateRandomVector(1000, 0.7, () => SparseVectorHelper.RandomInInterval(-100, 100, 2));
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

        [Test]
        public void EqualsReturnsFalseForNullArgument()
        {
            // arrange
            var originalVector = SparseVectorHelper.GenerateRandomVector(1000, 0.7, () => SparseVectorHelper.RandomInInterval(-100, 100, 2));
            var sv1 = new SparseVector<double>(originalVector);
            SparseVector<double> sv2 = null;

            // act
            var falseResult1 = sv1.Equals(sv2);
            var falseResult2 = sv1.Equals((object)sv2);

            // assert
            Assert.IsFalse(falseResult1);
            Assert.IsFalse(falseResult2);
        }

        [Test]
        public void EqualsReturnsFalseForDifferentTypeArgument()
        {
            // arrange
            var originalVector = SparseVectorHelper.GenerateRandomVector(1000, 0.7, () => SparseVectorHelper.RandomInInterval(-100, 100, 2));
            var sv = new SparseVector<double>(originalVector);

            // act
            var falseResult1 = sv.Equals(string.Empty);

            // assert
            Assert.IsFalse(falseResult1);
        }

        [Test]
        public void EqualsReturnsTrueForSameReferenceObjects()
        {
            // arrange
            var originalVector = SparseVectorHelper.GenerateRandomVector(1000, 0.7, () => SparseVectorHelper.RandomInInterval(-100, 100, 2));
            var sv1 = new SparseVector<double>(originalVector);
            SparseVector<double> sv2 = sv1;

            // act
            var trueResult1 = sv1.Equals(sv2);
            var trueResult2 = sv1.Equals((object)sv2);

            // assert
            Assert.IsTrue(trueResult1);
            Assert.IsTrue(trueResult2);
        }

        [Test]
        public void GetHashCodeReturnsSameHashCodeForEqualSparseVectors()
        {
            // arrange
            var originalVector = SparseVectorHelper.GenerateRandomVector(1000, 0.7, () => SparseVectorHelper.RandomInInterval(-100, 100, 2));
            var originalVectorCopy = originalVector.ToArray();
            var sv1 = new SparseVector<double>(originalVector);
            var sv2 = new SparseVector<double>(originalVectorCopy);
            
            // act
            var hc1 = sv1.GetHashCode();
            var hc2 = sv2.GetHashCode();

            // assert
            Assert.AreEqual(hc1, hc2);
        }
    }
}
