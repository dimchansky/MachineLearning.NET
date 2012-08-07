namespace Collections.Tests.Array
{
    using System.IO;

    using MachineLearning.Collections.Array;

    using NUnit.Framework;

    [TestFixture]
    public class MemoryMappedArrayTests
    {
        [Test]
        public void OneDimensionConstructorDoesNotThrowException()
        {
            const int length = 10000;
            using (var array = new MemoryMappedArray<double>(length))
            {
            }
        }

        [Test]
        public void TwoDimensionConstructorDoesNotThrowException()
        {
            const int size0 = 500;
            const int size1 = 20;
            using (var array = new MemoryMappedArray<double>(size0, size1))
            {
            }
        }

        [Test]
        public void OneDimensionArrayReturnsCorrectSizesAndCount()
        {
            const int length = 10000;
            using (var array = new MemoryMappedArray<double>(length))
            {
                Assert.AreEqual(length, array.NumElements);
                Assert.AreEqual(1, array.Size0);
                Assert.AreEqual(length, array.Size1);
            }
        }

        [Test]
        public void TwoDimensionArrayReturnsCorrectSizesAndCount()
        {
            const int size0 = 500;
            const int size1 = 20;
            using (var array = new MemoryMappedArray<double>(size0, size1))
            {
                Assert.AreEqual(size0 * size1, array.NumElements);
                Assert.AreEqual(size0, array.Size0);
                Assert.AreEqual(size1, array.Size1);
            }
        }

        [Test]
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

        [Test]
        public void OneDimensionConstructorCreateArrayInitializedWithDefaultValue()
        {
            const int length = 10000;
            using (var array = new MemoryMappedArray<double>(length))
            {
                for (int i = 0; i < length; i++)
                {
                    Assert.AreEqual(0, array[i]);
                }
            }
        }

        [Test]
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

        [Test]
        public void TwoDimensionConstructorCreateArrayInitializedWithDefaultValue()
        {
            const int size0 = 500;
            const int size1 = 20;
            using (var array = new MemoryMappedArray<double>(size0, size1))
            {
                for (int i = 0; i < size0; i++)
                {
                    for (int j = 0; j < size1; j++)
                    {
                        Assert.AreEqual(0, array[i, j]);
                    }
                }
            }
        }

        [Test]
        public void CreateNewCreatesOneDimensionalMatrixAndOpenCorrectlyReadIt()
        {
            const int length = 100;
            var tempFileName = Path.GetTempFileName();

            try
            {
                File.Delete(tempFileName);

                int size0;
                int size1;
                long count;
                using (var array = MemoryMappedArray<double>.CreateNew(tempFileName, length))
                {
                    size0 = array.Size0;
                    size1 = array.Size1;
                    count = array.NumElements;

                    for (int i = 0; i < length; i++)
                    {
                        array[i] = i;
                    }
                }

                using (var array = MemoryMappedArray<double>.Open(tempFileName))
                {
                    Assert.AreEqual(size0, array.Size0);
                    Assert.AreEqual(size1, array.Size1);
                    Assert.AreEqual(count, array.NumElements);

                    for (int i = 0; i < length; i++)
                    {
                        Assert.AreEqual(i, array[i]);
                    }                    
                }
            }
            finally
            {
                File.Delete(tempFileName);
            }
        }

        [Test]
        public void CreateNewCreatesTwoDimensionalMatrixAndOpenCorrectlyReadIt()
        {
            const int size0 = 500;
            const int size1 = 20;

            var tempFileName = Path.GetTempFileName();

            try
            {
                File.Delete(tempFileName);

                long count;
                using (var array = MemoryMappedArray<double>.CreateNew(tempFileName, size0, size1))
                {
                    count = array.NumElements;

                    for (int i = 0; i < size0; i++)
                    {
                        for (int j = 0; j < size1; j++)
                        {
                            array[i, j] = (long)i * size1 + j;
                        }
                    }
                }

                using (var array = MemoryMappedArray<double>.Open(tempFileName))
                {
                    Assert.AreEqual(size0, array.Size0);
                    Assert.AreEqual(size1, array.Size1);
                    Assert.AreEqual(count, array.NumElements);

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
            finally
            {
                File.Delete(tempFileName);
            }
        }

        [Test]
        public void TransposeCreatesTransposedArrayForOneDimensionArray()
        {
            const int length = 10000;
            using (var array = new MemoryMappedArray<double>(length))
            {
                for (int i = 0; i < length; i++)
                {
                    array[i] = i;
                }

                var transposed = array.Transpose();                

                Assert.AreEqual(length, transposed.NumElements);
                Assert.AreEqual(1, transposed.Size1);
                Assert.AreEqual(length, transposed.Size0);

                for (int i = 1; i < array.Size0; i++)
                {
                    for (int j = 1; i < array.Size1; j++)
                    {
                        Assert.AreEqual(array[i,j], transposed[j,i]);
                    }
                }
            }

        }

        [Test]
        public void TransposeCreatesTransposedArrayForTwoDimensionArray()
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

                var transposed = array.Transpose();

                Assert.AreEqual(array.NumElements, transposed.NumElements);
                Assert.AreEqual(size0, transposed.Size1);
                Assert.AreEqual(size1, transposed.Size0);

                for (int i = 0; i < size0; i++)
                {
                    for (int j = 0; j < size1; j++)
                    {
                        Assert.AreEqual(array[i, j], transposed[j, i]);
                    }
                }
            }
        }

    }
}
