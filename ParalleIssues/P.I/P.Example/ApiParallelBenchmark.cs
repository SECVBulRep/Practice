using BenchmarkDotNet.Attributes;

namespace P.Example;

[MemoryDiagnoser()]
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
    public  List<int> ParallelVersion()
    {
        var list = new List<int>();

        var tasks = Enumerable.Range(0, 1000)
            .Select(_ => new Func<int>(() => GetRandom(_httpClient).GetAwaiter().GetResult())).ToList();

        Parallel.For(0, tasks.Count, i => list.Add(tasks[i]()));

        return list;
    }

    public static async Task<int> GetRandom(HttpClient client)
    {
        int response = Convert.ToInt32(await client.GetStringAsync("http://localhost:8080/"));
        return response;
    }
}