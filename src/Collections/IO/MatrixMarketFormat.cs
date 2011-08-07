namespace MachineLearning.Collections.IO
{
    internal static class MatrixMarketFormat
    {
        public static string HeaderLine
        {
            get
            {
                return "%%MatrixMarket matrix coordinate real general";
            }
        }
    }
}