using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using ThothIndex.Domain;

namespace ThothIndex.App;

public class TextProcessor : ITextProcessor
{
    private readonly InvertedIndex index;
    private readonly int maxThreads;

    public TextProcessor(InvertedIndex index, IConfiguration config)
    {
        this.index = index;
        maxThreads = int.TryParse(config["maxthreads"], out int value) ? value : Environment.ProcessorCount;
    }

    public void ProcessFiles(string[] files)
    {
        var partitions = PartitionFiles(files, maxThreads);
        var tasks = partitions.Select(partition => Task.Run(() =>
        {
            foreach (var file in partition)
            {
                var content = File.ReadAllText(file);
                var words = Regex.Split(content, "\\W+");
                foreach (var word in words.Where(w => !string.IsNullOrWhiteSpace(w)))
                {
                    index.Add(word, Path.GetFileName(file));
                }
            }
        })).ToArray();

        Task.WaitAll(tasks);
    }

    private List<List<string>> PartitionFiles(string[] files, int partitions)
    {
        var result = new List<List<string>>();
        for (int i = 0; i < partitions; i++)
            result.Add(new List<string>());

        for (int i = 0; i < files.Length; i++)
        {
            result[i % partitions].Add(files[i]);
        }

        return result;
    }
}

public interface ITextProcessor
{
    void ProcessFiles(string[] files);
}
