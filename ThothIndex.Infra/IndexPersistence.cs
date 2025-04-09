using System.Text.Json;
using ThothIndex.Domain;

namespace ThothIndex.Infra;
public class IndexPersistence : IIndexPersistence
{
    public void SaveToDisk(InvertedIndex index, string path)
    {
        var snapshot = index.GetIndexSnapshot();

        File.WriteAllText(path, JsonSerializer.Serialize(snapshot));
    }
}

public interface IIndexPersistence
{
    void SaveToDisk(InvertedIndex index, string path);
}
