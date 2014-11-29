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

            sw.Start();
            var s = slugService.GetSlugs(generation: 1);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + " Elapsed Milliseconds");
            bool allMatch = false;
            allMatch = s.Select(sl => sl.FavouritePrime).Distinct().Count() == 1;
            Console.WriteLine(allMatch + " - all slugs have the same prime (" + s[0].FavouritePrime + ")");


            sw.Reset();
            sw.Start();
            var a = slugService.GetSlugsAsync(gen: 1).Result;
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + " Elapsed Milliseconds (async)");
            allMatch = a.Select(sl => sl.FavouritePrime).Distinct().Count() == 1;
            Console.WriteLine(allMatch + " - all slugs have the same prime (" + a[0].FavouritePrime + ")");

            Console.ReadLine();
        }
    }
}
