namespace AsyncTutorial
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class SlugGetter
    {
        public IList<Slug> GetDataSlugsParallel(int generation)
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

        public IList<Slug> GetDataSlugs(int generation)
        {
            List<Slug> slugs = new List<Slug>();

            for (int i = 0; i < 1000; i++)
            {
                slugs.Add(new Slug
                {
                    Id = i + 1,
                    Name = "Slug" + i.ToString(),
                    Generation = generation,
                    CabbagesEaten = (i % 2 == 0) ? 1 : 0
                });
                Thread.Sleep(5);
            };

            return slugs;
        }
    }
}
