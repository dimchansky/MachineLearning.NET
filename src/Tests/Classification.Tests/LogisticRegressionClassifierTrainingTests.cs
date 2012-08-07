namespace Classification.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MachineLearning.Classification.Interfaces;
    using MachineLearning.Classification.LogisticRegression;
    using MachineLearning.Classification.Model;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class LogisticRegressionClassifierTrainingTests
    {
        private const double Delta = 1E-13;

        private readonly IDataSet<bool, double> dataSet1 = DataHelper.GetDataSet1();
        private readonly IDataSet<bool, double> dataSet2 = DataHelper.GetDataSet2();

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorThrowsArgumentOutOfRangeExceptionIfFeaturesCountIsNotPositiveNumber()
        {
            // arrange            
            var dataSet = new Mock<IDataSet<bool, double>>().Object;

            // act
            var trainer = new LogisticRegressionClassifierTraining(0, 0.0, dataSet);

            // assert
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorThrowsArgumentOutOfRangeExceptionIfRegularizationParameterIsSmallerThanZero()
        {
            // arrange            
            var dataSet = new Mock<IDataSet<bool, double>>().Object;

            // act
            var trainer = new LogisticRegressionClassifierTraining(1, -1.0, dataSet);

            // assert
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsArgumentNullExceptionIfDataSetIsNull()
        {
            // arrange            

            // act
            var trainer = new LogisticRegressionClassifierTraining(1, 0.0, null);

            // assert
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorThrowsArgumentOutOfRangeException()
        {
            // arrange            
            var dataSetMock = new Mock<IDataSet<bool, double>>();
            dataSetMock.Setup(ds => ds.GetTrainingSamplesCount()).Returns(0);
            dataSetMock.Setup(ds => ds.GetData()).Returns(new TrainingSample<bool, double>[0]);

            // act
            var trainer = new LogisticRegressionClassifierTraining(1, 0.0, dataSetMock.Object);

            // assert
            Assert.Fail();
        }

        [Test]
        public void ConstructorDoesNotThrowExceptionIfAllParametersIsGood()
        {
            // arrange            
            var dataSetMock = new Mock<IDataSet<bool, double>>();
            dataSetMock.Setup(ds => ds.GetTrainingSamplesCount()).Returns(1);
            dataSetMock.Setup(ds => ds.GetData()).Returns(new[] { TrainingSample.Create(true, new double[1]) });

            // act
            var trainer = new LogisticRegressionClassifierTraining(1, 0.0, dataSetMock.Object);

            // assert            
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CostFunctionThrowsArgumentNullExceptionIfThetasIsNull()
        {
            // arrange            
            var dataSetMock = new Mock<IDataSet<bool, double>>();
            dataSetMock.Setup(ds => ds.GetTrainingSamplesCount()).Returns(1);
            dataSetMock.Setup(ds => ds.GetData()).Returns(new[] { TrainingSample.Create(true, new double[1]) });
            var trainer = new LogisticRegressionClassifierTraining(1, 0.0, dataSetMock.Object);

            // act
            var cost = trainer.CostFunction(null, new double[0]);

            // assert
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CostFunctionThrowsArgumentNullExceptionIfGradIsNull()
        {
            // arrange            
            var dataSetMock = new Mock<IDataSet<bool, double>>();
            dataSetMock.Setup(ds => ds.GetTrainingSamplesCount()).Returns(1);
            dataSetMock.Setup(ds => ds.GetData()).Returns(new[] { TrainingSample.Create(true, new double[1]) });
            var trainer = new LogisticRegressionClassifierTraining(1, 0.0, dataSetMock.Object);

            // act
            var cost = trainer.CostFunction(new double[0], null);

            // assert
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CostFunctionThrowsArgumentOutOfRangeExceptionIfThetasCountIsNotEqualToFeaturesCountPlusOne()
        {
            // arrange            
            var dataSetMock = new Mock<IDataSet<bool, double>>();
            dataSetMock.Setup(ds => ds.GetTrainingSamplesCount()).Returns(1);
            dataSetMock.Setup(ds => ds.GetData()).Returns(new[] { TrainingSample.Create(true, new double[1]) });
            var trainer = new LogisticRegressionClassifierTraining(1, 0.0, dataSetMock.Object);

            // act
            var cost = trainer.CostFunction(new double[0], new double[2]);

            // assert
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CostFunctionThrowsArgumentOutOfRangeExceptionIfGradCountIsNotEqualToFeaturesCountPlusOne()
        {
            // arrange            
            var dataSetMock = new Mock<IDataSet<bool, double>>();
            dataSetMock.Setup(ds => ds.GetTrainingSamplesCount()).Returns(1);
            dataSetMock.Setup(ds => ds.GetData()).Returns(new[] { TrainingSample.Create(true, new double[1]) });
            var trainer = new LogisticRegressionClassifierTraining(1, 0.0, dataSetMock.Object);

            // act
            var cost = trainer.CostFunction(new double[2], new double[0]);

            // assert
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CostFunctionThrowsNullReferenceExceptionIfOneOfTheTrainingSamplesIsNull()
        {
            // arrange            
            var dataSetMock = new Mock<IDataSet<bool, double>>();
            dataSetMock.Setup(ds => ds.GetTrainingSamplesCount()).Returns(1);
            dataSetMock.Setup(ds => ds.GetData()).Returns(new TrainingSample<bool, double>[] { null });
            var trainer = new LogisticRegressionClassifierTraining(1, 0.0, dataSetMock.Object);

            // act
            var cost = trainer.CostFunction(new double[2], new double[2]);

            // assert
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CostFunctionThrowsNullReferenceExceptionIfOneOfTheTrainingSamplesAttributesIsNull()
        {
            // arrange            
            var dataSetMock = new Mock<IDataSet<bool, double>>();
            dataSetMock.Setup(ds => ds.GetTrainingSamplesCount()).Returns(1);
            dataSetMock.Setup(ds => ds.GetData()).Returns(new[] { TrainingSample.Create(true, (double[])null) });
            var trainer = new LogisticRegressionClassifierTraining(1, 0.0, dataSetMock.Object);

            // act
            var cost = trainer.CostFunction(new double[2], new double[2]);

            // assert
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CostFunctionThrowsArgumentOutOfRangeExceptionIfOneOfTheTrainingSamplesAttributesLengthIsNotEqualToFeaturesCount()
        {
            // arrange            
            var dataSetMock = new Mock<IDataSet<bool, double>>();
            dataSetMock.Setup(ds => ds.GetTrainingSamplesCount()).Returns(1);
            dataSetMock.Setup(ds => ds.GetData()).Returns(new[] { TrainingSample.Create(true, new double[0]) });
            var trainer = new LogisticRegressionClassifierTraining(1, 0.0, dataSetMock.Object);

            // act
            var cost = trainer.CostFunction(new double[2], new double[2]);

            // assert
            Assert.Fail();
        }

        [Test]
        public void CostFunctionReturnsCorrectCostAndGradientForTestData1WithZerosInitialThetas()
        {
            // arrange
            const int FeaturesCount = 2;
            const double RegularizationParameter = 0.0; // no regularization

            var classifierTraining = new LogisticRegressionClassifierTraining(FeaturesCount, RegularizationParameter, this.dataSet1);

            IList<double> initialThetas = new double[3];
            IList<double> grad = new double[3]; // will be updated

            // act
            var cost = classifierTraining.CostFunction(initialThetas, grad);

            // assert
            Assert.AreEqual(0.69314718055994584, cost, Delta);
            Assert.AreEqual(-0.1, grad[0], Delta);
            Assert.AreEqual(-12.00921658929115, grad[1], Delta);
            Assert.AreEqual(-11.262842205513591, grad[2], Delta);
        }

        [Test]
        public void TrainReturnsCorrectThetasForTestData1()
        {
            // arrange
            const int FeaturesCount = 2;
            const double RegularizationParameter = 0.0; // no regularization

            var classifierTraining = new LogisticRegressionClassifierTraining(FeaturesCount, RegularizationParameter, this.dataSet1);

            // act
            var thetas = classifierTraining.Train();

            // assert
            Assert.AreEqual(-25.160497465074084, thetas[0], Delta);
            Assert.AreEqual(0.20622413037584902, thetas[1], Delta);
            Assert.AreEqual(0.20146509204776417, thetas[2], Delta);
        }

        [Test]
        public void CostFunctionReturnsCorrectCostAndGradientForTestData2WithZerosInitialThetasSixDegreesPolynomAndRegularizationParameterOne()
        {
            // arrange
            const int Degree = 6;
            var mappedFeaturesCount = DataHelper.TwoMappedFeaturesToDegreeCount(Degree);
            const double RegularizationParameter = 1.0;

            var mappedDataSet2 = DataHelper.MapTwoFeaturesDataSetToDegree(this.dataSet2, Degree);

            var classifierTraining = new LogisticRegressionClassifierTraining(mappedFeaturesCount, RegularizationParameter, mappedDataSet2);

            IList<double> initialThetas = new double[mappedFeaturesCount + 1];
            IList<double> grad = new double[mappedFeaturesCount + 1]; // will be updated

            // act
            var cost = classifierTraining.CostFunction(initialThetas, grad);

            // assert
            Assert.AreEqual(0.69314718055994606, cost, Delta);
            Assert.AreEqual(0.00847457627118644, grad[0], Delta);
            Assert.AreEqual(0.018788093220338989, grad[1], Delta);
            Assert.AreEqual(7.777118644068388E-05, grad[2], Delta);
            Assert.AreEqual(0.050344639536355922, grad[3], Delta);
            Assert.AreEqual(0.011501330787338986, grad[4], Delta);
            Assert.AreEqual(0.037664847359550863, grad[5], Delta);
            Assert.AreEqual(0.018355987221154262, grad[6], Delta);
            Assert.AreEqual(0.0073239339112221545, grad[7], Delta);
            Assert.AreEqual(0.00819244468389037, grad[8], Delta);
            Assert.AreEqual(0.023476488865153241, grad[9], Delta);
            Assert.AreEqual(0.039348623439160173, grad[10], Delta);
            Assert.AreEqual(0.0022392390663969436, grad[11], Delta);
            Assert.AreEqual(0.012860050337133708, grad[12], Delta);
            Assert.AreEqual(0.0030959372024053963, grad[13], Delta);
            Assert.AreEqual(0.03930281711039442, grad[14], Delta);
            Assert.AreEqual(0.019970746726922388, grad[15], Delta);
            Assert.AreEqual(0.0043298323241713673, grad[16], Delta);
            Assert.AreEqual(0.0033864390190702, grad[17], Delta);
            Assert.AreEqual(0.0058382207780586087, grad[18], Delta);
            Assert.AreEqual(0.0044762906651224848, grad[19], Delta);
            Assert.AreEqual(0.031007984901327702, grad[20], Delta);
            Assert.AreEqual(0.031031244228507674, grad[21], Delta);
            Assert.AreEqual(0.0010974023848666578, grad[22], Delta);
            Assert.AreEqual(0.0063157079664203581, grad[23], Delta);
            Assert.AreEqual(0.00040850300602094216, grad[24], Delta);
            Assert.AreEqual(0.0072650431643416884, grad[25], Delta);
            Assert.AreEqual(0.001376461747689044, grad[26], Delta);
            Assert.AreEqual(0.038793636344838761, grad[27], Delta);
        }

        [Test]
        public void CostFunctionReturnsCorrectCostAndGradientForTestData2WithOnesInitialThetasSixDegreesPolynomAndRegularizationParameterOne()
        {
            // arrange
            const int Degree = 6;
            var mappedFeaturesCount = DataHelper.TwoMappedFeaturesToDegreeCount(Degree);
            const double RegularizationParameter = 1.0;

            var mappedDataSet2 = DataHelper.MapTwoFeaturesDataSetToDegree(this.dataSet2, Degree);

            var classifierTraining = new LogisticRegressionClassifierTraining(mappedFeaturesCount, RegularizationParameter, mappedDataSet2);

            IList<double> initialOnesThetas = Enumerable.Repeat(1.0, mappedFeaturesCount + 1).ToArray();
            IList<double> grad = new double[mappedFeaturesCount + 1]; // will be updated

            // act
            var cost = classifierTraining.CostFunction(initialOnesThetas, grad);

            // assert
            Assert.AreEqual(2.1348483146660664, cost, Delta);
            Assert.AreEqual(0.34604507367924525, grad[0], Delta);
            Assert.AreEqual(0.085080732840233653, grad[1], Delta);
            Assert.AreEqual(0.11852456917131907, grad[2], Delta);
            Assert.AreEqual(0.15059159578437173, grad[3], Delta);
            Assert.AreEqual(0.015914488662613836, grad[4], Delta);
            Assert.AreEqual(0.16811439027324665, grad[5], Delta);
            Assert.AreEqual(0.067120936535572842, grad[6], Delta);
            Assert.AreEqual(0.032170528551636587, grad[7], Delta);
            Assert.AreEqual(0.026043207959570054, grad[8], Delta);
            Assert.AreEqual(0.10719727102949513, grad[9], Delta);
            Assert.AreEqual(0.097258847173850457, grad[10], Delta);
            Assert.AreEqual(0.010984331999342433, grad[11], Delta);
            Assert.AreEqual(0.041956570008359576, grad[12], Delta);
            Assert.AreEqual(0.0095721182298489117, grad[13], Delta);
            Assert.AreEqual(0.12367775873639067, grad[14], Delta);
            Assert.AreEqual(0.058955340734334054, grad[15], Delta);
            Assert.AreEqual(0.018704087603607224, grad[16], Delta);
            Assert.AreEqual(0.017293228450946573, grad[17], Delta);
            Assert.AreEqual(0.023526654852795346, grad[18], Delta);
            Assert.AreEqual(0.015130387245933704, grad[19], Delta);
            Assert.AreEqual(0.098581230325589381, grad[20], Delta);
            Assert.AreEqual(0.073283231256194875, grad[21], Delta);
            Assert.AreEqual(0.010514468913800548, grad[22], Delta);
            Assert.AreEqual(0.022705671362770082, grad[23], Delta);
            Assert.AreEqual(0.0090483240173878, grad[24], Delta);
            Assert.AreEqual(0.025635476344317537, grad[25], Delta);
            Assert.AreEqual(0.00823079217024389, grad[26], Delta);
            Assert.AreEqual(0.1060120397374798, grad[27], Delta);
        }

        [Test]
        public void TrainReturnsCorrectThetasForTestData2UsingSixDegreesPolynomAndRegularizationParameterOne()
        {
            // arrange
            const int Degree = 6;
            var mappedFeaturesCount = DataHelper.TwoMappedFeaturesToDegreeCount(Degree);
            const double RegularizationParameter = 1.0;

            var mappedDataSet2 = DataHelper.MapTwoFeaturesDataSetToDegree(this.dataSet2, Degree);

            var classifierTraining = new LogisticRegressionClassifierTraining(mappedFeaturesCount, RegularizationParameter, mappedDataSet2);

            // act
            var thetas = classifierTraining.Train();

            // assert
            Assert.AreEqual(1.2727257777291563, thetas[0], Delta);
            Assert.AreEqual(0.62535425095813046, thetas[1], Delta);
            Assert.AreEqual(1.1809815913524897, thetas[2], Delta);
            Assert.AreEqual(-2.019823757989518, thetas[3], Delta);
            Assert.AreEqual(-0.91748583789176419, thetas[4], Delta);
            Assert.AreEqual(-1.4316901766601973, thetas[5], Delta);
            Assert.AreEqual(0.12398226958728931, thetas[6], Delta);
            Assert.AreEqual(-0.36539570405212035, thetas[7], Delta);
            Assert.AreEqual(-0.35711892921468064, thetas[8], Delta);
            Assert.AreEqual(-0.17505537401554372, thetas[9], Delta);
            Assert.AreEqual(-1.458267839991064, thetas[10], Delta);
            Assert.AreEqual(-0.051039587950303174, thetas[11], Delta);
            Assert.AreEqual(-0.61573442413697599, thetas[12], Delta);
            Assert.AreEqual(-0.27467033331805246, thetas[13], Delta);
            Assert.AreEqual(-1.1927379097969517, thetas[14], Delta);
            Assert.AreEqual(-0.24230087297391839, thetas[15], Delta);
            Assert.AreEqual(-0.20588671185024385, thetas[16], Delta);
            Assert.AreEqual(-0.044782757804708882, thetas[17], Delta);
            Assert.AreEqual(-0.27781068196339476, thetas[18], Delta);
            Assert.AreEqual(-0.29530423532247418, thetas[19], Delta);
            Assert.AreEqual(-0.45626038474961511, thetas[20], Delta);
            Assert.AreEqual(-1.0433750496212086, thetas[21], Delta);
            Assert.AreEqual(0.027761239512917313, thetas[22], Delta);
            Assert.AreEqual(-0.2925082482769511, thetas[23], Delta);
            Assert.AreEqual(0.015549020302576095, thetas[24], Delta);
            Assert.AreEqual(-0.32745530880927648, thetas[25], Delta);
            Assert.AreEqual(-0.14387492070263289, thetas[26], Delta);
            Assert.AreEqual(-0.92456452390048494, thetas[27], Delta);
        }
    }
}