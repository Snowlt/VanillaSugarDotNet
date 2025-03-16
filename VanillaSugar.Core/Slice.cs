using System;
using System.Collections.Generic;
using System.Linq;

namespace VanillaSugar.Core
{
    /// <summary>
    /// 切片，为可索引对象提供增强的裁剪计算操作。
    /// 同时为以下类型提供了扩展方法，可直接进行切片操作：
    /// <list type="bullet">
    ///     <item><see cref="IList{T}"/> 列表</item>
    ///     <item><see cref="IReadOnlyList{T}"/> 只读列表</item>
    ///     <item><see cref="ICollection{T}"/> 可枚举集合（会转为列表进行处理）</item>
    ///     <item><see cref="string"/> 字符串</item>
    ///     <item>数组(<c>T[]</c>)</item>
    /// </list>
    /// <para>每次切片操作的返回值都是新对象，不会对源对象产生副作用。可以用于替代 <see cref="List{T}.GetRange(int, int)"/>、<see cref="string.Substring(int, int)"/> 等常用方法。</para>
    ///
    /// <para>主要用法为：<c>target.Slice(start, stop, step)</c></para>
    /// 参数作用如下：<list type="bullet">
    ///     <item><c>target</c> 进行切片操作的源对象</item>
    ///     <item><c>start</c> 操作的起点下标(包含)：下标从0开始，负数下标表示从右往左数，null 表示不做限制</item>
    ///     <item><c>stop</c> 操作的结束下标(不包含)：下标从0开始，负数下标表示从右往左数，null 表示不做限制</item>
    ///     <item><c>step</c> 步长：正数表示从左到右操作，负数表示从右到左操作，null 表示取默认值 1；
    ///     不可为 0，否则会抛出 <see cref="ArgumentException"/></item>
    /// </list>
    ///
    /// <para>设计思路来自 Python 中的切片操作，使用方式也相同</para>
    /// 例如:
    /// <code>
    /// int[] a = new int[] {1, 2, 3, 4, 5};
    /// int[] b = Slice.Slice(a, 1, 4, null);     // 截取 -> b = [2, 3, 4]
    /// int[] c = Slice.Slice(a, null, null, -1); // 逆序 -> c = [5, 4, 3, 2, 1]
    /// </code>
    /// 对应 Python 中:
    /// <code>
    /// a = [1, 2, 3, 4, 5]
    /// b = a[1:4]           # -> b = [2, 3, 4]
    /// c = a[::-1]          # -> c = [5, 4, 3, 2, 1]
    /// </code>
    /// </summary>
    public readonly struct Slice
    {
        /// <summary>
        /// 切片的起始位置（包括）
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// 切片的结束位置（包括）
        /// </summary>
        public int StopIncluded { get; }

        /// <summary>
        /// 步长
        /// </summary>
        public int Step { get; }

        /// <summary>
        /// 按其他参数执行切片操作后，得到的元素的数量
        /// </summary>
        public int Size { get; }


        /**
         * 初始化一个切片对象
         *
         * @param start  起始下标(包含)
         * @param stop   结束下标(不包含)
         * @param step   步长(默认为1，不能为0)
         * @param length 原始序列的总长度
         * @throws ArgumentException 当步长为 0 时抛出
         */
        public Slice(int? start, int? stop, int? step, int length)
        {
            int left, right, stopExcluded;
            this.Step = step ?? 1;
            if (this.Step > 0)
            {
                left = start ?? 0;
                right = stop ?? length;
                this.Start = Limit(left < 0 ? left + length : left, 0, length);
                stopExcluded = Limit(right < 0 ? right + length : right, 0, length);
            }
            else if (this.Step < 0)
            {
                left = start == null ? length - 1 : (start.Value < 0 ? start.Value + length : start.Value);
                right = stop == null ? -1 : (stop.Value < 0 ? stop.Value + length : stop.Value);
                this.Start = Limit(left, -1, length - 1);
                stopExcluded = Limit(right, -1, length - 1);
            }
            else
            {
                throw new ArgumentException($"Argument {nameof(step)} cannot be zero");
            }

            StopIncluded = this.Step > 0 ? stopExcluded - 1 : stopExcluded + 1;
            if (
                (this.Step > 0 && (this.Start >= stopExcluded || this.Start >= length)) ||
                (this.Step < 0 && (this.Start <= stopExcluded || this.Start < 0))
            ) Size = 0;
            else Size = Math.Abs((StopIncluded - this.Start) / this.Step) + 1;
        }

        /// <summary>
        /// 对切片后得到下标的进行遍历
        /// </summary>
        /// <param name="consumer">回调函数，传入旧/新下标</param>
        public void ForEachIndex(Action<int, int> consumer)
        {
            int newIndex = 0;
            if (Step > 0)
            {
                for (int i = Start; i <= StopIncluded; i += Step)
                {
                    consumer.Invoke(i, newIndex++);
                }
            }
            else
            {
                for (int i = Start; i >= StopIncluded; i += Step)
                {
                    consumer.Invoke(i, newIndex++);
                }
            }
        }

        private static int Limit(int value, int min, int max)
        {
            return value < min ? min : Math.Min(value, max);
        }

        public override string ToString()
        {
            return "Slice{start=" + Start + ", stopIncluded=" + StopIncluded + ", step=" + Step + ", size=" + Size +
                   '}';
        }
    }

