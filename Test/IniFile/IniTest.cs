using System.Collections.Immutable;
using VanillaSugar.Ini;
using VanillaSugar.Ini.Exception;

namespace Test.IniFile;

[TestFixture]
[TestOf(typeof(Ini))]
public class IniTest
{
    [Test]
    public void IniSetAndGet()
    {
        Ini ini = new Ini();
        Assert.AreEqual(0, ini.Count);

        Assert.IsNull(ini.Get("sec"));
        Section sec = ini.GetOrAdd("sec");
        Assert.AreSame(sec, ini.Get("sec"));
        Assert.AreEqual(1, ini.Count);
        Section secVoid = ini.GetOrAdd("void");
        Assert.AreEqual(2, ini.Count);
        CollectionAssert.AreEqual(ImmutableList.Create("sec", "void"), ini.SectionNames.ToList());

        Assert.IsTrue(ini.Rename("void", "void2"));
        Assert.AreEqual(2, ini.Count);
        Assert.AreSame(secVoid, ini.Get("void2"));
        CollectionAssert.AreEqual(ImmutableList.Create("sec", "void2"), ini.SectionNames.ToList());

        Assert.IsTrue(ini.Remove("void2"));
        Assert.AreEqual(1, ini.Count);

        ini.Clear();
        Assert.AreEqual(0, ini.Count);
        CollectionAssert.AreEqual(ImmutableList<string>.Empty, ini.SectionNames.ToList());
        Assert.AreNotSame(sec, ini.GetOrAdd("sec"));
        Assert.AreEqual(sec, ini.GetOrAdd("sec"));
    }

    [Test]
    public void ValueSetAndGet()
    {
        Section sec = new Ini().GetOrAdd("test");
        Assert.AreEqual(0, sec.Count);
        Assert.IsNull(sec.Get("key"));
        Assert.IsNull(sec.Get("non-exist"));
        Assert.IsFalse(sec.Contains("key"));
        // first set
        sec.Set("key", "value");
        Assert.AreEqual("value", sec.Get("key"));
        Assert.IsNull(sec.Get("non-exist"));
        Assert.IsTrue(sec.Contains("key"));
        Assert.AreEqual(1, sec.Count);
        // reset
        sec.Set("key", "value1");
        Assert.AreEqual("value1", sec.Get("key"));
        Assert.IsTrue(sec.Contains("key"));
        Assert.AreEqual(1, sec.Count);
        // rename
        Assert.IsTrue(sec.Rename("key", "key1"));
        Assert.IsFalse(sec.Rename("key", "new-name"));
        Assert.AreEqual("value1", sec.Get("key1"));
        Assert.IsNull(sec.Get("key"));
        // second set
        sec.Set("key2", "value2");
        Assert.AreEqual("value2", sec.Get("key2"));
        Assert.AreEqual(2, sec.Count);
        CollectionAssert.AreEqual(ImmutableList.Create("key1", "key2"), sec.Keys.ToList());
        // remove
        Assert.IsTrue(sec.Remove("key1"));
        Assert.IsFalse(sec.Remove("key1"));
        Assert.AreEqual(1, sec.Count);
        // dangling text
        sec.DanglingText = "Dangling text";
        Assert.AreEqual("Dangling text", sec.DanglingText);
        sec.Clear();
        Assert.AreEqual(0, sec.Count);
        Assert.IsNull(sec.Get("key1"));
        Assert.IsNull(sec.DanglingText);
    }

