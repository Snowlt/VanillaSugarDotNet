using System.Collections;
using VanillaSugar.Core;

namespace Test;

[TestFixture]
[TestOf(typeof(Slice))]
public class SliceTest
{
    [Test]
    public void SliceForwardTest()
    {
        // 正向
        int[] a = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        Assert.AreEqual(new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10}, a.Slice(null, null, null));
        Assert.AreEqual(new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10}, a.Slice());
        Assert.AreEqual(new[] {0, 3, 6, 9}, a.Slice(null, null, 3));
        Assert.AreEqual(new[] {0, 3, 6, 9}, a.Slice(0, 10, 3));
        Assert.AreEqual(new[] {1, 4, 7, 10}, a.Slice(1, null, 3));
        Assert.AreEqual(new[] {2, 5, 8}, a.Slice(2, null, 3));
        Assert.AreEqual(new[] {0, 3, 6}, a.Slice(null, 9, 3));
        Assert.AreEqual(new[] {0, 3, 6, 9}, a.Slice(null, 10, 3));
        Assert.AreEqual(new[] {2, 4, 6, 8}, a.Slice(-9, -1, 2));
        Assert.AreEqual(new[] {2, 4, 6, 8, 10}, a.Slice(-9, null, 2));
        Assert.AreEqual(new[] {10}, a.Slice(-1, null, 3));
        Assert.AreEqual(new[] {9}, a.Slice(-2, -1, 1));

        // 正向2
        int[] a2 = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
        Assert.AreEqual(new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}, a2.Slice(0, null, 1));
        Assert.AreEqual(new[] {0, 2, 4, 6, 8}, a2.Slice(null, null, 2));
        Assert.AreEqual(new[] {0, 2, 4, 6, 8}, a2.Slice(0, 10, 2));
        Assert.AreEqual(new[] {0, 2, 4, 6, 8}, a2.Slice(0, 9, 2));
        Assert.AreEqual(new[] {0, 2, 4, 6, 8}, a2.Slice(-10, -1, 2));
        Assert.AreEqual(new[] {1, 3, 5, 7, 9}, a2.Slice(1, null, 2));
        Assert.AreEqual(new[] {1, 3, 5, 7, 9}, a2.Slice(1, 10, 2));
        Assert.AreEqual(new[] {1, 3, 5, 7}, a2.Slice(1, 9, 2));
    }

    [Test]
    public void SliceForwardSpecialTest()
    {
        int[] a = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        // 正向空结果
        Assert.AreEqual(System.Array.Empty<int>(), a.Slice(0, 0, 1));
        Assert.AreEqual(System.Array.Empty<int>(), a.Slice(1, 1, 1));
        Assert.AreEqual(System.Array.Empty<int>(), a.Slice(2, 2, 3));
        Assert.AreEqual(System.Array.Empty<int>(), a.Slice(-1, -1, 1));
        Assert.AreEqual(System.Array.Empty<int>(), a.Slice(-2, -2, 1));
        Assert.AreEqual(System.Array.Empty<int>(), a.Slice(2, 1, 3));

        // 正向越界
        Assert.AreEqual(new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10}, a.Slice(null, 12, null));
        Assert.AreEqual(new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10}, a.Slice(-20, 20, null));
        Assert.AreEqual(new[] {0, 3, 6, 9}, a.Slice(null, 12, 3));
        Assert.AreEqual(new[] {1, 4, 7, 10}, a.Slice(1, 14, 3));
        Assert.AreEqual(System.Array.Empty<int>(), a.Slice(12, null, 3));
        Assert.AreEqual(System.Array.Empty<int>(), a.Slice(12, 14, 3));
    }

    [Test]
    public void SliceBackwardTest()
    {
        // 逆向1
        int[] a = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        Assert.AreEqual(new[] {10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0}, a.Slice(null, null, -1));
        Assert.AreEqual(new[] {10, 7, 4, 1}, a.Slice(null, null, -3));
        Assert.AreEqual(new[] {10, 7, 4}, a.Slice(10, 3, -3));
        Assert.AreEqual(new[] {10, 7, 4}, a.Slice(-1, 3, -3));
        Assert.AreEqual(new[] {10, 7, 4}, a.Slice(-1, -9, -3));
        Assert.AreEqual(new[] {9}, a.Slice(-2, -4, -2));

        // 逆向2
        int[] a2 = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
        Assert.AreEqual(new[] {9, 7, 5, 3, 1}, a2.Slice(null, null, -2));
        Assert.AreEqual(new[] {9, 7, 5, 3, 1}, a2.Slice(10, 0, -2));
        Assert.AreEqual(new[] {9, 7, 5, 3, 1}, a2.Slice(9, 0, -2));
        Assert.AreEqual(new[] {9, 7, 5, 3}, a2.Slice(9, 1, -2));
        Assert.AreEqual(new[] {9, 7, 5, 3, 1}, a2.Slice(-1, -10, -2));
        Assert.AreEqual(new[] {8, 6, 4, 2}, a2.Slice(-2, -10, -2));
        Assert.AreEqual(new[] {8, 6, 4, 2, 0}, a2.Slice(-2, null, -2));
        Assert.AreEqual(new[] {8, 6, 4, 2, 0}, a2.Slice(-2, -11, -2));
    }

    [Test]
    public void SliceBackwardSpecialTest()
    {
        int[] a = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        // 逆向空结果
        Assert.AreEqual(System.Array.Empty<int>(), a.Slice(1, 1, -1));
        Assert.AreEqual(System.Array.Empty<int>(), a.Slice(0, 0, -1));
        Assert.AreEqual(System.Array.Empty<int>(), a.Slice(0, 3, -1));
        Assert.AreEqual(System.Array.Empty<int>(), a.Slice(2, 2, -3));
        Assert.AreEqual(System.Array.Empty<int>(), a.Slice(-1, -1, -1));
        Assert.AreEqual(System.Array.Empty<int>(), a.Slice(-5, -5, -3));

        // 逆向越界
        Assert.AreEqual(new[] {10, 7, 4}, a.Slice(12, 3, -3));
        Assert.AreEqual(System.Array.Empty<int>(), a.Slice(-12, 3, -3));
    }

    [Test]
    public void ListAndCollection()
    {
        IList<long> list = [1, 2, 3, 4, 5];
        var readOnlyList = new ReadOnlyWrapList<long>(list);
        CollectionAssert.AreEqual(new List<long> {5, 3}, readOnlyList.Slice(-1, 0, -2));
        CollectionAssert.AreEqual(new List<long> {5, 3, 1}, readOnlyList.Slice(-1, null, -2));

        Queue<long> queue = new Queue<long>(list);
        CollectionAssert.AreEqual(new List<long> {5, 3}, queue.Slice(-1, 0, -2));
        CollectionAssert.AreEqual(new List<long> {5, 3, 1}, queue.Slice(-1, null, -2));
    }

    private class ReadOnlyWrapList<T>(IList<T> list) : IReadOnlyList<T>
    {
        private readonly IList<T> _list = list;

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _list.Count;

        public T this[int index] => _list[index];
    }

    [Test]
    public void CharSequence()
    {
        string s = "a1b2c3d4e5f6g7";
        Assert.AreEqual("abc", s.Slice(null, 5, 2));
        Assert.AreEqual("abc", s.Slice(null, 6, 2));
        Assert.AreEqual("1234567", s.Slice(1, s.Length, 2));
        Assert.AreEqual("7654321", s.Slice(-1, 0, -2));
    }

    [Test]
    public void Array()
    {
        char[] characters = ['A', 'B', 'C', 'D', 'E', 'F'];
        Assert.AreEqual(new[] {'B', 'D', 'F'}, characters.Slice(1, null, 2));

        double[] doubles = [1.0, 2.2, 3.3, 4.4, 5.5];
        Assert.AreEqual(new[] {1.0, 3.3, 5.5}, doubles.Slice(null, null, 2));
    }
}