namespace Collections.Tests.IO
{
    using System;

    using MachineLearning.Collections.Array;
    using MachineLearning.Collections.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TestHelpers;

    [TestClass]
    public class CachedMatrixMarketReaderTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsArgumentNullExceptionForNullReaderArgument()
        {
            // arrange
            
            // act
            var cachedReader = new CachedMatrixMarketReader<double>(null);

            // assert
            Assert.Fail();
        }

        [TestMethod]
        public void InitializedCachedMatrixMarketReaderReturnsTheSamePropertiesValues()
        {
            // arrange
            var reader = new InMemorySparseMatrixReader(
                new SparseVector<double> { { 0, 22.0 }, { 1, 28.0 } },
                new SparseVector<double> { { 0, 49.0 }, { 1, 64.0 } });

            var cachedReader = new CachedMatrixMarketReader<double>(reader);

            // act            

            // assert
            Assert.AreEqual(reader.ColumnsCount, cachedReader.ColumnsCount);
            Assert.AreEqual(reader.RowsCount, cachedReader.RowsCount);
            Assert.AreEqual(reader.ElementsCount, cachedReader.ElementsCount);
        }
    }
}
