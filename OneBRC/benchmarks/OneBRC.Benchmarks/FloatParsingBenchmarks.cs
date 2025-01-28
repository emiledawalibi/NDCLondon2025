using System.Text;
using BenchmarkDotNet.Attributes;

namespace OneBRC.Benchmarks;

[MemoryDiagnoser]
public class FloatParsingBenchmarks
{
    private const byte Semicolon = (byte)';';
    private const byte Newline = (byte)'\n';
    
    private const string Line = "London;1.111";
    private static readonly ReadOnlyMemory<byte> Utf8Line = "London;1.111"u8.ToArray();
    private static readonly byte[] Lines = "London;1.111\nParis;0.222\nNew York;666.666\n"u8.ToArray();
    
    // [Benchmark(Baseline = true)]
    // public float Naive()
    // {
    //     return float.Parse(Line.Split(';')[1]);
    // }
    //
    // [Benchmark]
    // public float FewerAllocations()
    // {
    //     var split = Line.IndexOf(';');
    //     var value = Line.Substring(split + 1);
    //     return float.Parse(value);
    // }
    //
    [Benchmark(Baseline = true)]
    public float ZeroAllocations()
    {
        var span = Line.AsSpan();
        var split = span.IndexOf(';');
        span = span.Slice(split + 1);
        return float.Parse(span);
    }

    [Benchmark]
    public float Utf8ZeroAllocations()
    {
        var span = Utf8Line.Span;
        
        var split = span.IndexOf(Semicolon);
        span = span.Slice(split + 1);
        return float.Parse(span);
    }
}