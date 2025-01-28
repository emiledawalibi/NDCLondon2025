using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace OneBRC;

public class MemoryMappedFileImpl
{
    private readonly string _filePath;

    public MemoryMappedFileImpl(string filePath)
    {
        _filePath = filePath;
    }

    public ValueTask Run()
    {
        var size = new FileInfo(_filePath).Length;
        var threadCount = Environment.ProcessorCount * 8;

        using var mmf = MemoryMappedFile.CreateFromFile(_filePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
        
        var chunks = MemoryMappedFileAnalyzer.Analyze(mmf, size, threadCount);
        using var view = mmf.CreateViewAccessor(0, size, MemoryMappedFileAccess.Read);

        Parallel.ForEach(chunks, new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
        }, chunk => chunk.Run(view));

        var dictionary = chunks[0].Dictionary;

        for (int i = 1; i < threadCount; i++)
        {
            foreach (var (key, value) in chunks[i].Dictionary)
            {
                ref var accumulator = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out bool existed);
                if (!existed) accumulator.SetCity(value.City);
                accumulator.Combine(value);
            }
        }
        
        foreach (var accumulator in dictionary.Values.OrderBy(a => a.City))
        {
            Console.WriteLine($"{accumulator.City}: {accumulator.Min:F1}/{accumulator.Mean:F1}/{accumulator.Max:F1}");
        }
        
        return default;
    }
}