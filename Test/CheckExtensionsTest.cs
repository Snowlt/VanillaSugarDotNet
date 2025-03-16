using VanillaSugar.Core;

namespace Test;

[TestFixture]
[TestOf(typeof(CheckExtensions))]
public class CheckExtensionsTest
{
    [Test]
    public void DictionaryEqual()
    {
        var dict1 = new Dictionary<int, string> {{1, "one"}, {2, "two"}};
        var dict2 = new Dictionary<int, string> {{1, "one"}, {2, "two"}};
        var dict3 = new Dictionary<int, string> {{1, "one"}, {3, "three"}};
        Assert.IsTrue(dict1.DictionaryEqual(dict2));
        Assert.IsTrue(dict2.DictionaryEqual(dict1));
        Assert.IsFalse(dict1.DictionaryEqual(dict3));
        Assert.IsFalse(dict3.DictionaryEqual(dict1));
        // compare key
        var dict4 = new Dictionary<int, string> {{-1, "one"}, {-2, "two"}};
        Assert.IsFalse(dict1.DictionaryEqual(dict4));
        Assert.IsFalse(dict4.DictionaryEqual(dict1));
    }

    [Test]
    public void DictionaryEqualWithComparer()
    {
        var dict1 = new Dictionary<int, string> {{1, "one"}, {3, "THREE"}};
        var dict2 = new Dictionary<int, string> {{1, "one"}, {3, "three"}};
        Assert.IsFalse(dict1.DictionaryEqual(dict2));
        Assert.IsFalse(dict2.DictionaryEqual(dict1));
        var comparer = EqualityComparer<string>.Create((a, b) =>
            a == b || string.Equals(a ?? "", b ?? "", StringComparison.OrdinalIgnoreCase));
        Assert.IsTrue(dict1.DictionaryEqual(dict2, comparer));
        Assert.IsTrue(dict2.DictionaryEqual(dict1, comparer));
    }
}