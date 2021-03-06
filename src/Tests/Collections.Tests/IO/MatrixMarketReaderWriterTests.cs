﻿namespace Collections.Tests.IO
{
    using System.IO;
    using System.Linq;

    using Collections.Tests.Helpers;

    using MachineLearning.Collections.Array;
    using MachineLearning.Collections.IO;

    using NUnit.Framework;

    using TestHelpers;

    [TestFixture]
    public class MatrixMarketReaderWriterTests
    {
        [Test]
        public void MatrixMarketReaderReadSameVectorsAsMatrixMarketWriterWrited()
        {
            // arrange
            const int rows = 100;
            const int columns = 100;
            const double density = 0.01;

            var originalVectors = SparseVectorHelper.GenerateSparseVectors(rows, columns, density, () => SparseVectorHelper.RandomInInterval(-100, 100, 2)).ToArray();
            // act

            //      write
            byte[] memory;
            int rowsWrited;
            int columnsWrited;
            long elementsWrited;
            using (var stream = new MemoryStream())
            using (var writer = new MatrixMarketWriter<double>(stream))
            {
                writer.Write(originalVectors);
                stream.Flush();

                memory = stream.ToArray();
                rowsWrited = writer.RowsCount;
                columnsWrited = writer.ColumnsCount;
                elementsWrited = writer.ElementsCount;
            }

            //      read
            SparseVector<double>[] readedVectors;
            int rowsToRead;
            int columnsToRead;
            long elementToRead;
            using (var stream = new MemoryStream(memory, false))
            using (var reader = new MatrixMarketReader<double>(stream))
            {
                rowsToRead = reader.RowsCount;
                columnsToRead = reader.ColumnsCount;
                elementToRead = reader.ElementsCount;
                readedVectors = reader.ReadRows().ToArray();
            }

            string text;
            using (var stream = new MemoryStream(memory, false))
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }            

            // assert
            Assert.AreEqual(rowsWrited, rowsToRead, string.Format("Rows count does not equal:\r\n{0}", text));
            Assert.AreEqual(columnsWrited, columnsToRead, string.Format("Columns count does not equal:\r\n{0}", text));
            Assert.AreEqual(elementsWrited, elementToRead, string.Format("Elements count does not equal:\r\n{0}", text));
            Assert.IsTrue(originalVectors.SequenceEqual(readedVectors), string.Format("Vectors do not equal:\r\n{0}", text));
        }
    }
}
