// See https://aka.ms/new-console-template for more information

using AllocFree;
using BenchmarkDotNet.Running;

// var id = Guid.NewGuid();
// Console.WriteLine(id);
//
// var base64Id = Convert.ToBase64String(id.ToByteArray());
// Console.WriteLine(base64Id);
//
// var urlFriendlybase64Id = Guider.ToStringFromGuid(id);
// Console.WriteLine(urlFriendlybase64Id);
//
// var idAgain = Guider.ToGuidFromString(urlFriendlybase64Id);
// Console.WriteLine(idAgain);
//
//

BenchmarkRunner.Run<GuiderBenchmark>();