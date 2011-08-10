namespace NonnegativeMatrixFactorization.Tests
{
    using System;
    using System.Diagnostics;

    using MachineLearning.Collections.Array;
    using MachineLearning.NonnegativeMatrixFactorization;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TestHelpers;

    [TestClass]
    public class NMFTests
    {
        [TestMethod]
        public void FactorizeTestMethod1()
        {
            // arrange
            var reader = new InMemorySparseMatrixReader(
                new SparseVector<double> { { 0, 22.0 }, { 1, 28.0 } },
                new SparseVector<double> { { 0, 49.0 }, { 1, 64.0 } });
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

        #endregion
    }
}
