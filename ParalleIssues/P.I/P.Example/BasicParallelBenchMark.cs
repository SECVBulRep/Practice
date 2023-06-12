using BenchmarkDotNet.Attributes;

namespace P.Example;

[MemoryDiagnoser()]
public class BasicParallelBenchMark
{
    [Benchmark()]
    public int[] NormalForEach()
    {
        var array = new int[1_000_000];

        for (int i = 0; i < 1_000_000; i++)
        {
            array[i] = i;
        }

        return array;
    }

    [Benchmark()]
    public int[] ParallelNormalForEach()
    {
        var array = new int[1_000_000];

        Parallel.For(0, 1_000_000, i => { array[i] = i; });

        return array;
    }
}