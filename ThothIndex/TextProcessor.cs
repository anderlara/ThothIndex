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
        Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = maxThreads }, file =>
        {
            var content = File.ReadAllText(file);
            var words = Regex.Split(content, "\\W+");
            foreach (var word in words.Where(w => !string.IsNullOrWhiteSpace(w)))
            {
                index.Add(word, Path.GetFileName(file));
            }
        });
    }
}

public interface ITextProcessor
{
    void ProcessFiles(string[] files);
}
