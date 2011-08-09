namespace NonnegativeMatrixFactorization.Tests
{
    using System;
    using System.Collections.Generic;
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
            using (var factorization = nmf.Factorize(1, 10000))
            {
                // assert
                Print("H", factorization.W);
                Print("W", factorization.H);
                Console.WriteLine("Factorization Euclidean distance: " + factorization.EuclideanDistance);
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

        private ISparseMatrixReader FromVectors(params SparseVector<double>[] rows)
        {
            return new InMemorySparseMatrix(rows);
        }

        class InMemorySparseMatrix : ISparseMatrixReader
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

            public IEnumerable<SparseVector<T>> ReadRows<T>() where T : struct, IEquatable<T>
            {
                if(typeof(T) != typeof(double))
                {
                    throw new NotSupportedException();
                }

                return (IEnumerable<SparseVector<T>>)(object)rows;
            }

            #endregion
        }

        #endregion

    }
}
