using BenchmarkDotNet.Attributes;

namespace OneBRC.Benchmarks;

[MemoryDiagnoser]
public class RangeVsSlice
{
    private static readonly ReadOnlyMemory<byte> Utf8Line = "London;1.111"u8.ToArray();

    [Benchmark(Baseline = true)]
    public int Slice()
    {
        var span = Utf8Line.Span;
        var index = span.IndexOf((byte)';');
        return span.Slice(index).Length;
    }
    
    [Benchmark]
    public int Range()
    {
        var span = Utf8Line.Span;
        var index = span.IndexOf((byte)';');
        return span[index..].Length;
    }
}