    public static class SliceExtensions
    {
        /// <summary>
        /// 对列表进行切片操作
        /// </summary>
        /// <typeparam name="T">元素类型(泛型)</typeparam>
        /// <param name="list">列表</param>
        /// <param name="start">起始下标(包含)</param>
        /// <param name="stop">结束下标(不包含)</param>
        /// <param name="step">步长(默认为1，不能为0)</param>
        /// <returns>新生成的列表</returns>
        /// <exception cref="ArgumentException">当步长为 0 时抛出</exception>
        public static IList<T> Slice<T>(this IList<T> list, int? start = null, int? stop = null, int? step = null)
        {
            Slice slice = new Slice(start, stop, step, list.Count);
            IList<T> nl = new List<T>(slice.Size);
            slice.ForEachIndex((i, n) => nl.Add(list[i]));
            return nl;
        }

        /// <summary>
        /// 对 <see cref="IEnumerable{T}"/> 对象进行切片操作。可能会对此对象中的元素遍历多次。
        /// </summary>
        /// <typeparam name="T">元素类型(泛型)</typeparam>
        /// <param name="enumerable">集合</param>
        /// <param name="start">起始下标(包含)</param>
        /// <param name="stop">结束下标(不包含)</param>
        /// <param name="step">步长(默认为1，不能为0)</param>
        /// <returns>新生成的列表</returns>
        /// <exception cref="ArgumentException">当步长为 0 时抛出</exception>
        public static IEnumerable<T> Slice<T>(this IEnumerable<T> enumerable, int? start = null, int? stop = null,
            int? step = null)
        {
            switch (enumerable)
            {
                case IList<T> list:
                    return list.Slice(start, stop, step);
                case IReadOnlyList<T> readOnlyList:
                {
                    Slice slice = new Slice(start, stop, step, readOnlyList.Count);
                    IList<T> nl = new List<T>(slice.Size);
                    slice.ForEachIndex((i, n) => nl.Add(readOnlyList[i]));
                    return nl;
                }
                default:
                    return enumerable.ToArray().Slice(start, stop, step);
            }
        }

        /// <summary>
        /// 对字符序列进行切片操作
        /// </summary>
        /// <param name="s">字符序列</param>
        /// <param name="start">起始下标(包含)</param>
        /// <param name="stop">结束下标(不包含)</param>
        /// <param name="step">步长(默认为1，不能为0)</param>
        /// <returns>处理后生成的字符串</returns>
        /// <exception cref="ArgumentException">当步长为 0 时抛出</exception>
        public static string Slice(this string s, int? start = null, int? stop = null, int? step = null)
        {
            Slice slice = new Slice(start, stop, step, s.Length);
            char[] chars = new char[slice.Size];
            slice.ForEachIndex((i, n) => chars[n] = s[i]);
            return new string(chars);
        }

        /// <summary>
        /// 对数组进行切片操作
        /// </summary>
        /// <param name="array">数组</param>
        /// <param name="start">起始下标(包含)</param>
        /// <param name="stop">结束下标(不包含)</param>
        /// <param name="step">步长(默认为1，不能为0)</param>
        /// <returns>新生成的数组</returns>
        /// <exception cref="ArgumentException">当步长为 0 时抛出</exception>
        public static T[] Slice<T>(this T[] array, int? start = null, int? stop = null, int? step = null)
        {
            Slice slice = new Slice(start, stop, step, array.Length);
            T[] a = new T[slice.Size];
            slice.ForEachIndex((i, n) => a[n] = array[i]);
            return a;
        }
    }
}