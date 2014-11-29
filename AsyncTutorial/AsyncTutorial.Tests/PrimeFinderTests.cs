namespace AsyncTutorial.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PrimeFinderTests
    {
        [TestMethod]
        public void FindNthPrime_FirstPrime_Gets2()
        {
            PrimeFinder primeFinder = new PrimeFinder();

            Int64 result = primeFinder.FindNthPrime(1);

            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void FindNthPrime_SecondPrime_Gets3()
        {
            PrimeFinder primeFinder = new PrimeFinder();

            Int64 result = primeFinder.FindNthPrime(2);

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void FindNthPrime_FifthPrime_Gets11()
        {
            PrimeFinder primeFinder = new PrimeFinder();

            Int64 result = primeFinder.FindNthPrime(5);

            Assert.AreEqual(11, result);
        }

        [TestMethod]
        public void FindNthPrime_1005thPrime_GetsResult()
        {
            PrimeFinder primeFinder = new PrimeFinder();

            Int64 result = primeFinder.FindNthPrime(1005);

            Assert.AreEqual(7951, result);
        }

        [TestMethod]
        public void FindNthPrime_10000thPrime_GetsResult()
        {
            PrimeFinder primeFinder = new PrimeFinder();

            Int64 result = primeFinder.FindNthPrime(10000);

            Assert.AreEqual(104729, result);
        }

        [TestMethod]
        public void FindNthPrime_LargeIndexPrime_GetsResult()
        {
            PrimeFinder primeFinder = new PrimeFinder();

            Int64 result = primeFinder.FindNthPrime(50000);

            Assert.IsNotNull(result);
        }
    }
}
