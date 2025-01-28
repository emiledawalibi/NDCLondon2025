using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;
using OneBRC.Shared;

namespace OneBRC;

public class MemoryMappedFileChunk
{
    private readonly long _offset;
    private readonly int _actualSize;
    public Dictionary<ulong, IntAccumulator> Dictionary { get; } = new();


    public MemoryMappedFileChunk(long offset, int actualSize, int i)
    {
        _offset = offset;
        _actualSize = actualSize;
    }

    public unsafe void Run(MemoryMappedViewAccessor view)
    {
        byte* pointer = null;
        view.SafeMemoryMappedViewHandle.AcquirePointer(ref pointer);

        var span = new ReadOnlySpan<byte>(pointer + _offset, _actualSize);

        while (span.Length > 0)
        {
            var eol = span.IndexOf((byte)'\n');
            if (eol == -1)
            {
                break;
            }

            var line = span.Slice(0, eol);
            ProcessLine(line);
            span = span.Slice(eol + 1);
        }
    }

    private void ProcessLine(ReadOnlySpan<byte> line)
    {
        var delimit = line.IndexOf((byte)';');
        var cityUtf8 = line.Slice(0, delimit);
        var valueUtf8 = line.Slice(delimit + 1);

        var cityHash = FastHash.HashBytes(cityUtf8);
        ref var acc = ref CollectionsMarshal.GetValueRefOrAddDefault(Dictionary, cityHash, out bool existed);
        if (!existed)
        {
            acc.SetCity(Encoding.UTF8.GetString(cityUtf8));
        }

        var value = FastParse.FloatAsInt(valueUtf8);
        acc.Record(value);
    }
}