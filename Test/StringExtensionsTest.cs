using VanillaSugar.Core;

namespace Test;

[TestFixture]
[TestOf(typeof(StringExtensions))]
public class StringExtensionsTest
{
    [Test]
    public void Repeat()
    {
        Assert.AreEqual("", "Nothing".Repeat(0));
        Assert.AreEqual("", "Nothing".Repeat(-2));
        Assert.AreEqual("1", "1".Repeat(1));
        Assert.AreEqual("AbcAbcAbc", "Abc".Repeat(3));
    }

    [Test]
    public void AppendIfMissing()
    {
        Assert.AreEqual("Something", "Some".AppendIfMissing("thing"));
        Assert.AreEqual("Something", "Something".AppendIfMissing("thing"));
        Assert.AreEqual("Content", "Content".AppendIfMissing(""));
        Assert.AreEqual("Suffix", "".AppendIfMissing("Suffix"));
        Assert.AreEqual("Something else",
            "Something else".AppendIfMissing(" Else", StringComparison.OrdinalIgnoreCase));
        Assert.AreEqual("Something else Else", "Something else".AppendIfMissing(" Else", StringComparison.Ordinal));
    }

    [Test]
    public void PrependIfMissing()
    {
        Assert.AreEqual("something", "thing".PrependIfMissing("some"));
        Assert.AreEqual("something", "something".PrependIfMissing("some"));
        Assert.AreEqual("Content", "Content".PrependIfMissing(""));
        Assert.AreEqual("Prefix", "".PrependIfMissing("Prefix"));
        Assert.AreEqual("Transmission", "Transmission".PrependIfMissing("trans", StringComparison.OrdinalIgnoreCase));
        Assert.AreEqual("transTransmission", "Transmission".PrependIfMissing("trans", StringComparison.Ordinal));
    }

    [Test]
    public void Capitalize()
    {
        Assert.AreEqual("Tom", "tom".Capitalize());
        Assert.AreEqual("Tom", "Tom".Capitalize());
        Assert.AreEqual("TOM", "TOM".Capitalize());
        Assert.AreEqual("TOM", "tOM".Capitalize());
        Assert.AreEqual("", "".Capitalize());
    }

    [Test]
    public void Uncapitalize()
    {
        Assert.AreEqual("tom", "tom".Uncapitalize());
        Assert.AreEqual("tom", "Tom".Uncapitalize());
        Assert.AreEqual("tOM", "TOM".Uncapitalize());
        Assert.AreEqual("tOM", "tOM".Uncapitalize());
        Assert.AreEqual("", "".Uncapitalize());
    }
}