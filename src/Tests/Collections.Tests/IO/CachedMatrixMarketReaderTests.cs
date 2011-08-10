namespace Collections.Tests.IO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Collections.Tests.Helpers;

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
            using(var cachedReader = new CachedMatrixMarketReader<double>(null))
            {
                
            }

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

            using(var cachedReader = new CachedMatrixMarketReader<double>(reader))
            {
                // act            

                // assert
                Assert.AreEqual(reader.ColumnsCount, cachedReader.ColumnsCount);
                Assert.AreEqual(reader.RowsCount, cachedReader.RowsCount);
                Assert.AreEqual(reader.ElementsCount, cachedReader.ElementsCount);
            }

        }

        [TestMethod]
        public void ReadRowsReturnSameVectorsInFirstScan()
        {
            // arrange
            const int rows = 100;
            const int columns = 100;
            const double zeroProbability = 0.99;
            var originalVectors = Enumerable
                .Range(0, rows)
                .Select(i => SparseVectorHelper.GenerateRandomSparseVector(columns, zeroProbability))
                .ToArray();
            var originalReader = new InMemorySparseMatrixReader(originalVectors);
            
            using(var cachedReader = new CachedMatrixMarketReader<double>(originalReader))
            {
                // act
                var firstScanResults = originalReader.ReadRows().Zip(cachedReader.ReadRows(), (originalVector, cachedVector) => originalVector == cachedVector).All(b => b);

                // assert
                Assert.IsTrue(firstScanResults);
            }
        }

        [TestMethod]
        public void ReadRowsReturnSameVectorsInSecondScan()
        {
            // arrange
            const int rows = 100;
            const int columns = 100;
            const double zeroProbability = 0.99;
            var originalVectors = Enumerable
                .Range(0, rows)
                .Select(i => SparseVectorHelper.GenerateRandomSparseVector(columns, zeroProbability))
                .ToArray();
            var originalReader = new InMemorySparseMatrixReader(originalVectors);

            using (var cachedReader = new CachedMatrixMarketReader<double>(originalReader))
            {
                // act
                foreach (var row in cachedReader.ReadRows())
                {                    
                }                
                var secondScanResults = originalReader.ReadRows().Zip(cachedReader.ReadRows(), (originalVector, cachedVector) => originalVector == cachedVector).All(b => b);

                // assert
                Assert.IsTrue(secondScanResults);
            }
        }

        [TestMethod]
        public void ReadRowsInSecondScanDoNotUseOriginalReader()
        {
            // arrange
            const int rows = 100;
            const int columns = 100;
            const double zeroProbability = 0.99;
            var originalVectors = Enumerable
                .Range(0, rows)
                .Select(i => SparseVectorHelper.GenerateRandomSparseVector(columns, zeroProbability))
                .ToArray();
            var originalReader = new InMemorySparseMatrixReader(originalVectors);
            var originalReaderWithCounters = new SparseMatrixReaderWithMemberInvocationCounters(originalReader);

            using (var cachedReader = new CachedMatrixMarketReader<double>(originalReaderWithCounters))
            {
                // act
                foreach (var row in cachedReader.ReadRows())
                {
                }
                foreach (var row in cachedReader.ReadRows())
                {
                }                

                // assert
                Assert.AreEqual(1, originalReaderWithCounters.ColumnsCountInvocations);
                Assert.AreEqual(1, originalReaderWithCounters.RowsCountInvocations);
                Assert.AreEqual(1, originalReaderWithCounters.ElementsCountInvocations);
                Assert.AreEqual(1, originalReaderWithCounters.ReadRowsInvocations);
            }
        }

        #region Helpers

        private class SparseMatrixReaderWithMemberInvocationCounters : ISparseMatrixReader<double>
        {
            private readonly ISparseMatrixReader<double> reader;

            public int RowsCountInvocations { get; private set; }

            public int ColumnsCountInvocations { get; private set; }

            public int ElementsCountInvocations { get; private set; }

            public int ReadRowsInvocations { get; private set; }

            public SparseMatrixReaderWithMemberInvocationCounters(ISparseMatrixReader<double> reader)
            {
                if (reader == null)
                {
                    throw new ArgumentNullException("reader");
                }
                this.reader = reader;
            }

            #region Implementation of ISparseMatrixReader<double>

            public int RowsCount
            {
                get
                {
                    this.RowsCountInvocations++;
                    return reader.RowsCount;
                }
            }

            public int ColumnsCount
            {
                get
                {
                    this.ColumnsCountInvocations++;
                    return reader.ColumnsCount;
                }
            }

            public long ElementsCount
            {
                get
                {
                    this.ElementsCountInvocations++;
                    return reader.ElementsCount;
                }
            }

            public IEnumerable<SparseVector<double>> ReadRows()
            {
                this.ReadRowsInvocations++;
                return reader.ReadRows();
            }

            #endregion
        }

        #endregion

    }
}
