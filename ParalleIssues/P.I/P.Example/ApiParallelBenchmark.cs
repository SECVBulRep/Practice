using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;

namespace P.Example;

[InProcess()]
public class ApiParallelBenchmark
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private const int TaskCount = 1000;


    [Benchmark()]
    public async Task<List<int>> ForEachVersion()
    {
        var list = new List<int>();
    
        var tasks = Enumerable.Range(0, 1000).Select(_ => new Func<Task<int>>(() => GetRandom(_httpClient))).ToList();
    
        foreach (var taks in tasks)
        {
            list.Add(await taks());
        }
    
        return list;
    }
    
    [Benchmark()]
    public List<int> UnlimParallelVersion() => ParallelVersion(-1);
    
    [Benchmark()]
    public List<int> LimimtedParallelVersion() => ParallelVersion(4);
    
    
    public List<int> ParallelVersion(int maxDegreesOfParallelism)
    {
        var list = new List<int>();
    
        var tasks = Enumerable.Range(0, 1000)
            .Select(_ => new Func<int>(() => GetRandom(_httpClient).GetAwaiter().GetResult())).ToList();
    
        Parallel.For(0, tasks.Count, new ParallelOptions
        {
            MaxDegreeOfParallelism = maxDegreesOfParallelism
        }, i => list.Add(tasks[i]()));
    
        return list;
    }
    
    [Benchmark()]
    public async Task<int[]> WhenAllVersion()
    {
        List<Task<int>> tasks = Enumerable.Range(0, 1000).Select(_ => GetRandom(_httpClient)).ToList();
    
        int[] list = await Task.WhenAll(tasks);
    
        return list;
    }
    
    [Benchmark()]
    public async Task<List<int>> AsyncParalVersion1() => await AsyncParalVersion(1);
    
    
    [Benchmark()]
    public async Task<List<int>> AsyncParalVersion10() => await AsyncParalVersion(10);

    
    [Benchmark()]
    public async Task<List<int>> AsyncParalVersion100() => await AsyncParalVersion(200);

    
    public async Task<List<int>> AsyncParalVersion(int batches)
    {
        var list = new List<int>();

        var tasks = Enumerable.Range(0, 1000).Select(_ => new Func<Task<int>>(() => GetRandom(_httpClient))).ToList();


        await ParallelForEachAsync(tasks, batches, async func =>
        {
            list.Add(await func());
        });
        
        return list;
    }
    
   
    
    
    
    public static Task ParallelForEachAsync<T>(
        IEnumerable<T> source,
        int degreeParallelism,
        Func<T, Task> body)
    {
        async Task AwaitPartition(IEnumerator<T> partition)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    await body(partition.Current);
                }
            }
        }

        return Task.WhenAll(
            Partitioner.
                Create(source)
                .GetPartitions(degreeParallelism)
                .AsParallel()
                .Select(AwaitPartition)
        );
    }

    public static async Task<int> GetRandom(HttpClient client)
    {
        int response = Convert.ToInt32(await client.GetStringAsync("http://localhost:8080/"));
        return response;
    }
}