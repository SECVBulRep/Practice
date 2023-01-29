// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using StackExchange.Redis;
using Test;

var summary = BenchmarkRunner.Run(typeof(Benchy));

Console.WriteLine("Hello, World!!!!");
 


