# C# Async for Slugs #

Fun fact for you dear reader, I've not been programming for long, my degree was in Chemistry and though I taught myself a little programming during my degree I was mainly focused on Chemistry. This means I've been programming professionally for less time than C# 5 has been released.

This is why I've found tutorials so far for the Async/Await features in C# 5 (Net 4.5) to be a little lacking, they generally assume the reader has been writing async programs prior to the release of the features.

As a solution to this I've endeavoured to write a guide to async for miserable slugs like myself who have not had exposure to async prior to these keywords.

### Demo Scenario

We're going to stick with a very simple console app available [on GitHub][link0] to make sure all the concepts can be understood by a slug.

Our ```Program.cs``` main method is shown below:

	public static void Main(string[] args)
	{	
	    SlugService slugService = new SlugService();
	    slugService.GetSlugs(generation: 1);
	}

The GitHub version contains a lot more lines which allow the method calls to be timed with the built in stopwatch.

The Slug Service is tasked with retrieving the records of some slugs we've stored somewhere. Let's see what it does:

	public IList<Slug> GetSlugs(int generation)
	{
	    Int64 generationalPrime = primeFinder.FindNthPrime(50000 + generation);
	
	    IList<Slug> slugs = slugGetter.GetDataSlugs(generation);
	
	    foreach (var slug in slugs)
	    {
	        slug.FavouritePrime = generationalPrime;
	    }
	
	    return slugs;
	}

Again, the GitHub version contains more code related to timing methods with Stopwatch. This method does the following things:

1. Gets the favourite prime for this generation of slugs. (As we all know, slugs are huge prime number fans).
2. Get the slugs from our storage medium.
3. Set the prime for this generation of slugs.

The prime finder gets the **nth** prime for any n. It's written to be quite inefficient so we can replicate some external call or large calculation that blocks the program.

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
	
	        if (currentNumberIsPrime) primes.Add(index);
	        index += 2;
	    }
	
	    return primes[n-1];
	}

If you don't understand the maths just ignore it, suffice to say when you call this method with an input of ~50,000 it takes around 8 seconds to return a result.

Finally we need to get our slugs from our data storage. I've not actually implemented a persistence medium for this demo so the SlugGetter does this:

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

The thread sleep in this method slows everything down so the program runs on a timescale which can be easily comprehended.

### The Problem with Synchronous Execution

Running the program with the Stopwatch related lines included gives this output:

