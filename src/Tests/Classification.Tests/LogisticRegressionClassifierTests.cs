namespace Classification.Tests
{
    using System;
    using System.Linq;

    using MachineLearning.Classification.Interfaces;
    using MachineLearning.Classification.LogisticRegression;
    using MachineLearning.Classification.Model;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class LogisticRegressionClassifierTests
    {
        private const double Delta = 1E-13;

        private readonly IDataSet<bool, double> dataSet1 = DataHelper.GetDataSet1();
        private readonly IDataSet<bool, double> dataSet2 = DataHelper.GetDataSet2();

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorThrowsArgumentOutOfRangeExceptionIfFeaturesCountIsNotPositiveNumber()
        {
            // arrange

            // act
            var classifier = new LogisticRegressionClassifier(0, 0.0);

            // assert
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorThrowsArgumentOutOfRangeExceptionIfRegularizationParameterIsSmallerThanZero()
        {
            // arrange            

            // act        
            var classifier = new LogisticRegressionClassifier(1, -1.0);

            // assert
            Assert.Fail();
        }

        [Test]
        public void ConstructorDoesNotThrowExceptionIfAllParametersIsGood()
        {
            // arrange            

            // act
            var classifier = new LogisticRegressionClassifier(1, 0.0);

            // assert
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ClassifyThrowsArgumentNullExceptionIfAttributesIsNull()
        {
            // arrange            
            var dataSetMock = new Mock<IDataSet<bool, double>>();
            dataSetMock.Setup(ds => ds.GetTrainingSamplesCount()).Returns(1);
            dataSetMock.Setup(ds => ds.GetData()).Returns(new[] { TrainingSample.Create(true, new double[1]) });

            var classifier = new LogisticRegressionClassifier(1, 0.0);
            classifier.Train(dataSetMock.Object);

            // act
            classifier.Classify(null);

            // assert
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ClassifyThrowsArgumentOutOfRangeExceptionIfAttributesCountIsNotEqualToFeaturesCount()
        {
            // arrange            
            var dataSetMock = new Mock<IDataSet<bool, double>>();
            dataSetMock.Setup(ds => ds.GetTrainingSamplesCount()).Returns(1);
            dataSetMock.Setup(ds => ds.GetData()).Returns(new[] { TrainingSample.Create(true, new double[1]) });

            var classifier = new LogisticRegressionClassifier(1, 0.0);
            classifier.Train(dataSetMock.Object);

            // act
            classifier.Classify(new double[0]);

            // assert
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ClassifyThrowsInvalidOperationExceptionIfClassifierIsNotTrained()
        {
            // arrange            
            var dataSetMock = new Mock<IDataSet<bool, double>>();
            dataSetMock.Setup(ds => ds.GetTrainingSamplesCount()).Returns(1);
            dataSetMock.Setup(ds => ds.GetData()).Returns(new[] { TrainingSample.Create(true, new double[1]) });

            var classifier = new LogisticRegressionClassifier(1, 0.0);

            // act
            classifier.Classify(new double[1]);

            // assert
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetCategoryProbabilityThrowsArgumentNullExceptionIfAttributesIsNull()
        {
            // arrange            
            var dataSetMock = new Mock<IDataSet<bool, double>>();
            dataSetMock.Setup(ds => ds.GetTrainingSamplesCount()).Returns(1);
            dataSetMock.Setup(ds => ds.GetData()).Returns(new[] { TrainingSample.Create(true, new double[1]) });

            var classifier = new LogisticRegressionClassifier(1, 0.0);
            classifier.Train(dataSetMock.Object);

            // act
            classifier.GetCategoryProbability(true, null);

            // assert
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetCategoryProbabilityThrowsArgumentOutOfRangeExceptionIfAttributesCountIsNotEqualToFeaturesCount()
        {
            // arrange            
            var dataSetMock = new Mock<IDataSet<bool, double>>();
            dataSetMock.Setup(ds => ds.GetTrainingSamplesCount()).Returns(1);
            dataSetMock.Setup(ds => ds.GetData()).Returns(new[] { TrainingSample.Create(true, new double[1]) });

            var classifier = new LogisticRegressionClassifier(1, 0.0);
            classifier.Train(dataSetMock.Object);

            // act
            classifier.GetCategoryProbability(true, new double[0]);

            // assert
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetCategoryProbabilityThrowsInvalidOperationExceptionIfClassifierIsNotTrained()
        {
            // arrange            
            var dataSetMock = new Mock<IDataSet<bool, double>>();
            dataSetMock.Setup(ds => ds.GetTrainingSamplesCount()).Returns(1);
            dataSetMock.Setup(ds => ds.GetData()).Returns(new[] { TrainingSample.Create(true, new double[1]) });

            var classifier = new LogisticRegressionClassifier(1, 0.0);

            // act
            classifier.GetCategoryProbability(true, new double[1]);

            // assert
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TrainThrowsArgumentNullExceptionIfTrainingDataSetIsNull()
        {
            // arrange
            var classifier = new LogisticRegressionClassifier(1, 0.0);

            // act
            classifier.Train(null);

            // assert
            Assert.Fail();
        }

        [Test]
        public void TrainDoesNotThrowExceptionOnValidDataSet()
        {
            // arrange
            var dataSetMock = new Mock<IDataSet<bool, double>>();
            dataSetMock.Setup(ds => ds.GetTrainingSamplesCount()).Returns(1);
            dataSetMock.Setup(ds => ds.GetData()).Returns(new[] { TrainingSample.Create(true, new double[1]) });

            var classifier = new LogisticRegressionClassifier(1, 0.0);

            // act
            classifier.Train(dataSetMock.Object);

            // assert
        }

        [Test]
        public void ThetasPropertyReturnsAllZerosIfClassifierIsNotTrained()
        {
            // arrange            
            var classifier = new LogisticRegressionClassifier(1, 0.0);

            // act
            var thetas = classifier.Thetas;

            // assert
            Assert.IsTrue(thetas.All(d => d == 0));
        }

        [Test]
        public void ThetasPropertyAfterTrainingReturnsTheSameThetasAsTrainerReturns()
        {
            // arrange
            var dataSetMock = new Mock<IDataSet<bool, double>>();
            dataSetMock.Setup(ds => ds.GetTrainingSamplesCount()).Returns(2);
            dataSetMock.Setup(ds => ds.GetData()).Returns(new[] { TrainingSample.Create(true, new[] { 0.0 }), 
                                                                  TrainingSample.Create(false, new[] { 1.0 }) });

            const int FeaturesCount = 1;
            const double RegularizationParameter = 0.0;

            var classifier = new LogisticRegressionClassifier(FeaturesCount, RegularizationParameter);
            classifier.Train(dataSetMock.Object);

            var classifierTraining = new LogisticRegressionClassifierTraining(FeaturesCount, RegularizationParameter, dataSetMock.Object);
            var expectedThetas = classifierTraining.Train();

            // act
            var thetas = classifier.Thetas;

            // assert
            Assert.IsTrue(expectedThetas.SequenceEqual(thetas));
        }

        [Test]
        public void GetCategoryProbabilityReturnsCorrectProbabilityForTestData1()
        {
            // arrange
            const int FeaturesCount = 2;
            const double RegularizationParameter = 0.0; // no regularization

            var classifier = new LogisticRegressionClassifier(FeaturesCount, RegularizationParameter);
            classifier.Train(this.dataSet1);

            // act
            var probability = classifier.GetCategoryProbability(true, new[] { 45.0, 85.0 });

            // assert
            Assert.AreEqual(0.77628055852165889, probability, Delta);
        }

        [Test]
        public void ClassifyAccuracyOnTestData1Is89Percents()
        {
            // arrange
            const int FeaturesCount = 2;
            const double RegularizationParameter = 0.0; // no regularization

            var classifier = new LogisticRegressionClassifier(FeaturesCount, RegularizationParameter);
            classifier.Train(this.dataSet1);

            // act
            var correctClassifications = this.dataSet1.GetData()
                .Select(data => classifier.Classify(data.Attributes) == data.Category ? data.Count : 0)
                .Sum();

            var accuracyPercents = (double)correctClassifications * 100 / this.dataSet1.GetTrainingSamplesCount();

            // assert
            Assert.AreEqual(89.0, accuracyPercents, Delta);
        }

        [Test]
        public void ClassifyAccuracyOnTestData2UsingDegree6AndRegularization1Is83Point05Percents()
        {
            // arrange
            const int Degree = 6;
            var mappedFeaturesCount = DataHelper.TwoMappedFeaturesToDegreeCount(Degree);
            const double RegularizationParameter = 1.0;

            var mappedDataSet2 = DataHelper.MapTwoFeaturesDataSetToDegree(this.dataSet2, Degree);

            var classifier = new LogisticRegressionClassifier(mappedFeaturesCount, RegularizationParameter);
            classifier.Train(mappedDataSet2);

            // act
            var correctClassifications = mappedDataSet2.GetData()
                .Select(data => classifier.Classify(data.Attributes) == data.Category ? data.Count : 0)
                .Sum();

            var accuracyPercents = (double)correctClassifications * 100 / mappedDataSet2.GetTrainingSamplesCount();

            // assert
            Assert.AreEqual(83.050847457627114, accuracyPercents, Delta);
        }
    }
}