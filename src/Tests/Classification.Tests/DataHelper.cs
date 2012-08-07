namespace Classification.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using MachineLearning.Classification;
    using MachineLearning.Classification.Interfaces;
    using MachineLearning.Classification.Model;

    public static class DataHelper
    {
        public static IDataSet<bool, double> GetDataSet1()
        {
            return GetDataSet("data1.txt");
        }

        public static IDataSet<bool, double> GetDataSet2()
        {
            return GetDataSet("data2.txt");
        }

        private static IDataSet<bool, double> GetDataSet(string resourceName)
        {
            var dataLines = GetResourceLines(resourceName);

            var parsedDataLines = ParseDoublesLines(dataLines, new[] { ',' });

            var dataSet1 = InMemoryDataSet.Create(parsedDataLines
                                                      .Select(pl => TrainingSample.Create(pl[2] != 0, new[] { pl[0], pl[1] }))
                                                      .ToArray());
            return dataSet1;
        }

        private static IEnumerable<string> GetResourceLines(string txtResourceName)
        {
            var type = typeof(DataHelper);
            using (var stream = type.Assembly.GetManifestResourceStream(type.Namespace + "." + txtResourceName))
            {
                foreach (var p in GetStreamLines(stream)) yield return p;
            }
        }

        private static IEnumerable<double[]> ParseDoublesLines(IEnumerable<string> dataLines, char[] separator)
        {
            return dataLines.Select(
                line => line.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                            .Select(v => Double.Parse(v, CultureInfo.InvariantCulture))
                            .ToArray());
        }

        private static IEnumerable<string> GetStreamLines(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        public static double[] MapTwoFeaturesToDegrees(double x1, double x2, int degree)
        {
            var res = new List<double>();
            for (var i = 1; i <= degree; i++)
            {
                for (var j = 0; j <= i; j++)
                {
                    res.Add(Math.Pow(x1, i - j) * Math.Pow(x2, j));
                }
            }

            return res.ToArray();
        }

        public static IDataSet<T, double> MapTwoFeaturesDataSetToDegree<T>(IDataSet<T, double> dataSet, int degree)
            where T : IEquatable<T>
        {
            return InMemoryDataSet.Create((from data in dataSet.GetData()
                                           let x1 = data.Attributes[0]
                                           let x2 = data.Attributes[1]
                                           select TrainingSample.Create(data.Category, MapTwoFeaturesToDegrees(x1, x2, degree), data.Count))
                                              .ToArray());
        }

        public static int TwoMappedFeaturesToDegreeCount(int degree)
        {
            return (degree * degree + 3 * degree) / 2;
        }
    }
}