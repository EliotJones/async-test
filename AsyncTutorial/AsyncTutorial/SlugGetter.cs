namespace AsyncTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class SlugGetter
    {
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
            }

            return slugs;
        }
    }
}
