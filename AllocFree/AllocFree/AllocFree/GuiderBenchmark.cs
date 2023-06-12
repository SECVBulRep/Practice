using BenchmarkDotNet.Attributes;

namespace AllocFree;


[MemoryDiagnoser(false)]
public class GuiderBenchmark
{
    private static readonly Guid TestIdAsGuid = Guid.Parse("ebc40f68-d173-43d3-911b-6b2d17513087");
    private static readonly string TestIdAsString = "aA-E63PR00ORG2stF1Ewhw";

    [Benchmark()]
    public Guid ToGuidFromString()
    {
        return Guider.ToGuidFromString(TestIdAsString);
    }
    [Benchmark()]
    public Guid ToGuidFromStringOpt()
    {
        return Guider.ToGuidFromStringOpt(TestIdAsString);
    }
    
    
    [Benchmark()]
    public string  FromGuidToString()
    {
        return Guider.ToStringFromGuid(TestIdAsGuid);
    }
    
    
    
    [Benchmark()]
    public string  FromGuidToStringOpt()
    {
        return Guider.ToStringFromGuidOpt(TestIdAsGuid);
    }
}