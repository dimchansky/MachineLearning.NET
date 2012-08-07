namespace Collections.Tests.IO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Collections.Tests.Helpers;

    using MachineLearning.Collections.Array;
    using MachineLearning.Collections.IO;

    using NUnit.Framework;

    using TestHelpers;

    [TestFixture]
    public class CachedMatrixMarketReaderTests
    {
        [Test]
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

        [Test]
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

        [Test]
        public void InitializedCachedMatrixMarketReaderReturnsTheSamePropertiesValuesForEmptyRowsArray()
        {
            // arrange
            var reader = new InMemorySparseMatrixReader();

            using (var cachedReader = new CachedMatrixMarketReader<double>(reader))
            {
                // act            

                // assert
                Assert.AreEqual(reader.ColumnsCount, cachedReader.ColumnsCount);
                Assert.AreEqual(reader.RowsCount, cachedReader.RowsCount);
                Assert.AreEqual(reader.ElementsCount, cachedReader.ElementsCount);
            }

        }

        [Test]
        public void ReadRowsReturnSameVectorsInFirstScan()
        {
            // arrange
            const int rows = 100;
            const int columns = 100;
            const double density = 0.01;

            var originalVectors = SparseVectorHelper.GenerateSparseVectors(rows, columns, density, () => SparseVectorHelper.RandomInInterval(-100, 100, 2)).ToArray();
            var originalReader = new InMemorySparseMatrixReader(originalVectors);
            
            using(var cachedReader = new CachedMatrixMarketReader<double>(originalReader))
            {
                // act
                var firstScanResults = originalReader.ReadRows().ZipFull(cachedReader.ReadRows(), (originalVector, cachedVector) => originalVector == cachedVector).All(b => b);

                // assert
                Assert.IsTrue(firstScanResults);
            }
        }

        [Test]
        public void ReadRowsReturnSameVectorsInFirstScanForEmptyRowsArray()
        {
            // arrange
            const int rows = 0;
            const int columns = 0;
            const double density = 0.01;

            var originalVectors = SparseVectorHelper.GenerateSparseVectors(rows, columns, density, () => SparseVectorHelper.RandomInInterval(-100, 100, 2)).ToArray();
            var originalReader = new InMemorySparseMatrixReader(originalVectors);

            using (var cachedReader = new CachedMatrixMarketReader<double>(originalReader))
            {
                // act
                var firstScanResults = originalReader.ReadRows().ZipFull(cachedReader.ReadRows(), (originalVector, cachedVector) => originalVector == cachedVector).All(b => b);

                // assert
                Assert.IsTrue(firstScanResults);
            }
        }

        [Test]
        public void ReadRowsReturnSameVectorsInSecondScan()
        {
            // arrange
            const int rows = 100;
            const int columns = 100;
            const double density = 0.01;

            var originalVectors = SparseVectorHelper.GenerateSparseVectors(rows, columns, density, () => SparseVectorHelper.RandomInInterval(-100, 100, 2)).ToArray();
            var originalReader = new InMemorySparseMatrixReader(originalVectors);

            using (var cachedReader = new CachedMatrixMarketReader<double>(originalReader))
            {
                // act
                foreach (var row in cachedReader.ReadRows())
                {                    
                }
                var secondScanResults = originalReader.ReadRows().ZipFull(cachedReader.ReadRows(), (originalVector, cachedVector) => originalVector == cachedVector).All(b => b);

                // assert
                Assert.IsTrue(secondScanResults);
            }
        }

        [Test]
        public void ReadRowsReturnSameVectorsInSecondScanForEmptyRowsArray()
        {
            // arrange
            const int rows = 0;
            const int columns = 0;
            const double density = 0.01;

            var originalVectors = SparseVectorHelper.GenerateSparseVectors(rows, columns, density, () => SparseVectorHelper.RandomInInterval(-100, 100, 2)).ToArray();
            var originalReader = new InMemorySparseMatrixReader(originalVectors);

            using (var cachedReader = new CachedMatrixMarketReader<double>(originalReader))
            {
                // act
                foreach (var row in cachedReader.ReadRows())
                {
                }
                var secondScanResults = originalReader.ReadRows().ZipFull(cachedReader.ReadRows(), (originalVector, cachedVector) => originalVector == cachedVector).All(b => b);

                // assert
                Assert.IsTrue(secondScanResults);
            }
        }

        [Test]
        public void ReadRowsInSecondScanDoNotUseOriginalReader()
        {
            // arrange
            const int rows = 100;
            const int columns = 100;
            const double density = 0.01;

            var originalVectors = SparseVectorHelper.GenerateSparseVectors(rows, columns, density, () => SparseVectorHelper.RandomInInterval(-100, 100, 2)).ToArray();
            var originalReader = new InMemorySparseMatrixReader(originalVectors);
            var originalReaderWithCounters = new SparseMatrixReaderWithMemberInvocationCounters<double>(originalReader);

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

        [Test]
        public void ReadRowsInSecondScanUseOriginalReaderIfFirstScanWasNotFull()
        {
            // arrange
            const int rows = 100;
            const int columns = 100;
            const double density = 0.01;

            var originalVectors = SparseVectorHelper.GenerateSparseVectors(rows, columns, density, () => SparseVectorHelper.RandomInInterval(-100, 100, 2)).ToArray();
            var originalReader = new InMemorySparseMatrixReader(originalVectors);
            var originalReaderWithCounters = new SparseMatrixReaderWithMemberInvocationCounters<double>(originalReader);

            using (var cachedReader = new CachedMatrixMarketReader<double>(originalReaderWithCounters))
            {
                // act
                foreach (var row in cachedReader.ReadRows().Take(1))
                {
                }
                foreach (var row in cachedReader.ReadRows())
                {
                }

                // assert
                Assert.AreEqual(1, originalReaderWithCounters.ColumnsCountInvocations);
                Assert.AreEqual(1, originalReaderWithCounters.RowsCountInvocations);
                Assert.AreEqual(1, originalReaderWithCounters.ElementsCountInvocations);
                Assert.AreEqual(2, originalReaderWithCounters.ReadRowsInvocations);
            }
        }

        [Test]
        public void ReadRowsInSecondScanDoNotUseOriginalReaderForEmptyRowsArray()
        {
            // arrange
            const int rows = 0;
            const int columns = 0;
            const double density = 0.01;

            var originalVectors = SparseVectorHelper.GenerateSparseVectors(rows, columns, density, () => SparseVectorHelper.RandomInInterval(-100, 100, 2)).ToArray(); 
            var originalReader = new InMemorySparseMatrixReader(originalVectors);
            var originalReaderWithCounters = new SparseMatrixReaderWithMemberInvocationCounters<double>(originalReader);

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

        private class SparseMatrixReaderWithMemberInvocationCounters<T> : ISparseMatrixReader<T>
            where T : struct, IEquatable<T>
        {
            private readonly ISparseMatrixReader<T> reader;

            public int RowsCountInvocations { get; private set; }

            public int ColumnsCountInvocations { get; private set; }

            public int ElementsCountInvocations { get; private set; }

            public int ReadRowsInvocations { get; private set; }

            public SparseMatrixReaderWithMemberInvocationCounters(ISparseMatrixReader<T> reader)
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

            public IEnumerable<SparseVector<T>> ReadRows()
            {
                this.ReadRowsInvocations++;
                return reader.ReadRows();
            }

            #endregion
        }

        #endregion
    }
}
