namespace Classification.Tests
{
    using System;
    using System.Collections.Generic;

    using MachineLearning.Classification.LogisticRegression;

    using NUnit.Framework;

    [TestFixture]
    public class LogisticRegressionTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HypothesisThrowsExceptionIfXsIsNull()
        {
            // arrange           
            IList<double> xs = null;
            IList<double> thetas = new double[0];

            // act
            LogisticRegression.Hypothesis(xs, thetas);

            // assert
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HypothesisThrowsExceptionIfThetasIsNull()
        {
            // arrange           
            IList<double> xs = new double[0];
            IList<double> thetas = null;

            // act
            LogisticRegression.Hypothesis(xs, thetas);

            // assert
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void HypothesisThrowsArgumentOutOfRangeExceptionIfXsCountPlusOneIsNotEqualToThetasCount()
        {
            // arrange           
            IList<double> xs = new double[2];
            IList<double> thetas = new double[4];

            // act
            LogisticRegression.Hypothesis(xs, thetas);

            // assert
            Assert.Fail();
        }

        [Test]
        public void HypothesisReturnsThetaZeroSigmoidIfXsIsEmpty()
        {
            // arrange           
            var rnd = new Random();
            var randomDouble = rnd.NextDouble();
            var expectedResult = LogisticRegression.Sigmoid(randomDouble);

            IList<double> xs = new double[0];
            IList<double> thetas = new double[] { randomDouble };

            // act
            var result = LogisticRegression.Hypothesis(xs, thetas);

            // assert
            Assert.AreEqual(expectedResult, result, 1E-13);
        }

        [Test]
        public void HypothesisReturnsCorrectResultForTestData()
        {
            // arrange           
            IList<double> xs = new double[] { -1, -2 };
            IList<double> thetas = new double[] { 9, 8, 7 };

            var expectedResult = LogisticRegression.Sigmoid(thetas[0] + thetas[1] * xs[0] + thetas[2] * xs[1]);

            // act
            var result = LogisticRegression.Hypothesis(xs, thetas);

            // assert
            Assert.AreEqual(expectedResult, result, 1E-13);
        }

        [Test]
        public void SigmoidReturnsOpoint5ForZeroValue()
        {
            // arrange

            // act
            var result = LogisticRegression.Sigmoid(0);

            // assert
            Assert.AreEqual(0.5, result, 1E-13);
        }

        [TestCase(-10.0, 4.5397868702434395E-05)]
        [TestCase(-1.0, 0.2689414213699951)]
        [TestCase(0.0, 0.5)]
        [TestCase(1.0, 0.7310585786300049)]
        [TestCase(10.0, 0.99995460213129761)]
        public void SigmoidReturnsCorrectResultForTestsData(double value, double expectedResult)
        {
            // arrange

            // act
            var result = LogisticRegression.Sigmoid(value);

            // assert
            var error = Math.Abs(expectedResult - result);
            Assert.Less(error, double.Epsilon);
        }
    }
}