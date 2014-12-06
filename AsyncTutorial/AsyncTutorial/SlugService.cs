namespace AsyncTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    public class SlugService
    {
        private PrimeFinder primeFinder = new PrimeFinder();
        private SlugGetter slugGetter = new SlugGetter();

        public IList<Slug> GetSlugsAsyncCaller(int gen)
        {
            var task = GetSlugsAsync(gen);
            Console.WriteLine("Returned from async");
            Thread.Sleep(1000);
            return task.Result;
        }

        public async Task<IList<Slug>> GetSlugsAsync(int gen)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Int64 generationalPrime = await primeFinder.FindNthPrimeAsync(50000 + gen);

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + " Milliseconds getting the nth prime (async)");
            sw.Reset();
            sw.Start();

            IList<Slug> slugs = slugGetter.GetDataSlugs(gen);

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + " Milliseconds getting the slugs from data storage (async)");

            foreach (var slug in slugs)
            {
                slug.FavouritePrime = generationalPrime;
            }

            return slugs;
        }

        public IList<Slug> GetSlugsParallel(int gen)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Task<Int64> generationalPrimeTask = primeFinder.FindNthPrimeTask(50000 + gen);

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + " Milliseconds getting the nth prime (parallel)");
            sw.Reset();
            sw.Start();

            IList<Slug> slugs = slugGetter.GetDataSlugs(gen);

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + " Milliseconds getting the slugs from data storage (parallel)");

            Int64 generationalPrime = generationalPrimeTask.Result;
            foreach (var slug in slugs)
            {
                slug.FavouritePrime = generationalPrime;
            }

            return slugs;
        }

        public IList<Slug> GetSlugs(int generation)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Int64 generationalPrime = primeFinder.FindNthPrime(50000 + generation);
            
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + " Milliseconds getting the nth prime");
            sw.Reset();
            sw.Start();

            IList<Slug> slugs = slugGetter.GetDataSlugs(generation);

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + " Milliseconds getting the slugs from data storage");

            foreach (var slug in slugs)
            {
                slug.FavouritePrime = generationalPrime;
            }

            return slugs;
        }
    }
}
