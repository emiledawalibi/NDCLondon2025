using System.IO.MemoryMappedFiles;

namespace OneBRC;

public static class MemoryMappedFileAnalyzer
{
    public static unsafe MemoryMappedFileChunk[] Analyze(MemoryMappedFile mmf, long size, int threadCount)
    {
        var chunks = new MemoryMappedFileChunk[threadCount];

        using var accessor = mmf.CreateViewAccessor(0, size, MemoryMappedFileAccess.Read);
        byte* pointer = null;
        accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref pointer);

        var estimatedChunkSize = size / threadCount;

        long offset = 0;

        for (int i = 0; i < threadCount - 1; i++)
        {
            var span = new ReadOnlySpan<byte>(pointer + offset, (int)estimatedChunkSize);
            var lastNewline = span.LastIndexOf((byte)'\n');
            var actualSize = lastNewline + 1;
            chunks[i] = new MemoryMappedFileChunk(offset, actualSize, i);
            offset += actualSize;
        }

        chunks[^1] = new MemoryMappedFileChunk(offset, (int)(size - offset), threadCount - 1);

        return chunks;
    }
}