using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace VanillaSugar.Core
{
    /// <summary>
    /// 提供用于检查各种集合是否为空以及比较字典的扩展方法。
    /// </summary>
    public static class CheckExtensions
    {
        /// <summary>
        /// 检查字符串的长度是否为空。
        /// </summary>
        /// <param name="target">要检查的字符串。</param>
        /// <returns>如果长度为空，则返回true；否则返回false。</returns>
        public static bool IsEmpty(this string target) => target.Length == 0;

        /// <summary>
        /// 检查 StringBuilder 的长度是否为空。
        /// </summary>
        /// <param name="target">要检查的StringBuilder。</param>
        /// <returns>如果长度为空，则返回true；否则返回false。</returns>
        public static bool IsEmpty(this StringBuilder target) => target.Length == 0;

        /// <summary>
        /// 检查集合是否为空。
        /// </summary>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <param name="target">要检查的集合。</param>
        /// <returns>如果集合为空，则返回true；否则返回false。</returns>
        public static bool IsEmpty<T>(this ICollection<T> target) => target.Count == 0;

        /// <summary>
        /// 检查数组是否为空。
        /// </summary>
        /// <typeparam name="T">数组中元素的类型。</typeparam>
        /// <param name="array">要检查的数组。</param>
        /// <returns>如果数组为空，则返回true；否则返回false。</returns>
        public static bool IsEmpty<T>(this T[] array) => array.Length == 0;

        /// <summary>
        /// 检查可枚举对象 <see cref="IEnumerable"/> 是否为空。
        /// </summary>
        /// <typeparam name="T">IEnumerable中元素的类型。</typeparam>
        /// <param name="target">要检查的IEnumerable。</param>
        /// <returns>如果对象中没有枚举出任何元素，则返回true；否则返回false。</returns>
        public static bool IsEmpty<T>(this IEnumerable target)
        {
            switch (target)
            {
                case string s:
                    return s.Length == 0;
                case ICollection<T> collection:
                    return collection.Count == 0;
                case ICollection collection:
                    return collection.Count == 0;
            }

            foreach (object unused in target)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 比较两个字典是否相等。
        /// 当两个字典的元素数量相等，所有的键都相同，且每个键所对应的值也相同时，则两个字典才视为相等。
        /// <para>
        /// 比较字典键时通过 <paramref name="target"/> 自带的比较器进行比较，行为与 <see cref="IDictionary{TKey,TValue}.ContainsKey(TKey)"/> 相同。
        /// 比较键对应的值时使用默认的比较器 <see cref="IEqualityComparer{T}"/> 进行比较。
        /// </para>
        /// </summary>
        /// <typeparam name="TKey">字典键的类型。</typeparam>
        /// <typeparam name="TValue">字典值的类型。</typeparam>
        /// <param name="target">要比较的第一个字典。</param>
        /// <param name="other">要比较的第二个字典。</param>
        /// <returns>如果两个字典相等，则返回true；否则返回false。</returns>
        public static bool DictionaryEqual<TKey, TValue>(this IDictionary<TKey, TValue> target,
            IDictionary<TKey, TValue> other)
        {
            return DictionaryEqual(target, other, EqualityComparer<TValue>.Default);
        }

        /// <summary>
        /// 比较两个字典是否相等。
        /// 当两个字典的元素数量相等，所有的键都相同，且每个键所对应的值也相同时，则两个字典才视为相等。
        /// <para>
        /// 比较字典键时通过 <paramref name="target"/> 自带的比较器进行比较，行为与 <see cref="IDictionary{TKey,TValue}.ContainsKey(TKey)"/> 相同。
        /// 比较键对应的字典值时通过 <paramref name="valueComparer"/> 来指定的 <see cref="IEqualityComparer{T}"/> 进行比较。
        /// </para>
        /// </summary>
        /// <typeparam name="TKey">字典键的类型。</typeparam>
        /// <typeparam name="TValue">字典值的类型。</typeparam>
        /// <param name="target">要比较的第一个字典。</param>
        /// <param name="other">要比较的第二个字典。</param>
        /// <param name="valueComparer">用于比较字典值的比较器。</param>
        /// <returns>如果两个字典相等，则返回true；否则返回false。</returns>
        public static bool DictionaryEqual<TKey, TValue>(this IDictionary<TKey, TValue> target,
            IDictionary<TKey, TValue> other, IEqualityComparer<TValue> valueComparer)
        {
            if (ReferenceEquals(target, other)) return true;
            if (target == null || other == null || target.Count != other.Count) return false;
            using (IEnumerator<KeyValuePair<TKey, TValue>> enumerator = other.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<TKey, TValue> pair = enumerator.Current;
                    if (!target.TryGetValue(pair.Key, out TValue value) || !valueComparer.Equals(pair.Value, value))
                        return false;
                }

                return true;
            }
        }
    }
}