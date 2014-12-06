namespace AsyncTutorial
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    public class Program
    {
        public static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();

            SlugService slugService = new SlugService();
            bool allMatch = false;
            sw.Start();
            var a = slugService.GetSlugsAsyncCaller(gen: 1);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + " Elapsed Milliseconds");
            allMatch = false;
            allMatch = a.Select(sl => sl.FavouritePrime).Distinct().Count() == 1;
            Console.WriteLine(allMatch + " - all slugs have the same prime (" + a[0].FavouritePrime + ")");
            Console.WriteLine(Environment.NewLine);
            sw.Reset();


            sw.Start();
            var s = slugService.GetSlugs(generation: 1);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + " Elapsed Milliseconds");
            allMatch = false;
            allMatch = s.Select(sl => sl.FavouritePrime).Distinct().Count() == 1;
            Console.WriteLine(allMatch + " - all slugs have the same prime (" + s[0].FavouritePrime + ")");
            Console.WriteLine(Environment.NewLine);

            sw.Reset();
            sw.Start();
            var p = slugService.GetSlugsParallel(gen: 1);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + " Elapsed Milliseconds (parallel)");
            allMatch = p.Select(sl => sl.FavouritePrime).Distinct().Count() == 1;
            Console.WriteLine(allMatch + " - all slugs have the same prime (" + p[0].FavouritePrime + ")");

            Console.ReadLine();
        }
    }
}
