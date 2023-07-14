// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using OptimizeMe.App;

// Benchmarks test = new Benchmarks();
// await test.Setup();
// test.GetAuthors();

BenchmarkRunner.Run<Benchmarks>();