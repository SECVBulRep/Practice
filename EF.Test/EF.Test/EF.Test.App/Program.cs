// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using EF.Test.App;

Console.WriteLine("Hello, World!");

 //  Benchmarks test = new Benchmarks();
 // await test.Setup();
 // var temp =   test.GetAuthors();
// var temp2 =   test.GetAuthorsOptimized();
//  var temp3=   await  test.GetAuthorsOptimizedCompiled();
//var temp3=   await  test.EF_Query_Filter();

 // Console.WriteLine(temp.Count);

BenchmarkRunner.Run<Benchmarks>();