    [Test]
    public void Comments()
    {
        Section sec = new Ini().GetOrAdd("test");
        Assert.AreEqual(0, sec.Count);
        Assert.AreEqual(0, sec.KeyAndCommentsCount);
        sec.Set("key1", "value1");
        sec.Set("key2", "value2");
        Assert.AreEqual(2, sec.KeyAndCommentsCount);

        sec.AddCommentsBefore("key1", ImmutableList.Create("aa", "bb"));
        sec.AddCommentsAfter("key1", ImmutableList.Create("cc", "dd"));
        sec.AddComments("ee", "ff");
        Assert.AreEqual(2, sec.Count);
        Assert.AreEqual(8, sec.KeyAndCommentsCount);
        CollectionAssert.AreEqual(ImmutableList.Create("aa", "bb"), sec.GetCommentsBefore("key1").ToList());
        CollectionAssert.AreEqual(ImmutableList.Create("cc", "dd"), sec.GetCommentsAfter("key1").ToList());
        CollectionAssert.AreEqual(ImmutableList.Create("cc", "dd"), sec.GetCommentsBefore("key2").ToList());
        CollectionAssert.AreEqual(ImmutableList.Create("ee", "ff"), sec.GetCommentsAfter("key2").ToList());
        CollectionAssert.AreEqual(ImmutableList.Create("aa", "bb", "cc", "dd", "ee", "ff"), sec.GetComments().ToList());
        sec.RemoveCommentsAfter("key1");
        CollectionAssert.AreEqual(ImmutableList.Create("aa", "bb", "ee", "ff"), sec.GetComments().ToList());
        Assert.IsTrue(sec.Remove("key2"));
        CollectionAssert.AreEqual(ImmutableList.Create("aa", "bb", "ee", "ff"), sec.GetComments().ToList());
        CollectionAssert.AreEqual(ImmutableList.Create("ee", "ff"), sec.GetCommentsAfter("key1").ToList());
        sec.RemoveCommentsAfter("key1");
        CollectionAssert.AreEqual(ImmutableList.Create("aa", "bb"), sec.GetComments().ToList());
        CollectionAssert.AreEqual(ImmutableList<string>.Empty, sec.GetCommentsAfter("key1").ToList());
        Assert.AreEqual(1, sec.Count);
        Assert.AreEqual(3, sec.KeyAndCommentsCount);
        sec.RemoveComments();
        CollectionAssert.AreEqual(ImmutableList<string>.Empty, sec.GetComments().ToList());
        Assert.AreEqual(1, sec.Count);
        Assert.AreEqual(1, sec.KeyAndCommentsCount);
    }

    [Test]
    public void CommentExceptions()
    {
        Section sec = new Ini().GetOrAdd("test");
        sec.Set("key1", "value1");
        Assert.AreEqual(1, sec.Count);
        Assert.AreEqual(1, sec.KeyAndCommentsCount);
        const string nonExistKey = "non-exist";
        Assert.Throws<AccessValueException>(() => sec.GetCommentsBefore(nonExistKey));
        Assert.Throws<AccessValueException>(() => sec.GetCommentsAfter(nonExistKey));
        Assert.Throws<AccessValueException>(() => sec.RemoveCommentsBefore(nonExistKey));
        Assert.Throws<AccessValueException>(() => sec.RemoveCommentsAfter(nonExistKey));
        Assert.Throws<AccessValueException>(() =>
            sec.AddCommentsBefore(nonExistKey, ImmutableList.Create("comment")));
        Assert.Throws<AccessValueException>(() =>
            sec.AddCommentsAfter(nonExistKey, ImmutableList.Create("comment")));
        Assert.AreEqual(1, sec.Count);
        Assert.AreEqual(1, sec.KeyAndCommentsCount);
    }

    [Test]
    public void GetItemValue()
    {
        Ini ini = GetExampleIni();
        Assert.AreEqual("value1", ini.GetItemValue("sec1", "key1"));
        Assert.AreEqual("value3", ini.GetItemValue("sec2", "key3"));
        Assert.IsNull(ini.GetItemValue("sec1", "non-exist"));
        Assert.IsNull(ini.GetItemValue("sec2", "non-exist"));
        Assert.IsNull(ini.GetItemValue("non-exist-sec", "key1"));
        Assert.IsNull(ini.GetItemValue("non-exist-sec", "key3"));
    }

