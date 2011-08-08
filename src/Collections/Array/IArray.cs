namespace MachineLearning.Collections.Array
{
    public interface IArray<T> : IArrayBase
        where T : struct
    {
        T this[int index] { get; set; }

        T this[int index0, int index1] { get; set; }

        /// <summary>
        /// Transposes the 1st and 2nd dimensions of the array, making a shallow copy.
        /// </summary>
        /// <returns>Shallow copy of the transposed array.</returns>
        IArray<T> Transpose();
    }
}