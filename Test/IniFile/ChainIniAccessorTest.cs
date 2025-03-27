using System.Collections.Immutable;
using VanillaSugar.Ini;

namespace Test.IniFile;

[TestFixture]
[TestOf(typeof(ChainIniAccessor))]
public class ChainIniAccessorTest
{
    [Test]
    public void AddAndSet()
    {
        Ini example = new Ini();
        Section untitledSection = example.UntitledSection;
        untitledSection.AddComments("Hello", "World");
        untitledSection.Set("key", "value");
        untitledSection.AddComments("Ini");
        Section section = example.GetOrAdd("sec");
        section.Set("a", "1");
        section.Set("b", "2");
        section.Set("c", "3");
        section.AddCommentsBefore("b", ImmutableList.Create("Before b"));
        example.GetOrAdd("void");

        Ini ini = new Ini();
        ini.ChainAccess()
            .OpenUntitledSection()
            .AddComment("Hello").AddComment("World").Set("key", "value").AddComment("Ini")
            .CloseSection()
            .OpenSection("sec")
            .Set("a", "1").AddComment("Before b").Set("b", "2").Set("c", "3")
            .CloseSection()
            .OpenSection("void").CloseSection();

        Assert.AreEqual(example, ini);
    }

    [Test]
    public void Section()
    {
        Ini original = new Ini();
        Section section = original.GetOrAdd("sec");
        section.Set("key", "1");
        section.Set("key2", "2");
        Assert.AreEqual("1", section.Get("key"));
        Assert.AreEqual("2", section.Get("key2"));
        ChainSectionAccessor sectionAccessor = original.ChainAccess().OpenSection("sec");
        sectionAccessor.Rename("key", "new-key");
        Assert.AreEqual("1", section.Get("new-key"));
        Assert.IsFalse(section.Contains("key"));
        sectionAccessor.Remove("key2");
        Assert.IsFalse(section.Contains("key2"));
    }

    [Test]
    public void Ini()
    {
        Ini original = new Ini();
        Section section = original.GetOrAdd("sec");
        section.Set("key", "1");
        original.GetOrAdd("sec2");
        Assert.IsTrue(original.Contains("sec"));
        Assert.AreEqual("1", original.GetItemValue("sec", "key"));
        Assert.IsTrue(original.Contains("sec2"));
        ChainIniAccessor iniAccessor = original.ChainAccess().RenameSection("sec", "new-sec");
        Assert.IsTrue(original.Contains("new-sec"));
        Assert.IsFalse(original.Contains("sec"));
        Assert.IsTrue(original.Contains("sec2"));
        Assert.AreEqual("1", original.GetItemValue("new-sec", "key"));
        iniAccessor.RemoveSection("sec2");
        Assert.IsTrue(original.Contains("new-sec"));
        Assert.IsFalse(original.Contains("sec2"));
    }
}