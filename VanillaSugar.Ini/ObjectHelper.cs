using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VanillaSugar.Ini
{
    internal static class ObjectHelper
    {
        public static void AssertNotNull(object argument, string argumentName)
        {
            if (argument == null) throw new ArgumentNullException(argumentName);
        }

        private static readonly IReadOnlyList<string> EmptyStringList =
            UnmodifiableList<string>.Wrap(new List<string>(0));

        public static IReadOnlyList<string> WrapList(this IList<string> list)
        {
            return list == null || list.Count == 0 ? EmptyStringList : UnmodifiableList<string>.Wrap(list);
        }

        public static bool IsEmpty<T>(this ICollection<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static bool ContentEqual<T>(this ICollection<T> obj, ICollection<T> other)
        {
            if (ReferenceEquals(obj, other)) return true;
            if (obj == null || other == null || obj.Count != other.Count) return false;
            return obj.SequenceEqual(other);
        }

        public static bool DictEqual<TKey, TValue>(this IDictionary<TKey, TValue> obj, IDictionary<TKey, TValue> other)
        {
            if (ReferenceEquals(obj, other)) return true;
            if (obj == null || other == null || obj.Count != other.Count) return false;
            IEnumerator<KeyValuePair<TKey, TValue>> enumerator = obj.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<TKey, TValue> pair = enumerator.Current;
                    if (!other.TryGetValue(pair.Key, out TValue value) || !Equals(pair.Value, value))
                        return false;
                }

                return true;
            }
            finally
            {
                enumerator.Dispose();
            }
        }
    }

    internal class UnmodifiableList<T> : IReadOnlyList<T>
    {
        public static UnmodifiableList<T> Wrap(IList<T> list)
        {
            return new UnmodifiableList<T>(list);
        }

        private readonly IList<T> _list;

        private UnmodifiableList(IList<T> list)
        {
            _list = list;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _list).GetEnumerator();
        }

        public int Count => _list.Count;

        public T this[int index] => _list[index];
    }
}