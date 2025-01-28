namespace OneBRC.Benchmarks;

internal sealed class ReadOnlyMemoryComparer : IEqualityComparer<ReadOnlyMemory<byte>>, 
    IAlternateEqualityComparer<ReadOnlySpan<byte>, ReadOnlyMemory<byte>>
{
    public static IEqualityComparer<ReadOnlyMemory<byte>> Default { get; } = new ReadOnlyMemoryComparer();
 
    public ReadOnlyMemory<byte> Create(ReadOnlySpan<byte> alternate) => 
        alternate.ToArray();
 
    public bool Equals(ReadOnlySpan<byte> alternate, ReadOnlyMemory<byte> other) =>
        alternate.SequenceEqual(other.Span);
 
    public bool Equals(ReadOnlyMemory<byte> x, ReadOnlyMemory<byte> y) => 
        x.Span.SequenceEqual(y.Span);
 
    public int GetHashCode(ReadOnlySpan<byte> alternate)
    {
        HashCode hc = default;
        hc.AddBytes(alternate);
        return hc.ToHashCode();
    }
 
    public int GetHashCode(ReadOnlyMemory<byte> obj) => GetHashCode(obj.Span);
}