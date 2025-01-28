using System.Reflection;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace OneBRC.Benchmarks;

public class CityMemoryPool
{
    private readonly byte[] _pool = new byte[16384];
    private int _next;

    public ReadOnlyMemory<byte> GetOrAdd(ReadOnlySpan<byte> key)
    {
        var span = _pool.AsSpan();
        
        int index = span.IndexOf(key);
        if (index > -1)
        {
            return _pool.AsMemory(index, key.Length);
        }
        key.CopyTo(span.Slice(_next));
        var memory = _pool.AsMemory(_next, key.Length);
        _next += key.Length;
        return memory;
    }
}

[MemoryDiagnoser]
public class CityBenchmarks
{
    private static readonly ReadOnlyMemory<byte> ThousandTxt = GetThousandTxt();
    private static readonly CityMemoryPool _pool = new();

    [Benchmark(Baseline = true)]
    public int CountAsStrings()
    {
        var dict = new Dictionary<string, int>();
        
        var span = ThousandTxt.Span;
        foreach (var range in span.Split((byte)'\n'))
        {
            var line = span[range];
            var semicolon = line.IndexOf((byte)';');
            var utf8City = line.Slice(0, semicolon);
            var city = Encoding.UTF8.GetString(utf8City);
            dict[city] = dict.TryGetValue(city, out var count) ? count + 1 : 1;
        }
        
        return dict.Count;
    }

    [Benchmark]
    public int CountAsPooledStrings()
    {
        var dict = new Dictionary<ReadOnlyMemory<byte>, int>();
        
        var span = ThousandTxt.Span;
        foreach (var range in span.Split((byte)'\n'))
        {
            var line = span[range];
            var semicolon = line.IndexOf((byte)';');
            var utf8City = line.Slice(0, semicolon);
            var city = _pool.GetOrAdd(utf8City);
            dict[city] = dict.TryGetValue(city, out var count) ? count + 1 : 1;
        }
        
        return dict.Count;
    }

    private static byte[] GetThousandTxt()
    {
        using var s = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("OneBRC.Benchmarks.thousand.txt")!;
        var bytes = new byte[s.Length];
        s.ReadExactly(bytes);
        return bytes;
    }
}