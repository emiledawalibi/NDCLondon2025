using System.Text;
using BenchmarkDotNet.Attributes;

namespace OneBRC.Benchmarks;

[MemoryDiagnoser]
public class MultiFloatParsingBenchmarks
{
    private const byte Semicolon = (byte)';';
    private const byte Newline = (byte)'\n';
    
    private static readonly string Floats = Enumerable.Range(0, 1000)
        .Select(_ => (Random.Shared.NextSingle() * 10).ToString("0.000"))
        .Aggregate((a, b) => a + "\n" + b);

    private static readonly ReadOnlyMemory<byte> Utf8Floats = Encoding.UTF8.GetBytes(Floats);

    [Benchmark(Baseline = true)]
    public float SumAsFloats()
    {
        var sum = 0f;
        var span = Utf8Floats.Span;

        foreach (var range in span.Split(Newline))
        {
            var line = span[range];
            sum += float.Parse(line);
        }

        return sum;
    }

    [Benchmark]
    public float SumAsInts()
    {
        var sum = 0;
        var span = Utf8Floats.Span;
        
        foreach (var range in span.Split(Newline))
        {
            var line = span[range];
            sum += ParseAsInt(line);
        }

        return sum / 1000f;
    }

    private static int ParseAsInt(ReadOnlySpan<byte> span)
    {
        int value = 0;
        
        for (; span.Length > 0; span = span.Slice(1))
        {
            if (span[0] == (byte)'.') continue;
            
            value = value * 10 + (span[0] - (byte)'0');
        }

        return value;
    }
}