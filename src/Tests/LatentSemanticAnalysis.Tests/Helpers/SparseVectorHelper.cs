﻿namespace LatentSemanticAnalysis.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MachineLearning.LatentSemanticAnalysis;

    static class SparseVectorHelper
    {
        private static readonly Random Rnd = new Random();

        public static Dictionary<int, double> GenerateRandomVector(int count, double zeroProbability)
        {
            return Enumerable.Range(0, count)
                .Select(i => new
                    {
                        Index = i,
                        Value = Rnd.NextDouble() < zeroProbability ? 0.0 : Math.Round(IntervalToInterval(Rnd.NextDouble(), 0, 1, 0.01, 100) * (Rnd.NextDouble() < 0.5 ? -1 : 1), 2, MidpointRounding.AwayFromZero)
                    })
                .ToDictionary(arg => arg.Index, arg => arg.Value);
        }

        public static SparseVector<double> GenerateRandomSparseVector(int count, double zeroProbability)
        {
            return new SparseVector<double>(GenerateRandomVector(count, zeroProbability));
        }

        private static double IntervalToInterval(double x, double x1, double x2, double y1, double y2)
        {
            return (x - x1) / (x2 - x1) * (y2 - y1) + y1;
        }
    }
}
