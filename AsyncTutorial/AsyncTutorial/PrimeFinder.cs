namespace AsyncTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class PrimeFinder
    {
        public async Task<Int64> FindNthPrimeAsync(int n)
        {
            return await Task.Factory.StartNew(() => FindNthPrime(n));
        }

        public Int64 FindNthPrime(int n)
        {
            if (n < 1) throw new ArgumentException();

            List<int> primes = new List<int> { 2 };

            bool primeFound = false;
            int index = 3;

            while (!primeFound)
            {
                if (primes.Count >= n)
                {
                    primeFound = true;
                    break;
                }

                bool currentNumberIsPrime = true;

                foreach (int prime in primes)
                {
                    currentNumberIsPrime = index % prime != 0;
                    if (!currentNumberIsPrime) break;
                }

                if (currentNumberIsPrime)
                {
                    primes.Add(index);
                }

                index += 2;
            }

            return primes[n-1];
        }
    }
}
