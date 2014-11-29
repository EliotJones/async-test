namespace AsyncTutorial
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Linq;

    public class SlugGetter
    {
        public IList<Slug> GetDataSlugs(int generation)
        {
            ConcurrentBag<Slug> slugs = new ConcurrentBag<Slug>();

            Parallel.For(0, 1000, i =>
             {
                 slugs.Add(new Slug
                 {
                     Id = i + 1,
                     Name = "Slug" + i.ToString(),
                     Generation = generation,
                     CabbagesEaten = (i % 2 == 0) ? 1 : 0
                 });
                 Thread.Sleep(5);
             });

            return slugs.ToList();
        }
    }
}
