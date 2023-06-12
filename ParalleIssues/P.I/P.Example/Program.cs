// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using P.Example;

BenchmarkRunner.Run<ApiParallelBenchmark>();

//  HttpClient _httpClient = new HttpClient();
//  var list = new List<int>();
//  
// var tasks = Enumerable.Range(0, 1000)
//     .Select(_ => new Func<int>(() => GetRandom(_httpClient).GetAwaiter().GetResult())).ToList();
//
// Parallel.For(0, tasks.Count, i => list.Add(tasks[i]()));
//
//  static async Task<int> GetRandom(HttpClient client)
//  {
//      int response = Convert.ToInt32(await client.GetStringAsync("http://localhost:8080/"));
//      return response;
//  }
//  
//  Console.ReadKey();