﻿// See https://aka.ms/new-console-template for more information

using System.Collections.Frozen;
using System.Collections.Immutable;
using BenchmarkDotNet.Running;
using F.App;

// var list  = new List<int>{1,2,2,3};
//
// // list.Add();
// // list.Remove() и тд и тп;
//
// var hashSet = list.ToHashSet();
//
// // hashSet.Add()
// // hashSet.Remove() и тд и тп; НЕ Потокобезопасный 
//
// var immutableHashSet = list.ToImmutableHashSet();
//
// // ImmutablehashSet.Add()
// // ImmutablehashSet.Remove() и тд и тп;  Потокобезопасный  но каждый раз создает копию / РАсскажи что это сбалансированное дереов и что с ростом данных add/remove все больше и больше будет страдать
//
//
// var frozenSet = list.ToFrozenSet();
// // нет add/delete и тд и тп !!! Ну какой trade of ???
//
//
// Console.WriteLine("Hello, World!");

BenchmarkRunner.Run<SetBenchmark>();