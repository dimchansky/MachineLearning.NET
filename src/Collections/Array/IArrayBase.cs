namespace MachineLearning.Collections.Array
{
    public interface IArrayBase
    {
        /// <summary>
        /// Returns the size of the first dimension.
        /// </summary>
        /// <value>The size of the first dimension.</value>
        int Size0 { get; }

        /// <summary>
        /// Returns the size of the second dimension.
        /// </summary>
        /// <value>The size of the second dimension.</value>
        int Size1 { get; }

        long NumElements { get; }
    }
}