    [Test]
    public void UntitledSection()
    {
        Ini ini = GetExampleIni();
        Section untitled = ini.UntitledSection;
        Assert.IsNotNull(untitled);
        Assert.AreEqual(0, untitled.Count);
        Assert.AreEqual(1, untitled.KeyAndCommentsCount);
        // key value
        untitled.Set("key1", "value1");
        untitled.Set("key2", "value2");
        Assert.AreEqual(2, untitled.Count);
        Assert.AreEqual(3, untitled.KeyAndCommentsCount);
        CollectionAssert.AreEqual(ImmutableList.Create("key1", "key2"), untitled.Keys.ToList());
        // comment
        untitled.AddCommentsAfter("key1", ImmutableList.Create("aa", "bb"));
        CollectionAssert.AreEqual(ImmutableList.Create("aa", "bb"), untitled.GetCommentsBefore("key2").ToList());
        CollectionAssert.AreEqual(ImmutableList.Create("This is a comment", "aa", "bb"),
            untitled.GetComments().ToList());
        // clear
        untitled.Clear();
        Assert.IsNull(untitled.DanglingText);
        CollectionAssert.AreEqual(ImmutableList<string>.Empty, untitled.Keys.ToList());
        CollectionAssert.AreEqual(ImmutableList<string>.Empty, untitled.GetComments().ToList());
    }

    [Test]
    public void IniEquality()
    {
        var ini1 = GetExampleIni();
        var ini2 = GetExampleIni();
        Assert.AreEqual(ini1.Get("sec1"), ini2.Get("sec1"));
        Assert.AreEqual(ini1.Get("sec2"), ini2.Get("sec2"));
        Assert.AreEqual(ini1, ini2);
        Assert.AreNotEqual(ini1.Get("sec1"), ini2.Get("sec2"));
        Assert.AreEqual(ini2.Get("sec1"), ini1.Get("sec1"));
        Assert.AreEqual(ini2.Get("sec2"), ini1.Get("sec2"));
        Assert.AreEqual(ini2, ini1);
        Assert.AreNotEqual(ini2.Get("sec2"), ini1.Get("sec1"));
    }

    [Test]
    public void DeepClone()
    {
        Ini origin = GetExampleIni();
        Ini clone = origin.DeepClone();
        Assert.AreNotSame(origin, clone);
        Assert.AreEqual(origin, clone);
        Assert.IsNotNull(clone.Get("sec1"));
        Assert.AreNotSame(origin.Get("sec1"), clone.Get("sec1"));
        Assert.IsNotNull(clone.Get("sec2"));
        Assert.AreNotSame(origin.Get("sec2"), clone.Get("sec2"));
        Assert.AreEqual("The dangling text", clone.Get("sec2").DanglingText);
    }

    [Test]
    public void ToCollection()
    {
        Ini ini = GetExampleIni();
        Section untitled = ini.UntitledSection;
        Section sec1 = ini.Get("sec1");
        Section sec2 = ini.Get("sec2");
        // Map
        Dictionary<string, string> example1 = new Dictionary<string, string>
        {
            {"key1", "value1"},
            {"key2", "value2"}
        };
        CollectionAssert.AreEqual(example1, sec1.ToMap().ToList());
        Dictionary<string, string> example2 = new Dictionary<string, string> {{"key3", "value3"}};
        CollectionAssert.AreEqual(example2, sec2.ToMap().ToList());
        // Comment
        CollectionAssert.AreEqual(ImmutableList.Create("This is a comment"), untitled.GetComments().ToList());
        CollectionAssert.AreEqual(ImmutableList<string>.Empty, sec1.GetComments().ToList());
        CollectionAssert.AreEqual(ImmutableList.Create("comment before key3", "comment after key3"),
            sec2.GetComments().ToList());
    }

    private static Ini GetExampleIni()
    {
        Ini origin = new Ini();
        origin.UntitledSection.AddComments("This is a comment");
        Section orgSec1 = origin.GetOrAdd("sec1");
        orgSec1.Set("key1", "value1");
        orgSec1.Set("key2", "value2");
        Section orgSec2 = origin.GetOrAdd("sec2");
        orgSec2.Set("key3", "value3");
        orgSec2.AddCommentsBefore("key3", ImmutableList.Create("comment before key3"));
        orgSec2.AddCommentsAfter("key3", ImmutableList.Create("comment after key3"));
        orgSec2.DanglingText = "The dangling text";
        return origin;
    }
}