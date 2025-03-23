using System;
using System.Collections.Generic;
using System.Linq;

namespace VanillaSugar.Core
{
    /// <summary>
    /// 提供基于 <see cref="Random"/> 的常用随机操作静态方法。
    /// </summary>
    public static class RandHelper
    {
        private static readonly Random Random = new Random();

        /// <summary>
        /// 从列表中随机选取一个元素并返回
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">列表</param>
        /// <returns>选取的新元素</returns>
        /// <exception cref="ArgumentNullException">如果 list 为 null</exception>
        /// <exception cref="ArgumentException">如果 list 长度为 0</exception>
        public static T Choice<T>(IList<T> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (list.Count == 0) throw new ArgumentException("List cannot be empty.");

            return list[NextInt(list.Count)];
        }

        /// <summary>
        /// 从数组中随机选取一个元素并返回
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <returns>选取的新元素</returns>
        /// <exception cref="ArgumentNullException">如果 array 为 null</exception>
        /// <exception cref="ArgumentException">如果 array 长度为 0</exception>
        public static T Choice<T>(T[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (array.Length == 0) throw new ArgumentException("Array cannot be empty.");

            return array[NextInt(array.Length)];
        }

        /// <summary>
        /// 从列表中随机选取 k 个可重复的元素，并将选取的元素组装为新列表返回
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">列表</param>
        /// <param name="k">选取数量</param>
        /// <returns>选取的多个新元素</returns>
        /// <exception cref="ArgumentNullException">如果 list 为 null</exception>
        /// <exception cref="ArgumentException">如果 list 长度为 0 或 k &lt; 0</exception>
        public static IList<T> Choices<T>(IList<T> list, int k)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (list.Count == 0) throw new ArgumentException("List cannot be empty.");
            if (k < 0) throw new ArgumentException("k must be non-negative.");

            int n = list.Count;
            List<T> result = new List<T>(k);
            for (int i = 0; i < k; i++)
            {
                result.Add(list[NextInt(n)]);
            }

            return result;
        }

        /// <summary>
        /// 从列表中无重复的随机抽样 k 个元素，将结果组成一个新列表并返回
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">列表</param>
        /// <param name="k">选取数量</param>
        /// <returns>按顺序随机抽样出的 k 个元素</returns>
        /// <exception cref="ArgumentNullException">如果 list 为 null</exception>
        /// <exception cref="ArgumentException">如果 list 长度为 0 ，k &gt; list 长度或 k &lt; 0</exception>
        public static IList<T> Sample<T>(IList<T> list, int k)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (list.Count == 0) throw new ArgumentException("List cannot be empty.");
            if (k < 0 || k > list.Count) throw new ArgumentException("k must be between 0 and the list size.");

            if (k == 1) return new List<T>(1) {Choice(list)};
            // 算法从 Python 库中的 random.sample 移植 https://github.com/python/cpython/blob/3.12/Lib/random.py#L359
            // 备注：思路类似 shuffle 方法中打乱顺序，交换位置的同时进行选择，只需重复 k 次即可
            List<T> result = new List<T>(k);
            T[] pool = list.ToArray();
            int n = pool.Length;
            for (int i = 0; i < k; i++)
            {
                int j = NextInt(n - i);
                result.Add(pool[j]);
                pool[j] = pool[n - 1 - i];
            }

            return result;
        }

