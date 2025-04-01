using System;
using System.Text;

namespace VanillaSugar.Core
{
    /// <summary>
    /// 为 <see cref="string"/> 提供部分字符串操作扩展方法。
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 返回一个由原字符串重复 <paramref name="count"/> 次后组成的字符串。
        /// 如果重复次数 &lt;= 0 则会返回空字符串。
        /// </summary>
        /// <param name="value">原字符串</param>
        /// <param name="count">要重复的次数</param>
        /// <returns>重复后的字符串。</returns>
        public static string Repeat(this string value, int count)
        {
            if (count <= 0)
                return string.Empty;
            if (count == 1)
                return value;
            if (value.Length == 1)
            {
                char ch = value[0];
                char[] chars = new char[value.Length];
                for (var i = 0; i < chars.Length; i++)
                {
                    chars[i] = ch;
                }

                return new string(chars);
            }

            StringBuilder builder = new StringBuilder(value.Length * count);
            for (int i = 0; i < count; i++)
            {
                builder.Append(value);
            }

            return builder.ToString();
        }

        /// <summary>
        /// 如果原字符串的末尾不包含 <paramref name="suffix"/> 参数指定的后缀，则在末尾添加这个后缀，否则返回原字符串。
        /// 判断是否包含同 <see cref="string.EndsWith(string, StringComparison)"/> 方法。
        /// </summary>
        /// <param name="value">原字符串</param>
        /// <param name="suffix">后缀</param>
        /// <param name="comparisonType">字符串的比较方式</param>
        /// <returns>添加了后缀的字符串或原字符串。</returns>
        public static string AppendIfMissing(this string value, string suffix,
            StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(suffix) || value.EndsWith(suffix, comparisonType))
                return value;
            return value + suffix;
        }

        /// <summary>
        /// 如果原字符串的开头不包含 <paramref name="prefix"/> 参数指定的前缀，则在开头添加这个前缀，否则返回原字符串。
        /// 判断是否包含同 <see cref="string.EndsWith(string, StringComparison)"/> 方法。
        /// </summary>
        /// <param name="value">原字符串</param>
        /// <param name="prefix">前缀</param>
        /// <param name="comparisonType">字符串的比较方式</param>
        /// <returns>添加了前缀的字符串或原字符串。</returns>
        public static string PrependIfMissing(this string value, string prefix,
            StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(prefix) || value.StartsWith(prefix, comparisonType))
                return value;
            return prefix + value;
        }

        /// <summary>
        /// 将字符串首字母大写，除了第一个字符外其他字符不会改变。
        /// </summary>
        /// <param name="value">原字符串</param>
        /// <returns>首字母大写的字符串。</returns>
        public static string Capitalize(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            char c = value[0];
            if (char.IsUpper(c))
                return value;
            return char.ToUpperInvariant(c) + value.Substring(1);
        }

        /// <summary>
        /// 将字符串首字母小写，除了第一个字符外其他字符不会改变。
        /// </summary>
        /// <param name="value">原字符串</param>
        /// <returns>首字母小写的字符串。</returns>
        public static string Uncapitalize(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            char c = value[0];
            if (char.IsLower(c))
                return value;
            return char.ToLowerInvariant(c) + value.Substring(1);
        }
    }
}