![9219 Milliseconds getting prime, 5663 milliseconds getting slugs, 14884 overall](http://eliot-jones.com/Images/asynctutorial/SyncRun.png)

The total run time is almost entirely due to the time taken to calculate the prime and the time taken to retrieve our slugs. The rest of the program takes about 2 milliseconds to run!

Unfortunately in the normal synchronous way of calling methods each method is run in turn causing our program to take a long time.

To visually illustrate the call stack I created this diagram:

![The SlugService first waits 9.2 seconds for PrimeFinder then 5.7s for SlugGetter](http://eliot-jones.com/Images/asynctutorial/SyncCallStack.png)

By boosting the hardware of our computer we might achieve a more speedy slug retrieval (through a faster primefinder) however there's a limit to scaling up in this way. We need to move execution on to other threads so we're not being blocked by the PrimeFinder.

### Parallel Execution != Async

This is the first conceptual hurdle for people new to C# wanting to learn about async and await. Since all examples of async use methods which return ```Task<T>``` it's easy to assume ```Task<T>``` is related to the async features.

```Task<T>``` and related concepts in the ```System.Threading.Tasks``` namespace have actually been features of .NET since .NET 4.0. They form the core of the TPL (*Task Parallel Library*). We can therefore rewrite our SlugService with a parallel execution mode.

It would be nice if we could run the PrimeFinder on one of our cores in a worker task while the main execution ran the SlugGetter.

To create a Task for our PrimeFinder we can wrap our normal method in a new Task:

	public Task<Int64> FindNthPrimeTask(int n)
	{
	    return Task.Run(() => FindNthPrime(n));
	}

```Task.Run()``` instantiates a new Task object and starts it running, it is a slightly nicer version of the .NET 4 TPL ```Task.Factory.StartNew()```. These both provide a shortcut for creating a new Task and then starting it:

	Task t = new Task(() => DoSomething());
	t.Start();

Our method call now returns a Task so in our slug service we need to change the result of the PrimeFinder call to a ```Task<Int64>``` object:

	public IList<Slug> GetSlugsParallel(int gen)
	{
	    Task<Int64> generationalPrimeTask = primeFinder.FindNthPrimeTask(50000 + gen);
	
	    IList<Slug> slugs = slugGetter.GetDataSlugs(gen);
	
	    Int64 generationalPrime = generationalPrimeTask.Result;
	    foreach (var slug in slugs)
	    {
	        slug.FavouritePrime = generationalPrime;
	    }
	    return slugs;
	}

Where we call ```Task.Result``` the main thread blocks execution while waiting for the results of the Task. This means we avoid a **Race Condition** where we use our prime before it has been properly calculated.

We will run the synchronous single threaded ```GetSlugs``` and then the ```GetSlugsParallel``` method:

![53 Milliseconds getting prime, 6002 milliseconds getting slugs, 8959 overall for parallel](http://eliot-jones.com/Images/asynctutorial/ParallelRun.png)

As the results show, the run-time was almost entirely due to the time taken to calculate the prime (53 milliseconds was spent calling and returning the task). With the prime calculation taking place on another thread we were able to reduce run time.


It gets a bit harder to illustrate this call stack. First SlugService is called normally and performs a call to ```PrimeFinder``` as usual. However this returns in 50 milliseconds and the main thread goes on to run the ```SlugGetter``` which takes 6 seconds to return a result. 

![The SlugService first waits 9.2 seconds for PrimeFinder then 5.7s for SlugGetter](http://eliot-jones.com/Images/asynctutorial/ParallelCallStack.png)

In the meantime a worker thread (in orange) runs the prime finder which takes about 9 seconds to run. This is running while the main thread is running SlugGetter. Therefore rather than adding runtimes together as we did for synchronous execution we take the highest time (this isn't always the case but works for this example).

### Time for Async

So we've seen how we can use parallel execution without the async/await keywords. Let's introduce them to our program now.

For our ```PrimeFinder``` we create a wrapper for our method which returns a task just like we did for the Parallel version:

	public Task<Int64> FindNthPrimeAsync(int n)
	{
	    return Task.Run(() => FindNthPrime(n));
	}

In ```SlugService``` we create a method with the async keyword in the method declaration. Async methods must return one of the following return types:
 
+ ```Task<T>```
+ ```Task```
+ ```void```

This is achieved by changing the body of the method as follows:

	public async Task<IList<Slug>> GetSlugsAsync(int gen)
	{
	    Int64 generationalPrime = await primeFinder.FindNthPrimeAsync(50000 + gen);
	
	    IList<Slug> slugs = slugGetter.GetDataSlugs(gen);

	    foreach (var slug in slugs)
	    {
	        slug.FavouritePrime = generationalPrime;
	    }
	
	    return slugs;
	}

This is a bit confusing, the method is declared to return ```Task<IList<Slug>>``` but the method body returns ```IList<Slug>```. This is because the compiler can be thought of as wrapping the method up into a Task behind the scenes.

We have used await in the body of the method to tell the async method to wait for the Prime Finder to complete.

Because [Console Applications cannot have an async main method][link1] we have to 'unwrap' our async call at some point. To do this I created another method in ```SlugService``` that main actually calls to, Stephen Cleary outlines how to do this properly for console apps in the link but I'm using it here to illustrate a point:

	public IList<Slug> GetSlugsAsyncCaller(int gen)
	{
	    var task = GetSlugsAsync(gen);
	    Console.WriteLine("Returned from async");
		Thread.Sleep(1000);
	    return task.Result;
	}

This time the console output looks like this where async is run first, then synchronous and finally parallel execution:

![8946 Milliseconds getting prime, 5598 milliseconds getting slugs, 14548 overall for async](http://eliot-jones.com/Images/asynctutorial/AsyncRun.png)

As the results show, the async execution (14.5 seconds) takes just as long as the synchronous execution. But we're dealing with tasks, what gives?

Async works in a quite different way to parallel execution and async execution is not necessarily parallel. Let's look at a call stack diagram that attempts to explain this (again the blue arrows illustrate the main thread).

![The SlugService returns when it hits await. It then executes fully for Result](http://eliot-jones.com/Images/asynctutorial/AsyncCallStack.png)

This is all terribly confusing. Basically at the point the code execution hits ```await``` in ```GetSlugsAsync``` it **returns to the caller**, in this case ```GetSlugsAsyncCaller```, this is illustrated with the thicker blue arrow. The execution of that method (GetSlugsAsyncCaller) continues. It is then blocked by the call to result and the rest of the ```GetSlugsAsync``` method executes in the normal way.

Let's reiterate that; ```await``` **returns execution to the caller** and the caller continues to execute normally. Because we started the PrimeFinder task this executes in Parallel while we're back in the GetSlugsAsyncCaller method which is why the Thread.Sleep(1000) in that method doesn't increase the execution time  So what's the benefit of this?

### Purpose of Async



[link0]:https://github.com/EliotJones/async-test/tree/VS2013Solution/AsyncTutorial
[link1]:http://blog.stephencleary.com/2012/02/async-console-programs.html