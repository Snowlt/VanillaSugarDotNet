using VanillaSugar.Core;

namespace Test;

[TestFixture]
[TestOf(typeof(CheckHelper))]
public class CheckHelperTest
{
    [Test]
    public void BlankString()
    {
        string[] noEmpty = ["Hello", "unit", "Test"];
        string[] oneEmpty = ["123", string.Empty, "ABC"];
        string[] oneBlank = ["123", "  \t\n", "ABC"];
        string?[] blankAndEmpty = [null, "  \t\n", ""];
        // none
        Assert.IsTrue(CheckHelper.NoneBlank(noEmpty));
        Assert.IsFalse(CheckHelper.NoneBlank(oneEmpty));
        Assert.IsFalse(CheckHelper.NoneBlank(oneBlank));
        Assert.IsFalse(CheckHelper.NoneBlank(blankAndEmpty));
        // any one
        Assert.IsFalse(CheckHelper.AnyBlank(noEmpty));
        Assert.IsTrue(CheckHelper.AnyBlank(oneEmpty));
        Assert.IsTrue(CheckHelper.AnyBlank(oneBlank));
        Assert.IsTrue(CheckHelper.AnyBlank(blankAndEmpty));
        // any not
        Assert.IsTrue(CheckHelper.AnyNotBlank(noEmpty));
        Assert.IsTrue(CheckHelper.AnyNotBlank(oneEmpty));
        Assert.IsTrue(CheckHelper.AnyNotBlank(oneBlank));
        Assert.IsFalse(CheckHelper.AnyNotBlank(blankAndEmpty));
        // all
        Assert.IsFalse(CheckHelper.AllBlank(noEmpty));
        Assert.IsFalse(CheckHelper.AllBlank(oneEmpty));
        Assert.IsFalse(CheckHelper.AllBlank(oneBlank));
        Assert.IsTrue(CheckHelper.AllBlank(blankAndEmpty));
    }

    [Test]
    public void Null()
    {
        object[] noneNull = [false, "", 0];
        object?[] oneNull = [false, null, 0];
        object?[] allNull = [null, null];
        // none
        Assert.IsTrue(CheckHelper.NoneNull(noneNull));
        Assert.IsFalse(CheckHelper.NoneNull(oneNull));
        Assert.IsFalse(CheckHelper.NoneNull(allNull));
        // any one
        Assert.IsFalse(CheckHelper.AnyNull(noneNull));
        Assert.IsTrue(CheckHelper.AnyNull(oneNull));
        Assert.IsTrue(CheckHelper.AnyNull(allNull));
        // any not
        Assert.IsTrue(CheckHelper.AnyNotNull(noneNull));
        Assert.IsTrue(CheckHelper.AnyNotNull(oneNull));
        Assert.IsFalse(CheckHelper.AnyNotNull(allNull));
        // all
        Assert.IsFalse(CheckHelper.AllNull(noneNull));
        Assert.IsFalse(CheckHelper.AllNull(oneNull));
        Assert.IsTrue(CheckHelper.AllNull(allNull));
    }
}