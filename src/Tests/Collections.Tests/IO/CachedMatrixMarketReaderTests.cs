namespace Collections.Tests.IO
{
    using System;

    using MachineLearning.Collections.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CachedMatrixMarketReaderTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsArgumentNullExceptionForNullReaderArgument()
        {
            // arrange
            
            // act
            var cachedReader = new CachedMatrixMarketReader<double>(null);

            // assert
            Assert.Fail();
        }
    }
}
