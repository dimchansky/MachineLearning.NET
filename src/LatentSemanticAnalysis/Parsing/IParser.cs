namespace MachineLearning.LatentSemanticAnalysis.Parsing
{
    using System.Collections.Generic;

    public interface IParser
    {
        IEnumerable<string> Parse(string text);
    }
}
