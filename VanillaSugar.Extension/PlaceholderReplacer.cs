using System;
using System.Collections.Generic;
using System.Text;

namespace VanillaSugar.Extension
{
    /// <summary>
    /// 占位符替换器。
    /// <para>添加多个占位符与替换字符串，然后在文本中进行快速查找和替换。</para>
    /// <seealso cref="KeywordTree"/>
    /// </summary>
    public class PlaceholderReplacer
    {
        private readonly KeywordTree _keywordTree = new KeywordTree();
        private readonly Dictionary<string, string> _replacements = new Dictionary<string, string>();

        public PlaceholderReplacer()
        {
        }

        public PlaceholderReplacer(IDictionary<string, string> replacements)
        {
            foreach (var pair in replacements)
            {
                replacements.Add(pair.Key, pair.Value);
                _keywordTree.Add(pair.Key);
            }
        }

        /// <summary>
        /// 添加一个占位符与对应的替换文本。
        /// <para>如果多次调用这个方法传入相同的占位符，则使用最后一次传入的替换文本。</para>
        /// </summary>
        /// <param name="placeholder">占位符（不能为 null 或空字符串）。</param>
        /// <param name="replacement">替换文本（不能为 null）。</param>
        /// <exception cref="ArgumentException">如果 <paramref name="placeholder"/> 为 null 或空字符串。</exception>
        /// <exception cref="ArgumentNullException">如果 <paramref name="replacement"/> 为 null。</exception>
        public void SetReplacement(string placeholder, string replacement)
        {
            if (replacement == null)
                throw new ArgumentNullException(nameof(replacement));
            if (string.IsNullOrEmpty(placeholder))
                throw new ArgumentException($"{nameof(placeholder)} must not be null or empty");

            if (!_replacements.ContainsKey(placeholder))
            {
                _keywordTree.Add(placeholder);
            }

            _replacements[placeholder] = replacement;
        }

        /// <summary>
        /// 判断指定的字符串中是否包含任意占位符。
        /// </summary>
        /// <param name="target">待判断字符串。</param>
        /// <returns>包含的占位符。</returns>
        public bool ContainsAnyPlaceholder(string target)
        {
            if (string.IsNullOrEmpty(target)) return false;
            return _keywordTree.ContainsAny(target);
        }

        /// <summary>
        /// 将已添加的占位符生成为集合。
        /// </summary>
        /// <returns>集合。</returns>
        public ISet<string> ToPlaceholdersSet()
        {
            return new HashSet<string>(_replacements.Keys);
        }

        /// <summary>
        /// 在指定的字符串中匹配所有出现的占位符，并将成功匹配到的部分替换为新内容。
        /// </summary>
        /// <param name="target">指定的字符串。</param>
        /// <param name="matchShortestText">如果占位符之间形成了子字符串，优先匹配最短的占位符。</param>
        /// <returns>替换占位符后的字符串。</returns>
        /// <exception cref="ArgumentNullException">如果 <paramref name="target"/> 为 null。</exception>
        /// <seealso cref="SetReplacement(string, string)"/>
        public string ReplaceAll(string target, bool matchShortestText)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (target.Length == 0) return target;

            int length = target.Length;
            StringBuilder builder = new StringBuilder(length);
            int beg = 0;
            for (int i = 0; i < length; i++)
            {
                int matchedKeywordLength = _keywordTree.MatchKeywordLength(target, length, i, matchShortestText);
                if (matchedKeywordLength != 0)
                {
                    builder.Append(target, beg, i - beg);
                    string placeholder = target.Substring(i, matchedKeywordLength);
                    var value = _replacements.TryGetValue(placeholder, out string replacement) ? replacement : null;
                    builder.Append(value ?? placeholder);
                    i += matchedKeywordLength - 1;
                    beg = i + 1;
                }
            }

            if (beg < length)
            {
                builder.Append(target, beg, length - beg);
            }

            return builder.ToString();
        }

        /// <summary>
        /// 在指定的字符串中匹配有 <paramref name="prefix"/> 作为前缀的占位符，将前缀和占位符共同替换为新内容。
        /// </summary>
        /// <param name="target">指定的字符串。</param>
        /// <param name="prefix">需要替换的占位符前缀。</param>
        /// <param name="matchShortestText">如果占位符之间形成了子字符串，优先匹配最短的占位符。</param>
        /// <returns>替换占位符后的字符串。</returns>
        /// <exception cref="ArgumentNullException">如果 <paramref name="target"/> 为 null。</exception>
        /// <seealso cref="SetReplacement(string, string)"/>
        public string ReplaceAllByPrefix(string target, char prefix, bool matchShortestText)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (target.Length == 0) return target;

            int length = target.Length;
            int end = length - 1;
            StringBuilder builder = new StringBuilder(length);
            int beg = 0;
            for (int i = 0; i < end;)
            {
                char c = target[i];
                if (c != prefix)
                {
                    i++;
                    continue;
                }

                int nextPos = i + 1;
                char nextC = target[nextPos];
                if (nextC == prefix)
                {
                    builder.Append(target, beg, nextPos - beg);
                    i += 2;
                    beg = i;
                }
                else
                {
                    int matchedKeywordLength =
                        _keywordTree.MatchKeywordLength(target, length, nextPos, matchShortestText);
                    if (matchedKeywordLength != 0)
                    {
                        builder.Append(target, beg, i - beg);
                        string placeholder = target.Substring(nextPos, matchedKeywordLength);
                        string value = _replacements.TryGetValue(placeholder, out var replacement) ? replacement : null;
                        if (value != null)
                        {
                            builder.Append(value);
                        }
                        else
                        {
                            builder.Append(prefix).Append(placeholder);
                        }

                        i += matchedKeywordLength + 1;
                        beg = i;
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            if (beg < length)
            {
                builder.Append(target, beg, length - beg);
            }

            return builder.ToString();
        }
    }
}