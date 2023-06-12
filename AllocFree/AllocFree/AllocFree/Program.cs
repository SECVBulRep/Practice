// See https://aka.ms/new-console-template for more information

using AllocFree;
using BenchmarkDotNet.Running;

var id = Guid.NewGuid();
Console.WriteLine(id);

var base64Id = Convert.ToBase64String(id.ToByteArray());
Console.WriteLine(base64Id);

var urlFriendlybase64Id = Guider.ToStringFromGuid(id);
var urlFriendlybase64IdOpt = Guider.ToStringFromGuidOpt(id);
Console.WriteLine(urlFriendlybase64Id);
Console.WriteLine(urlFriendlybase64IdOpt);

var idAgain = Guider.ToGuidFromString(urlFriendlybase64Id);
var idAgainOpt = Guider.ToGuidFromStringOpt(urlFriendlybase64Id);
Console.WriteLine(idAgain);
Console.WriteLine(idAgainOpt);



//BenchmarkRunner.Run<GuiderBenchmark>();