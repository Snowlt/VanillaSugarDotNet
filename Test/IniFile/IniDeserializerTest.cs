using System.Collections.Immutable;
using System.Text;
using VanillaSugar.Ini;

namespace Test.IniFile;

[TestFixture]
[TestOf(typeof(IniDeserializer))]
public class IniDeserializerTest
{
    [Test]
    public void ReadNormal()
    {
        using var stream = ExampleContents.AsStream(ExampleContents.Normal);
        Ini ini = new IniDeserializer().Read(stream, Encoding.UTF8);
        Assert.IsNotNull(ini);
        Assert.IsTrue(ini.Contains("Sec1"));
        Assert.IsTrue(ini.Contains("Sec2"));
        Assert.AreEqual(0, ini.UntitledSection.KeyAndCommentsCount);
        CollectionAssert.AreEqual(ImmutableList.Create("Sec1", "Sec2"), ini.SectionNames.ToList());
        Section section1 = ini.Get("Sec1");
        Assert.IsNotNull(section1);
        Assert.AreEqual("value1", section1.Get("key1"));
        Assert.AreEqual("value2", section1.Get("key2"));
        Assert.AreEqual("value3", section1.Get("key3"));
        Section section2 = ini.Get("Sec2");
        Assert.IsNotNull(section2);
        Assert.AreEqual("value4", section2.Get("key4"));
        Assert.AreEqual("value5", section2.Get("key5"));
    }

    [Test]
    public void ReadAbnormal()
    {
        using var stream = ExampleContents.AsStream(ExampleContents.Abnormal);

        Ini ini = new IniDeserializer().Read(stream, Encoding.UTF8);
        Assert.IsNotNull(ini);
        CollectionAssert.AreEqual(ImmutableList.Create("Sec1", "Sec2", "Sec3"), ini.SectionNames.ToList());
        // Untitled Section
        Section untitledSection = ini.UntitledSection;
        Assert.IsNotNull(untitledSection);
        Assert.AreEqual(2, untitledSection.Count);
        Assert.AreEqual(3, untitledSection.KeyAndCommentsCount);
        Assert.IsNull(untitledSection.DanglingText);
        Assert.AreEqual("untitled-value1", untitledSection.Get("untitled-key1"));
        Assert.AreEqual("untitled value 2", untitledSection.Get("untitled key 2"));
        // Section 1
        Section section1 = ini.Get("Sec1");
        Assert.IsNotNull(section1);
        Assert.AreEqual(3, section1.Count);
        Assert.AreEqual(6, section1.KeyAndCommentsCount);
        Assert.AreEqual("  Dangling Content In Sec1\n" +
                        "\n" +
                        "  Next dangling line", section1.DanglingText);
        CollectionAssert.AreEqual(ImmutableList.Create("Comment1 before key1", "Comment2 before key1"),
            section1.GetCommentsBefore("key1").ToList());
        Assert.AreEqual("value1", section1.Get("key1"));
        Assert.AreEqual("value2\n    value2 next line", section1.Get("key2"));
        CollectionAssert.AreEqual(ImmutableList.Create("Comment before key3"),
            section1.GetCommentsBefore("key3").ToList());
        Assert.AreEqual("value3", section1.Get("key3"));
        // Section 2(Empty)
        Section section2 = ini.Get("Sec2");
        Assert.IsNotNull(section2);
        Assert.AreEqual(0, section2.KeyAndCommentsCount);
        Assert.IsNull(section2.DanglingText);
        // Section 3
        Section section3 = ini.Get("Sec3");
        Assert.IsNotNull(section3);
        Assert.AreEqual(2, section3.Count);
        Assert.AreEqual("value4", section3.Get("key4"));
        Assert.AreEqual("value5", section3.Get("key5"));
    }

    [Test]
    public void ReadTopDangling()
    {
        const string danglingText = "  Dangling Content In Sec1\n\n  Next dangling line";

        var deserializer1 = new IniDeserializer
        {
            DanglingTextOption = IniDeserializer.DanglingTextOptions.ToComment
        };
        using (var stream = ExampleContents.AsStream(ExampleContents.TopDangling))
        {
            Ini ini1 = deserializer1.Read(stream, Encoding.UTF8);
            Assert.AreEqual(0, ini1.UntitledSection.KeyAndCommentsCount);
            Section sectionA = ini1.Get("Sec1");
            Assert.IsNull(sectionA.DanglingText);
            CollectionAssert.AreEqual(ImmutableList.Create(danglingText, "Comment1 before key1"),
                sectionA.GetCommentsBefore("key1").ToList());
            CollectionAssert.AreEqual(ImmutableList.Create(danglingText, "Comment1 before key1"),
                sectionA.GetComments().ToList());
        }

        var deserializer2 = new IniDeserializer
        {
            DanglingTextOption = IniDeserializer.DanglingTextOptions.Drop
        };
        using (var stream = ExampleContents.AsStream(ExampleContents.TopDangling))
        {
            Ini ini2 = deserializer2.Read(stream, Encoding.UTF8);
            Assert.AreEqual(0, ini2.UntitledSection.KeyAndCommentsCount);
            Section sectionB = ini2.Get("Sec1");
            Assert.IsNull(sectionB.DanglingText);
            CollectionAssert.AreEqual(ImmutableList.Create("Comment1 before key1"),
                sectionB.GetCommentsBefore("key1").ToList());
            CollectionAssert.AreEqual(ImmutableList.Create("Comment1 before key1"), sectionB.GetComments().ToList());
        }

        var deserializer3 = new IniDeserializer
        {
            DanglingTextOption = IniDeserializer.DanglingTextOptions.Keep
        };
        using (var stream = ExampleContents.AsStream(ExampleContents.TopDangling))
        {
            Ini ini3 = deserializer3.Read(stream, Encoding.UTF8);
            Assert.AreEqual(0, ini3.UntitledSection.KeyAndCommentsCount);
            Section sectionC = ini3.Get("Sec1");
            Assert.AreEqual(danglingText, sectionC.DanglingText);
            CollectionAssert.AreEqual(ImmutableList.Create("Comment1 before key1"),
                sectionC.GetCommentsBefore("key1").ToList());
            CollectionAssert.AreEqual(ImmutableList.Create("Comment1 before key1"), sectionC.GetComments().ToList());
        }
    }
}