using VanillaSugar.Core;

namespace Test;

[TestFixture]
[TestOf(typeof(RandHelper))]
public class RandHelperTest
{
    [Test]
    public void ShuffleList()
    {
        List<int> example = GetSeq(1, 10);

        int count = 0;
        for (int i = 0; i < 10; i++)
        {
            List<int> shuffle = RandHelper.Shuffle(example);
            Assert.AreEqual(10, shuffle.Count);
            foreach (int e in shuffle)
            {
                Assert.IsTrue(example.Contains(e));
            }

            if (example.SequenceEqual(shuffle))
            {
                count++;
            }
        }

        Assert.IsTrue(count <= 2);
    }

    [Test]
    public void ShuffleArray()
    {
        int[] example = GetSeq(1, 10).ToArray();
        int[] backup = (int[]) example.Clone();
        HashSet<int> elements = [..example];
        int count = 0;
        for (int i = 0; i < 10; i++)
        {
            int[] shuffle = RandHelper.Shuffle(example);
            Assert.AreEqual(backup.Length, shuffle.Length);
            CollectionAssert.AreEqual(backup, example);
            Assert.AreNotSame(example, shuffle);
            Assert.IsTrue(elements.IsSupersetOf(shuffle));
            if (example.SequenceEqual(shuffle))
            {
                count++;
            }
        }

        Assert.IsTrue(count <= 2);
        RandHelper.ShuffleSelf(example);
        Assert.IsTrue(elements.IsSupersetOf(example));
        Assert.IsFalse(example.SequenceEqual(backup));
    }

    [Test]
    public void Sample()
    {
        List<int> list = GetSeq(1, 10);
        HashSet<int> elements = [..list];
        int size = 5;
        int loop = 10;
        HashSet<string> results = [];
        for (int i = 0; i < loop; i++)
        {
            IList<int> sample = RandHelper.Sample(list, size);
            Assert.AreEqual(size, sample.Count);
            Assert.IsTrue(elements.IsSupersetOf(sample));

            results.Add(string.Join(",", sample));
        }

        Assert.IsTrue(results.Count >= loop - 2);
    }

    [Test]
    public void RandIntInclusive()
    {
        int count = 0;
        int min = int.MaxValue - 1;
        for (int i = 100; i > 0; i--)
        {
            if (RandHelper.RandIntBetween(min, int.MaxValue) == int.MaxValue)
            {
                count++;
            }
        }

        Assert.AreNotEqual(0, count);
        Assert.Throws<ArgumentException>(() => RandHelper.RandIntBetween(2, 1));
    }

    [Test]
    public void RandBytes()
    {
        Assert.Throws<ArgumentException>(() => RandHelper.RandBytes(-1));
        Assert.AreEqual(Array.Empty<byte>(), RandHelper.RandBytes(0));
        Assert.AreEqual(3, RandHelper.RandBytes(3).Length);
    }

    private static List<int> GetSeq(int start, int stop)
    {
        return Enumerable.Range(start, stop - start + 1).ToList();
    }
}