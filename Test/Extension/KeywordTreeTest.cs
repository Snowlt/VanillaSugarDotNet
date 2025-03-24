using VanillaSugar.Extension;

namespace Test.Extension;

[TestFixture]
[TestOf(typeof(KeywordTree))]
public class KeywordTreeTest
{
    [Test]
    public void ContainsAnyKeyword()
    {
        var tree = GetDemoKeywordTree();
        Assert.IsTrue(tree.ContainsAny("12be21"));
        Assert.IsTrue(tree.ContainsAny("be123"));
        Assert.IsTrue(tree.ContainsAny("123be"));
        Assert.IsFalse(tree.ContainsAny("ball"));
        Assert.IsFalse(tree.ContainsAny("ring alarm on christmas"));
    }

    [Test]
    public void MatchFirstKeyword()
    {
        var tree = GetDemoKeywordTree();
        Assert.AreEqual("be", tree.MatchFirstKeyword("12be21", true));
        Assert.AreEqual("be", tree.MatchFirstKeyword("be123", true));
        Assert.AreEqual("be", tree.MatchFirstKeyword("123be", true));
        Assert.AreEqual("be", tree.MatchFirstKeyword("12bell21", true));
        Assert.AreEqual("be", tree.MatchFirstKeyword("bell123", true));
        Assert.AreEqual("be", tree.MatchFirstKeyword("123bell", true));
        Assert.AreEqual("bell", tree.MatchFirstKeyword("12bell21", false));
        Assert.AreEqual("bell", tree.MatchFirstKeyword("bell123", false));
        Assert.AreEqual("bell", tree.MatchFirstKeyword("123bell", false));
        var tree2 = new KeywordTree();
        tree2.Add("a");
        tree2.Add("b");
        tree2.Add("c");
        tree2.Add("ba");
        const string target1 = "123ba45";
        Assert.AreEqual("b", tree2.MatchFirstKeyword(target1, true));
        Assert.AreEqual("ba", tree2.MatchFirstKeyword(target1, false));
        const string target2 = "ba123";
        Assert.AreEqual("b", tree2.MatchFirstKeyword(target2, true));
        Assert.AreEqual("ba", tree2.MatchFirstKeyword(target2, false));
        const string target3 = "123ba";
        Assert.AreEqual("b", tree2.MatchFirstKeyword(target3, true));
        Assert.AreEqual("ba", tree2.MatchFirstKeyword(target3, false));
    }

    [Test]
    public void ToKeywordsSet()
    {
        var tree = GetDemoKeywordTree();
        Assert.AreEqual(2, tree.MinLength);
        Assert.AreEqual(4, tree.MaxLength);
        var expectedSet = new HashSet<string> {"be", "bed", "bell"};
        Assert.IsTrue(expectedSet.SetEquals(tree.ToKeywordsSet()));
    }

    private static KeywordTree GetDemoKeywordTree()
    {
        var tree = new KeywordTree();
        tree.Add("bell");
        tree.Add("be");
        tree.Add("bed");
        return tree;
    }
}