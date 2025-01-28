using System.Runtime.InteropServices;

namespace OneBRC.Benchmarks;

public readonly struct CityKey : IEquatable<CityKey>
{
    private readonly long _0;
    private readonly long _1;
    private readonly long _2;
    private readonly long _3;

    public CityKey(ReadOnlySpan<long> span)
    {
        _0 = span[0];
        _1 = span[1];
        _2 = span[2];
        _3 = span[3];
    }

    public static CityKey Create(ReadOnlySpan<byte> span)
    {
        Span<long> vector = stackalloc long[8];
        
        if (span.Length < 9)
        {
            Span<byte> buffer = stackalloc byte[8];
            span.CopyTo(buffer);
            vector[0] = MemoryMarshal.Read<long>(buffer);
        }
        else if (span.Length < 17)
        {
            Span<byte> buffer = stackalloc byte[8];
            span.Slice(0, 8).CopyTo(buffer);
            vector[0] = MemoryMarshal.Read<long>(buffer);
            span.Slice(8).CopyTo(buffer);
            vector[1] = MemoryMarshal.Read<long>(buffer);
        }
        else if (span.Length < 25)
        {
            Span<byte> buffer = stackalloc byte[8];
            span.Slice(0, 8).CopyTo(buffer);
            vector[0] = MemoryMarshal.Read<long>(buffer);
            span.Slice(8, 8).CopyTo(buffer);
            vector[1] = MemoryMarshal.Read<long>(buffer);
            span.Slice(16).CopyTo(buffer);
            vector[2] = MemoryMarshal.Read<long>(buffer);
        }
        else if (span.Length < 33)
        {
            Span<byte> buffer = stackalloc byte[8];
            span.Slice(0, 8).CopyTo(buffer);
            vector[0] = MemoryMarshal.Read<long>(buffer);
            span.Slice(8, 8).CopyTo(buffer);
            vector[1] = MemoryMarshal.Read<long>(buffer);
            span.Slice(16, 8).CopyTo(buffer);
            vector[2] = MemoryMarshal.Read<long>(buffer);
            span.Slice(24).CopyTo(buffer);
            vector[3] = MemoryMarshal.Read<long>(buffer);
        }
        
        return new CityKey(vector);
    }

    public bool Equals(CityKey other)
    {
        return _0 == other._0 && _1 == other._1 && _2 == other._2 && _3 == other._3;
    }

    public override bool Equals(object? obj)
    {
        return obj is CityKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_0, _1, _2, _3);
    }

    public static bool operator ==(CityKey left, CityKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CityKey left, CityKey right)
    {
        return !left.Equals(right);
    }
}