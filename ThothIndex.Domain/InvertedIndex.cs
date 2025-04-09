using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace ThothIndex.Domain;

public class InvertedIndex
{
    private readonly ConcurrentDictionary<string, ConcurrentBag<string>> index = new();

    public void Add(string word, string document)
    {
        var normalized = Normalize(word);
        index.AddOrUpdate(normalized,
            _ => new ConcurrentBag<string> { document },
            (_, list) => { list.Add(document); return list; });
    }

    public IEnumerable<string> Search(string word)
    {
        var normalized = Normalize(word);
        return index.TryGetValue(normalized, out var list) ? list.Distinct() : Enumerable.Empty<string>();
    }

    public Dictionary<string, List<string>> GetIndexSnapshot()
    {
        return index.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Distinct().ToList());
    }

    public static string Normalize(string word) => Regex.Replace(word.ToLowerInvariant(), "[^a-z0-9]", "");
}
