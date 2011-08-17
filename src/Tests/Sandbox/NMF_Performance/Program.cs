namespace NMF_Performance
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using MachineLearning.Collections.IO;
    using MachineLearning.NonnegativeMatrixFactorization;

    using TestHelpers;

    class Program
    {
        static void Main(string[] args)
        {
            const string matrixFileName = @"c:\temp\matrix.txt";
            const int rows = 10000;
            const int columns = 1000;
            const double density = 0.01;

            const int featuresCount = 10;
            const int iterationsCount = 30;

            Console.WriteLine("Generating matrix {0}x{1} with avg. {2} non-zero elements ({3}%)...", rows, columns, (long)((double)rows * columns * density), density * 100.0);
            using (var file = new FileStream(matrixFileName, FileMode.Create))
            using (var mmwriter = new MatrixMarketWriter<double>(file))
            {
                var vectors = SparseVectorHelper.GenerateSparseVectors(rows, columns, density, () => SparseVectorHelper.RandomInInterval(0.01, 100, 2));
                mmwriter.Write(vectors);
            }

            Console.WriteLine("Matrix factorization (features=" + featuresCount + ", iterations=" + iterationsCount + ")...");
            using (var file = new FileStream(matrixFileName, FileMode.Open))
            using (var mmreader = new MatrixMarketReader<double>(file))
            using (var cachedReader = new CachedMatrixMarketReader<double>(mmreader))
            {
                var nmf = new NMF(cachedReader);

                // act
                var sw = Stopwatch.StartNew();
                using (var factorization = nmf.Factorize(featuresCount, iterationsCount))
                {
                    sw.Stop();                    
                    Console.WriteLine("Factorization time: " + sw.ElapsedMilliseconds + " ms.");

                    Console.WriteLine("Euclidean distance calculation...");
                    sw = Stopwatch.StartNew();
                    var euclideanDistance = nmf.GetEuclideanDistance(factorization);
                    sw.Stop();
                    Console.WriteLine("Euclidean distance calculation time: " + sw.ElapsedMilliseconds + " ms.");                   
                    Console.WriteLine("Factorization Euclidean distance: " + euclideanDistance);                    
                }
            }
        }
    }
}
