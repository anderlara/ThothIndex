using ThothIndex.Domain;

namespace ThothIndex.Test;
public class InvertedIndexTests
{
    [Fact]
    public void Add_And_Search_Word_Returns_Correct_Document()
    {
        var index = new InvertedIndex();
        index.Add("Porsche", "file1.txt");
        index.Add("Porsche", "file2.txt");

        var results = index.Search("porsche").ToList();

        Assert.Contains("file1.txt", results);
        Assert.Contains("file2.txt", results);
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void Search_Nonexistent_Word_Returns_Empty()
    {
        var index = new InvertedIndex();
        var results = index.Search("nonexistent");

        Assert.Empty(results);
    }

    [Fact]
    public void Normalize_Removes_Special_Characters()
    {
        string input = "B@n@n@";
        string expected = "bnn";
        string actual = InvertedIndex.Normalize(input);

        Assert.Equal(expected, actual);
    }
}