namespace ShoNMF_Performance
{
    using System;
    using System.Diagnostics;

    using ShoNS.Array;

    using ShoNonnegativeMatrixFactorization;

    class Program
    {
        static void Main(string[] args)
        {
            const int rows = 120000;
            const int columns = 300000;
            const double density = 0.00154874775953107590950927675241;

            const int featuresCount = 100;
            const int iterationsCount = 1;

            Console.WriteLine("Generating matrix {0}x{1} with avg. {2} non-zero elements ({3}%)...", rows, columns, (long)((double)rows * columns * density), density * 100.0);
            using (var sparseMatrix = new SparseDoubleArray(rows, columns))
            {
                sparseMatrix.FillRandom(new Random(), (float)density);

                Console.WriteLine("Generated non-zero elements: {0}", sparseMatrix.CountCells);

                Console.WriteLine("Matrix factorization (features=" + featuresCount + ", iterations=" + iterationsCount + ")...");

                var nmf = new ShoNMF(sparseMatrix);
                // act
                var sw = Stopwatch.StartNew();
                using (var factorization = nmf.Factorize(featuresCount, iterationsCount))
                {
                    sw.Stop();
                    Console.WriteLine("Factorization time: " + sw.ElapsedMilliseconds + " ms.");
                }
            }
        }
    }
}
