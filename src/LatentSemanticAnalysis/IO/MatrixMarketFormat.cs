namespace MachineLearning.LatentSemanticAnalysis.IO
{
    internal class MatrixMarketFormat
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