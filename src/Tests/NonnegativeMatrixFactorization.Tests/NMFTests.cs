namespace NonnegativeMatrixFactorization.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using MachineLearning.Collections.Array;
    using MachineLearning.Collections.IO;
    using MachineLearning.NonnegativeMatrixFactorization;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NMFTests
    {
        [TestMethod]
        public void FactorizeTestMethod1()
        {
            // arrange
            var reader = FromVectors(new SparseVector<double>{{0, 22.0},{1, 28.0}},
                                     new SparseVector<double>{{0, 49.0},{1, 64.0}});
            var nmf = new NMF(reader);
            
            // act
            var sw = Stopwatch.StartNew();
            using (var factorization = nmf.Factorize(2, 100))
            {
                sw.Stop();

                // assert
                Print("W", factorization.W);
                Print("H", factorization.H);
                Console.WriteLine("Factorization Euclidean distance: " + factorization.EuclideanDistance);
                Console.WriteLine("Time: " + sw.ElapsedMilliseconds + " ms.");
            }
        }

        #region Helpers

        private void Print(string arrayName, IArray<double> array)
        {
            Console.WriteLine(arrayName + ":");
            for (int i = 0; i < array.Size0; i++)
            {
                for (int j = 0; j < array.Size1; j++)
                {
                    Console.Write(array[i,j] + " ");
                }
                Console.WriteLine();
            }
        }

        private ISparseMatrixReader<double> FromVectors(params SparseVector<double>[] rows)
        {
            return new InMemorySparseMatrix(rows);
        }

        class InMemorySparseMatrix : ISparseMatrixReader<double>
        {
            private readonly SparseVector<double>[] rows;

            private readonly int columnsCount;

            private readonly int elementsCount;

            public InMemorySparseMatrix(SparseVector<double>[] rows)
            {
                if (rows == null)
                {
                    throw new ArgumentNullException("rows");
                }
                this.rows = rows;

                columnsCount = (from r in rows from e in r select e.Key).Max() + 1;
                elementsCount = (from r in rows select r.NonZeroValuesCount).Sum();
            }

            #region Implementation of ISparseMatrixReader

            public int RowsCount
            {
                get
                {
                    return rows.Length;
                }
            }

            public int ColumnsCount
            {
                get
                {
                    return columnsCount;
                }
            }

            public long ElementsCount
            {
                get
                {
                    return elementsCount;
                }
            }

            public IEnumerable<SparseVector<double>> ReadRows() 
            {
                return rows;
            }

            #endregion
        }

        #endregion

    }
}
