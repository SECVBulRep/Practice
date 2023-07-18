// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using OptimizeMe.App;

  // Benchmarks test = new Benchmarks();
  // await test.Setup();
  // var temp =  await test.GetAuthorsOpt();
  // var temp2 =  await test.EF_Query_Filter();

//Console.WriteLine(temp.Count);

 BenchmarkRunner.Run<Benchmarks>();