        /// <summary>
        /// 将 List 中的元素打乱顺序，组成新的 List 返回，原对象不会被修改
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">原始未打乱的 List</param>
        /// <returns>打乱顺序的新 List</returns>
        /// <exception cref="ArgumentNullException">list 为 null</exception>
        public static List<T> Shuffle<T>(List<T> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));

            List<T> result = new List<T>(list);
            ShuffleSelf(result);
            return result;
        }

        /// <summary>
        /// 将数组中的元素打乱顺序，组成新的数组返回，原对象不会被修改
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">原始未打乱的数组</param>
        /// <returns>打乱顺序的新数组</returns>
        /// <exception cref="ArgumentNullException">数组为 null</exception>
        public static T[] Shuffle<T>(T[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            T[] result = (T[]) array.Clone();
            ShuffleSelf(result);
            return result;
        }

        /// <summary>
        /// 将给定 List 中的元素顺序随机打乱
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">要打乱的 List</param>
        /// <exception cref="ArgumentNullException">list 为 null</exception>
        public static void ShuffleSelf<T>(List<T> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));

            for (int i = list.Count; i > 1; i--)
            {
                int j = NextInt(i);
                (list[j], list[i - 1]) = (list[i - 1], list[j]);
            }
        }

        /// <summary>
        /// 将给定数组中的元素顺序随机打乱
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">要打乱的数组</param>
        /// <exception cref="ArgumentNullException">array 为 null</exception>
        public static void ShuffleSelf<T>(T[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            for (int i = array.Length; i > 1; i--)
            {
                int j = NextInt(i);
                (array[j], array[i - 1]) = (array[i - 1], array[j]);
            }
        }

        /// <summary>
        /// 返回一个 int 随机数 x， 0 &lt;= x &lt; max
        /// </summary>
        /// <param name="max">最大数（不包含）</param>
        /// <returns>随机数</returns>
        /// <exception cref="ArgumentException">如果 max &lt;= 0</exception>
        public static int NextInt(int max)
        {
            if (max <= 0) throw new ArgumentException("max must be greater than 0.");
            return Random.Next(max);
        }

        /// <summary>
        /// 返回一个 int 随机数 x， min &lt;= x &lt; max
        /// </summary>
        /// <param name="min">最小数（包含）</param>
        /// <param name="max">最大数（不包含）</param>
        /// <returns>随机数</returns>
        /// <exception cref="ArgumentException">如果 max &lt;= min</exception>
        public static int NextInt(int min, int max)
        {
            if (max <= min) throw new ArgumentException("max must be greater than min.");
            return Random.Next(min, max);
        }

        /// <summary>
        /// 返回一个 long 随机数 x， 0 &lt;= x &lt; max
        /// </summary>
        /// <param name="max">最大数（不包含）</param>
        /// <returns>随机数</returns>
        /// <exception cref="ArgumentException">如果 max &lt;= 0</exception>
        public static long NextLong(long max)
        {
            if (max <= 0) throw new ArgumentException("max must be greater than 0.");
            return NextLong(0, max);
        }

        /// <summary>
        /// 返回一个 long 随机数 x， min &lt;= x &lt; max
        /// </summary>
        /// <param name="min">最小数（包含）</param>
        /// <param name="max">最大数（不包含）</param>
        /// <returns>随机数</returns>
        /// <exception cref="ArgumentException">如果 max &lt;= min</exception>
        public static long NextLong(long min, long max)
        {
            if (max <= min) throw new ArgumentException("max must be greater than min.");
            return (long) NextDouble(min, max);
        }

        /// <summary>
        /// 返回一个 float 随机数 x， 0.0F &lt;= x &lt; max
        /// </summary>
        /// <param name="max">最大数（不包含）</param>
        /// <returns>随机数</returns>
        /// <exception cref="ArgumentException">如果 max &lt;= 0.0</exception>
        public static float NextFloat(float max)
        {
            if (max <= 0.0f) throw new ArgumentException("max must be greater than 0.0.");
            return NextFloat(0, max);
        }

        /// <summary>
        /// 返回一个 float 随机数 x， min &lt;= x &lt; max
        /// </summary>
        /// <param name="min">最小数（包含）</param>
        /// <param name="max">最大数（不包含）</param>
        /// <returns>随机数</returns>
        /// <exception cref="ArgumentException">如果 max &lt;= min</exception>
        public static float NextFloat(float min, float max)
        {
            if (max <= min) throw new ArgumentException("max must be greater than min.");
            return min + (float) (Random.NextDouble() * (max - min));
        }

        /// <summary>
        /// 返回一个 double 随机数 x， 0.0 &lt;= x &lt; 1.0
        /// </summary>
        /// <returns>随机数</returns>
        public static double NextDouble()
        {
            return Random.NextDouble();
        }

        /// <summary>
        /// 返回一个 double 随机数 x， 0.0 &lt;= x &lt; max
        /// </summary>
        /// <param name="max">最大数（不包含）</param>
        /// <returns>随机数</returns>
        /// <exception cref="ArgumentException">如果 max &lt;= 0.0</exception>
        public static double NextDouble(double max)
        {
            if (max <= 0.0) throw new ArgumentException("max must be greater than 0.0.");
            return NextDouble(0, max);
        }

        /// <summary>
        /// 返回一个 double 随机数 x， min &lt;= x &lt; max
        /// </summary>
        /// <param name="min">最小数（包含）</param>
        /// <param name="max">最大数（不包含）</param>
        /// <returns>随机数</returns>
        /// <exception cref="ArgumentException">如果 max &lt;= min</exception>
        public static double NextDouble(double min, double max)
        {
            if (max <= min) throw new ArgumentException("max must be greater than min.");
            return min + ((max - min) * Random.NextDouble());
        }

        /// <summary>
        /// 返回一个随机 boolean 值
        /// </summary>
        /// <returns>true 或 false</returns>
        public static bool NextBool()
        {
            return Random.Next(1) == 1;
        }

        /// <summary>
        /// 返回一个 int 随机数 x， minInclusive &lt;= x &lt;= max
        /// </summary>
        /// <param name="minInclusive">最小数（包含）</param>
        /// <param name="maxInclusive">最大数（包含）</param>
        /// <returns>随机数</returns>
        /// <exception cref="ArgumentException">如果 maxInclusive &lt; minInclusive</exception>
        public static int RandIntBetween(int minInclusive, int maxInclusive)
        {
            if (maxInclusive < minInclusive)
                throw new ArgumentException("maxInclusive must be greater than or equal to minInclusive.");
            return (int) NextLong(minInclusive, maxInclusive + 1L);
        }

        /// <summary>
        /// 生成由 n 个随机字节组成的数组
        /// </summary>
        /// <param name="n">生成的字节数量</param>
        /// <returns>随机生成的字节数组</returns>
        /// <exception cref="ArgumentException">如果 n &lt; 0</exception>
        public static byte[] RandBytes(int n)
        {
            if (n < 0) throw new ArgumentException("n must be non-negative.");
            byte[] result = new byte[n];
            Random.NextBytes(result);
            return result;
        }
    }
}