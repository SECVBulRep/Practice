using System.Collections.Frozen;
using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;

namespace F.App;




[MemoryDiagnoser()]
public class SetBenchmark
{
    private Random _random = null!;

    private List<int> _list = null!;

    private HashSet<int> _hashSet = null!;
    private ImmutableHashSet<int> _immutableHashSet = null!;
    private FrozenSet<int> _frozenSet = null!;

    
    private Dictionary<int,string> _dictionary = null!;
    private ImmutableDictionary<int,string> _immutableDictionary = null!;
    private FrozenDictionary<int,string> _frozenDictionary = null!;
    
    private int _searchItem;

    [Params(1000)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _random = new Random(420);
        _list = Enumerable.Range(0, Size).Select(_ => _random.Next()).ToList();
        _searchItem = _list[Size / 2];

        _hashSet = _list.ToHashSet();
        _immutableHashSet = _list.ToImmutableHashSet();
        _frozenSet = _list.ToFrozenSet(true);

        _dictionary = _list.ToDictionary(x => x, x => x.ToString());
        _immutableDictionary = _dictionary.ToImmutableDictionary();
        _frozenDictionary = _dictionary.ToFrozenDictionary();

    }

    // [Benchmark()]
    // public HashSet<int> HashSet_Init() => _list.ToHashSet();
    //
    // [Benchmark()]
    // public ImmutableHashSet<int> ImmutableHashSet_Init() => _list.ToImmutableHashSet();
    
    [Benchmark()]
    public FrozenSet<int> FrozenSet_Init() => _list.ToFrozenSet();
    
    [Benchmark()]
    public FrozenSet<int> FrozenSet_Init_OptimizedForReading() => _list.ToFrozenSet(true);
    
    // [Benchmark()]
    // public bool HashSet_Contains() => _hashSet.Contains(_searchItem);
    //
    // [Benchmark()]
    // public bool ImmutableHashSet_Contains() => _immutableHashSet.Contains(_searchItem);
    //
    // [Benchmark()]
    // public bool FrozenSet_Contains() => _frozenSet.Contains(_searchItem);
    
    
    // [Benchmark()]
    // public bool Dictionary_ContainsKey() => _dictionary.ContainsKey(_searchItem);
    //
    // [Benchmark()]
    // public bool ImmutableDictionary_ContainsKey() => _immutableDictionary.ContainsKey(_searchItem);
    //
    // [Benchmark()]
    // public bool FrozenDictionary_ContainsKey() => _frozenDictionary.ContainsKey(_searchItem);
    //
    // [Benchmark()]
    // public string Dictionary_Contains() => _dictionary[_searchItem];
    //
    // [Benchmark()]
    // public string ImmutableDictionary_Contains() => _immutableDictionary[_searchItem];
    //
    // [Benchmark()]
    // public string FrozenDictionary_Contains() => _frozenDictionary[_searchItem];
    
    
}