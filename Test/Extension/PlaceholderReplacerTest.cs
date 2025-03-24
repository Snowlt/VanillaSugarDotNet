using VanillaSugar.Extension;

namespace Test.Extension;

[TestFixture]
[TestOf(typeof(PlaceholderReplacer))]
public class PlaceholderReplacerTest
{
    [Test]
    public void SetReplacement()
    {
        var replacer = new PlaceholderReplacer();
        replacer.SetReplacement("{0}", "First");
        Assert.AreEqual("First demo", replacer.ReplaceAll("{0} demo", false));
        Assert.AreEqual("First demo", replacer.ReplaceAll("{0} demo", true));
        replacer.SetReplacement("{0}", "Second");
        Assert.AreEqual("Second demo", replacer.ReplaceAll("{0} demo", false));
        Assert.AreEqual("Second demo", replacer.ReplaceAll("{0} demo", true));
    }

    [Test]
    public void ReplaceAll()
    {
        var replacer = GetPlaceholderReplacer();
        Assert.AreEqual("prefix * suffix", replacer.ReplaceAll("prefix ba suffix", false));
        Assert.AreEqual("prefix 21 suffix", replacer.ReplaceAll("prefix ba suffix", true));
        Assert.AreEqual("1 * 3", replacer.ReplaceAll("a ba c", false));
        Assert.AreEqual("1 21 3", replacer.ReplaceAll("a ba c", true));
        Assert.AreEqual("*v*", replacer.ReplaceAll("bavba", false));
        Assert.AreEqual("21v21", replacer.ReplaceAll("bavba", true));
    }

    [Test]
    public void ReplaceAllWithPrefix()
    {
        var replacer = GetPlaceholderReplacer();
        Assert.AreEqual("prefix * suffix", replacer.ReplaceAllByPrefix("prefix $ba suffix", '$', false));
        Assert.AreEqual("prefix 2a suffix", replacer.ReplaceAllByPrefix("prefix $ba suffix", '$', true));
        // boundary
        Assert.AreEqual("1 2b3,*", replacer.ReplaceAllByPrefix("$a $bb$c,$ba", '$', false));
        Assert.AreEqual("1 2b3,2a", replacer.ReplaceAllByPrefix("$a $bb$c,$ba", '$', true));
        Assert.AreEqual("*v*", replacer.ReplaceAllByPrefix("$bav$ba", '$', false));
        Assert.AreEqual("2av2a", replacer.ReplaceAllByPrefix("$bav$ba", '$', true));
        // With not in replacer
        Assert.AreEqual("prefix #x1*1#x suffix", replacer.ReplaceAllByPrefix("prefix #x1#ba1#x suffix", '#', false));
        Assert.AreEqual("prefix #x12a1#x suffix", replacer.ReplaceAllByPrefix("prefix #x1#ba1#x suffix", '#', true));
        Assert.AreEqual("#x1*1#x", replacer.ReplaceAllByPrefix("#x1#ba1#x", '#', false));
        Assert.AreEqual("#x12a1#x", replacer.ReplaceAllByPrefix("#x1#ba1#x", '#', true));
        // escape
        Assert.AreEqual("$a=1 escape by $", replacer.ReplaceAllByPrefix("$$a=$a escape by $$", '$', false));
        Assert.AreEqual("$a=1 escape by $", replacer.ReplaceAllByPrefix("$$a=$a escape by $", '$', false));
        Assert.AreEqual("*$a+$b=$c => 1+2=3*",
            replacer.ReplaceAllByPrefix("$ba$$a+$$b=$$c => $a+$b=$c$ba", '$', false));
        Assert.AreEqual("2a$a+$b=$c => 1+2=32a",
            replacer.ReplaceAllByPrefix("$ba$$a+$$b=$$c => $a+$b=$c$ba", '$', true));
    }

    [Test]
    public void ToPlaceholdersSet()
    {
        var replacer = GetPlaceholderReplacer();
        var expected = new HashSet<string> {"b", "a", "ba", "c"};
        Assert.IsTrue(expected.SetEquals(replacer.ToPlaceholdersSet()));
    }

    private static PlaceholderReplacer GetPlaceholderReplacer()
    {
        var replacer = new PlaceholderReplacer();
        replacer.SetReplacement("a", "1");
        replacer.SetReplacement("b", "2");
        replacer.SetReplacement("c", "3");
        replacer.SetReplacement("ba", "*");
        return replacer;
    }
}