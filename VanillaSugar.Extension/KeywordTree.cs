using System;
using System.Collections.Generic;
using System.Text;

namespace VanillaSugar.Extension
{
    /// <summary>
    /// 关键词树
    /// <para>用于快速在文本中匹配多个字符串或子字符串。</para>
    /// </summary>
    public class KeywordTree
    {
        private class TreeNode
        {
            public bool Boundary { get; set; }
            public Dictionary<char, TreeNode> Next { get; private set; }

            public TreeNode GetNextLevel(char c)
            {
                return Next != null && Next.TryGetValue(c, out var value) ? value : null;
            }

            public TreeNode GetNextLevelOrAdd(char c)
            {
                if (Next == null)
                {
                    Next = new Dictionary<char, TreeNode>();
                }

                if (!Next.ContainsKey(c))
                {
                    Next[c] = new TreeNode();
                }

                return Next[c];
            }
        }

        private readonly TreeNode _root = new TreeNode();

        /// <summary>
        /// 获取最短的字符长度。
        /// </summary>
        public int MinLength { get; private set; } = 0;

        /// <summary>
        /// 获取最长的字符长度。
        /// </summary>
        public int MaxLength { get; private set; } = 0;


        /// <summary>
        /// 添加关键词。
        /// </summary>
        /// <param name="keyword">关键词。</param>
        /// <exception cref="ArgumentException">如果关键词为空则抛出</exception>
        public void Add(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                throw new ArgumentException("keyword is empty");
            }

            TreeNode node = _root;
            int length = keyword.Length;
            int endInclusive = length - 1;
            for (int i = 0; i <= endInclusive; i++)
            {
                char c = keyword[i];
                node = node.GetNextLevelOrAdd(c);
                if (i == endInclusive)
                {
                    node.Boundary = true;
                }
            }

            MinLength = MinLength == 0 ? length : Math.Min(MinLength, length);
            MaxLength = Math.Max(MaxLength, length);
        }

        /// <summary>
        /// 根据树中存放的关键词生成集合。
        /// </summary>
        /// <returns>集合。</returns>
        public ISet<string> ToKeywordsSet()
        {
            HashSet<string> set = new HashSet<string>();
            StringBuilder builder = new StringBuilder(MaxLength);
            RecursivelyTravel(_root, builder, set);
            return set;
        }

        /// <summary>
        /// 判断指定的字符串中是否包含树中的任意关键词。
        /// </summary>
        /// <param name="target">待判断字符串。</param>
        /// <returns>包含的关键词。</returns>
        /// <exception cref="ArgumentNullException">如果传入值为 null</exception>
        public bool ContainsAny(string target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (target.Length == 0) return false;

            int length = target.Length;
            int searchEnding = length - MinLength + 1;
            for (int i = 0; i < searchEnding; i++)
            {
                int keywordLength = MatchKeywordLength(target, length, i, true);
                if (keywordLength != 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 在指定的字符串中匹配首个出现的关键词。
        /// </summary>
        /// <param name="target">待判断字符串。</param>
        /// <param name="matchShortestText">如果关键词之间形成了子字符串，优先匹配最短的关键词。</param>
        /// <returns>匹配到的首个关键词，未匹配到则返回 null。</returns>
        /// <exception cref="ArgumentNullException">如果传入值为 null</exception>
        public string MatchFirstKeyword(string target, bool matchShortestText)
        {
            return MatchFirstKeyword(target, 0, target.Length, matchShortestText);
        }

        /// <summary>
        /// 在指定的字符串中匹配首个出现的关键词。
        /// </summary>
        /// <param name="target">待判断字符串。</param>
        /// <param name="start">字符串起始位置（包括）。</param>
        /// <param name="end">字符串结束位置（不包括）。</param>
        /// <param name="matchShortestText">如果关键词之间形成了子字符串，优先匹配最短的关键词。</param>
        /// <returns>匹配到的首个关键词，未匹配到则返回 null。</returns>
        /// <exception cref="ArgumentNullException">如果传入值为 null</exception>
        public string MatchFirstKeyword(string target, int start, int end, bool matchShortestText)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (target.Length == 0) return null;


            if (start < 0 || start >= end - 1)
            {
                throw new ArgumentException("illegal range, start=" + start + ", end=" + end);
            }

            int searchEnding = end - MinLength + 1;
            for (int i = start; i < searchEnding; i++)
            {
                int length = MatchKeywordLength(target, end, i, matchShortestText);
                if (length != 0)
                {
                    return target.Substring(i, length);
                }
            }

            return null;
        }

        /// <summary>
        /// 从指定位置开始匹配关键词，并返回匹配到的长度。
        /// </summary>
        /// <param name="target">字符串。</param>
        /// <param name="length">字符串长度。</param>
        /// <param name="pos">开始匹配位置。</param>
        /// <param name="matchShortestText">匹配最短文本。</param>
        /// <returns>匹配到的长度，0是未匹配。</returns>
        internal int MatchKeywordLength(string target, int length, int pos, bool matchShortestText)
        {
            int matchedLength = 0;
            TreeNode node = _root;
            for (int i = pos; i < length; i++)
            {
                char c = target[i];
                node = node.GetNextLevel(c);
                if (node == null)
                {
                    break;
                }

                if (node.Boundary)
                {
                    matchedLength = i - pos + 1;
                    if (matchShortestText)
                    {
                        break;
                    }
                }
            }

            return matchedLength;
        }

        private void RecursivelyTravel(TreeNode node, StringBuilder builder, HashSet<string> keywords)
        {
            if (node.Boundary)
            {
                keywords.Add(builder.ToString());
            }

            if (node.Next == null || node.Next.Count == 0)
            {
                return;
            }

            foreach (var entry in node.Next)
            {
                builder.Append(entry.Key);
                RecursivelyTravel(entry.Value, builder, keywords);
                builder.Length -= 1;
            }
        }
    }
}