namespace Collections.Tests
{
    using MachineLearning.Collections;
    using MachineLearning.Collections.Array;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MemoryMappedArrayTests
    {
        [TestMethod]
        public void OneDimensionConstructorDoesNotThrowException()
        {
            const int length = 10000;
            using (var array = new MemoryMappedArray<double>(length))
            {
            }
        }

        [TestMethod]
        public void TwoDimensionConstructorDoesNotThrowException()
        {
            const int size0 = 500;
            const int size1 = 20;
            using (var array = new MemoryMappedArray<double>(size0, size1))
            {
            }
        }

        [TestMethod]
        public void OneDimensionArrayReturnsCorrectSizesAndCount()
        {
            const int length = 10000;
            using (var array = new MemoryMappedArray<double>(length))
            {
                Assert.AreEqual(length, array.Count);
                Assert.AreEqual(1, array.Size0);
                Assert.AreEqual(length, array.Size1);
            }
        }

        [TestMethod]
        public void TwoDimensionArrayReturnsCorrectSizesAndCount()
        {
            const int size0 = 500;
            const int size1 = 20;
            using (var array = new MemoryMappedArray<double>(size0, size1))
            {
                Assert.AreEqual(size0 * size1, array.Count);
                Assert.AreEqual(size0, array.Size0);
                Assert.AreEqual(size1, array.Size1);
            }
        }

        [TestMethod]
        public void OneDimensionIndexedPropertyWorksCorrectly()
        { 
            const int length = 10000;
            using (var array = new MemoryMappedArray<double>(length))
            {
                for (int i = 0; i < length; i++)
                {
                    array[i] = i;
                }

                for (int i = 0; i < length; i++)
                {
                    Assert.AreEqual(i, array[i]);
                }
            }
        }

        [TestMethod]
        public void TwoDimensionIndexedPropertyWorksCorrectly()
        {
            const int size0 = 500;
            const int size1 = 20;
            using (var array = new MemoryMappedArray<double>(size0, size1))
            {
                for (int i = 0; i < size0; i++)
                {
                    for (int j = 0; j < size1; j++)
                    {
                        array[i, j] = (long)i * size1 + j;
                    }
                }

                for (int i = 0; i < size0; i++)
                {
                    for (int j = 0; j < size1; j++)
                    {
                        double v = (double)i * size1 + j;
                        Assert.AreEqual(v, array[i, j]);
                    }
                }
            }
        }
    }
}
