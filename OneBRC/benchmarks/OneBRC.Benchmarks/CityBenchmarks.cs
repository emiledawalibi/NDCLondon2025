using System.Reflection;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace OneBRC.Benchmarks;

[MemoryDiagnoser]
public class CityBenchmarks
{
    private static readonly ReadOnlyMemory<byte> ThousandTxt = GetThousandTxt();

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
    public int CountHashed()
    {
        var dict = new Dictionary<long, int>();

        var span = ThousandTxt.Span;
        foreach (var range in span.Split((byte)'\n'))
        {
            var line = span[range];
            var semicolon = line.IndexOf((byte)';');
            var utf8City = line.Slice(0, semicolon);
            long hash = 0;
            unchecked
            {
                foreach (var b in utf8City)
                {
                    hash = (hash * 97) + b;
                }
            }

            dict[hash] = dict.TryGetValue(hash, out var count) ? count + 1 : 1;
        }

        return dict.Count;
    }


    [Benchmark]
    public int CountMemory()
    {
        var dict = new Dictionary<ReadOnlyMemory<byte>, int>(ReadOnlyMemoryComparer.Default);
        var lookup = dict.GetAlternateLookup<ReadOnlySpan<byte>>();

        var span = ThousandTxt.Span;
        foreach (var range in span.Split((byte)'\n'))
        {
            var line = span[range];
            var semicolon = line.IndexOf((byte)';');
            var utf8City = line.Slice(0, semicolon);
            if (lookup.TryGetValue(utf8City, out var city))
            {
                lookup[utf8City] = city + 1;
            }
            else
            {
                lookup[utf8City] = 1;
            }
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