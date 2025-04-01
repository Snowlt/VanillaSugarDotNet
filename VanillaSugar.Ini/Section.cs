using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using VanillaSugar.Ini.Exception;

namespace VanillaSugar.Ini
{
    public class Section : IEnumerable<KeyValuePair<string, string>>, IEquatable<Section>
    {
        private readonly Dictionary<string, string> _items = new Dictionary<string, string>();
        private readonly List<Node> _nodes = new List<Node>();

        /// <summary>
        /// 处于区块顶部的注释。
        /// </summary>
        private List<string> _topComments;

        /// <summary>
        /// 区块的顶部文本（不含注释前缀）。
        /// <c>null</c> 表示顶部文本不存在。
        /// 由于顶部文本不遵守注释的格式，写入 INI 文件后可能会导致兼容性问题，请谨慎使用此功能。
        /// </summary>
        public string DanglingText { get; set; }

        /// <summary>
        /// 获取所有项的键名（Key）。
        /// 键名（Key）按添加顺序排列。
        /// </summary>
        public IReadOnlyList<string> Keys => _nodes.Select(node => node.Key).ToList();

        /// <summary>
        /// 获取此区块内项（键值对）的总数。
        /// </summary>
        /// <value>项的数量</value>
        public int Count => _items.Count;

        /// <summary>
        /// 获取此区块内项（键值对）以及注释的总数。
        /// </summary>
        /// <value>项的数量</value>
        public int KeyAndCommentsCount =>
            (_topComments?.Count ?? 0) + _nodes.Sum(node => 1 + (node.Comments?.Count ?? 0));

        internal Section()
        {
        }

        /// <summary>
        /// 获取此区块中指定项（键值对）中的值。
        /// 如果键名（Key）不存在，则返回 <c>null</c>。
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>值</returns>
        /// <exception cref="ArgumentNullException">当 <c>key</c> 为 <c>null</c></exception>
        public string Get(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _items.TryGetValue(key, out string value) ? value : null;
        }

        /// <summary>
        /// 向区块中添加新项（键值对），如果区块中已有相同键名（Key）的项则会覆盖。
        /// 传入的值会自动调用 <see cref="object.ToString()"/> 转为 <see cref="string"/> 类型。
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="value">值</param>
        /// <exception cref="ArgumentNullException">当 <c>key</c> 或 <c>value</c> 为 <c>null</c></exception>
        public void Set(string key, object value)
        {
            ObjectHelper.AssertNotNull(key, nameof(key));
            ObjectHelper.AssertNotNull(value, nameof(value));
            string stringValue = value.ToString();
            bool notExisted = !_items.ContainsKey(key);
            _items[key] = stringValue;
            if (notExisted) _nodes.Add(new Node(key));
        }

        /// <summary>
        /// 获取此区块中指定项（键值对）的值，当键（Key）不存在时返回 <c>def</c> 替代。
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="def">当键名不存在时的替代返回值</param>
        /// <returns>值或替代值</returns>
        /// <exception cref="ArgumentNullException">当 <c>key</c> 为 <c>null</c></exception>
        public string Get(string key, string def)
        {
            string value = Get(key);
            return value ?? def;
        }

        /// <summary>
        /// 获取此区块中指定项（键值对）的值，并转为 <c>int</c> 返回。
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>值</returns>
        /// <exception cref="ArgumentNullException">当 <c>key</c> 为 <c>null</c></exception>
        /// <exception cref="AccessValueException">当键（Key）不存在，或值无法转换为 <c>int</c></exception>
        public int GetAsInt(string key)
        {
            string value = Get(key);
            if (int.TryParse(value, out int result)) return result;
            throw new AccessValueException($"Unable to parse value of key \"{key}\" to int");
        }

        /// <summary>
        /// 获取此区块中指定项（键值对）的值，并转为 <c>long</c> 返回。
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>值</returns>
        /// <exception cref="ArgumentNullException">当 <c>key</c> 为 <c>null</c></exception>
        /// <exception cref="AccessValueException">当键（Key）不存在，或值无法转换为 <c>long</c></exception>
        public long GetAsLong(string key)
        {
            string value = Get(key);
            if (long.TryParse(value, out long result))
            {
                return result;
            }

            throw new AccessValueException($"Unable to parse value of key \"{key}\" to long");
        }

        /// <summary>
        /// 获取此区块中指定项（键值对）的值，并转为 <c>boolean</c> 返回。
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>值</returns>
        /// <exception cref="ArgumentNullException">当 <c>key</c> 为 <c>null</c></exception>
        /// <see cref="bool.Parse(string)"/> 转换方法
        public bool GetAsBool(string key)
        {
            string value = Get(key);
            return "true".Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 检测此区块中是否包含某项（键值对）。
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>包含时返回 <c>true</c>, 不包含返回 <c>false</c></returns>
        /// <exception cref="ArgumentNullException">当 <c>key</c> 为 <c>null</c></exception>
        public bool Contains(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _items.ContainsKey(key);
        }

        /// <summary>
        /// 移除指定项（键值对）。
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>成功移除返回 <c>true</c>, 否则返回 <c>false</c></returns>
        /// <exception cref="ArgumentNullException">当 <c>key</c> 为 <c>null</c></exception>
        public bool Remove(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (_items.Remove(key) && _nodes.Count > 0)
            {
                int i = FindKeyIndex(key);
                Node node = _nodes[i];
                _nodes.RemoveAt(i);
                if (i == 0)
                {
                    AppendTopComments(node.Comments);
                }
                else
                {
                    _nodes[i - 1].AppendComments(node.Comments);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 将指定项的键名（Key）更改为新键名，如果键名（Key）不存在则什么也不做。
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="newKey">新的键名</param>
        /// <returns>成功修改返回 <c>true</c>, 否则返回 <c>false</c></returns>
        /// <exception cref="ArgumentNullException">当 <c>key</c> 或 <c>newKey</c> 为 <c>null</c></exception>
        public bool Rename(string key, string newKey)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (newKey == null) throw new ArgumentNullException(nameof(newKey));
            if (key == newKey) return false;
            string value = _items.TryGetValue(key, out var item) ? item : null;
            if (value != null)
            {
                _items.Remove(key);
                _items.Add(newKey, value);
                foreach (Node node in _nodes)
                {
                    if (node.Key.Equals(key))
                    {
                        node.Key = newKey;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 清空当前区块的所有内容。
        /// </summary>
        public void Clear()
        {
            _items.Clear();
            _nodes.Clear();
            _topComments = null;
            DanglingText = null;
        }

        #region Comment Operation

        /// <summary>
        /// 添加注释。
        /// </summary>
        /// <param name="contents">注释的内容</param>
        public void AddComments(params string[] contents)
        {
            if (contents != null && contents.Length > 0)
            {
                AddComments(contents.ToList());
            }
        }

        /// <summary>
        /// 添加注释。
        /// </summary>
        /// <param name="contents">注释的内容</param>
        public void AddComments(ICollection<string> contents)
        {
            var filtered = FilterNonNull(contents);
            if (filtered.Count == 0) return;
            int size = _nodes.Count;
            if (size == 0) AppendTopComments(filtered);
            else _nodes[size - 1].AppendComments(filtered);
        }

        /// <summary>
        /// 获取指定项（键值对）到上一项（键值对）之间的全部注释。
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>注释列表</returns>
        /// <exception cref="AccessValueException">当键（Key）不存在或为 <c>null</c></exception>
        public IReadOnlyList<string> GetCommentsBefore(string key)
        {
            int i = FindKeyIndex(key);
            if (i == 0) return _topComments.WrapList();
            return _nodes[i - 1].Comments.WrapList();
        }

        /// <summary>
        /// 在指定项（键值对）到上一项（键值对）之间添加注释，如果已有注释则合并到末尾。
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="contents">注释的内容</param>
        /// <exception cref="AccessValueException">当键（Key）不存在或为 <c>null</c></exception>
        public void AddCommentsBefore(string key, ICollection<string> contents)
        {
            int i = FindKeyIndex(key);
            if (i == 0) AppendTopComments(contents);
            else _nodes[i - 1].AppendComments(contents);
        }

        /// <summary>
        /// 移除指定项（键值对）到上一项（键值对）之间的全部注释。
        /// </summary>
        /// <param name="key">键名</param>
        /// <exception cref="AccessValueException">当键（Key）不存在或为 <c>null</c></exception>
        public void RemoveCommentsBefore(string key)
        {
            int i = FindKeyIndex(key);
            if (i == 0) _topComments = null;
            else _nodes[i - 1].RemoveComments();
        }

        /// <summary>
        /// 获取指定项（键值对）到下一项（键值对）之间的全部注释。
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>注释列表</returns>
        /// <exception cref="AccessValueException">当键（Key）不存在或为 <c>null</c></exception>
        public IReadOnlyList<string> GetCommentsAfter(string key)
        {
            Node node = FindKeyNode(key);
            return node.Comments.WrapList();
        }

        /// <summary>
        /// 在指定项（键值对）到下一项（键值对）之间添加注释，如果已有注释则合并到末尾。
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="contents">注释的内容</param>
        /// <exception cref="AccessValueException">当键（Key）不存在或为 <c>null</c></exception>
        public void AddCommentsAfter(string key, ICollection<string> contents)
        {
            Node node = FindKeyNode(key);
            node.AppendComments(contents);
        }

        /// <summary>
        /// 在指定项（键值对）到下一项（键值对）之间添加注释，如果已有注释则合并到末尾。
        /// </summary>
        /// <param name="key">键名</param>
        /// <exception cref="AccessValueException">当键（Key）不存在或为 <c>null</c></exception>
        public void RemoveCommentsAfter(string key)
        {
            Node node = FindKeyNode(key);
            node.RemoveComments();
        }

        /// <summary>
        /// 获取所有的注释。
        /// 注释按添加顺序排列。
        /// </summary>
        /// <returns>注释列表</returns>
        public IReadOnlyList<string> GetComments()
        {
            List<string> comments = _topComments;
            var top = comments != null ? comments.AsEnumerable() : Enumerable.Empty<string>();
            var inNode = _nodes.SelectMany(node => node.Comments ?? Enumerable.Empty<string>());
            return top.Concat(inNode).ToList();
        }

        /// <summary>
        /// 移除所有的注释。
        /// </summary>
        public void RemoveComments()
        {
            _topComments = null;
            foreach (Node node in _nodes)
            {
                node.RemoveComments();
            }
        }

        #endregion

        #region Collection Conversion

        /// <summary>
        /// 将区块中项（键值对）生成为一个新 <see cref="Dictionary{String, String}">Dictionary</see>。
        /// </summary>
        /// <returns>区块中的项</returns>
        public IDictionary<string, string> ToDict()
        {
            return new Dictionary<string, string>(_items);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public bool Equals(Section other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return _nodes.ContentEqual(other._nodes) && _topComments.ContentEqual(other._topComments) &&
                   DanglingText == other.DanglingText;
        }

        public override bool Equals(object obj)
        {
            return obj is Section section && Equals(section);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return ValueTuple.Create(DanglingText, _topComments, _items).GetHashCode();
        }

        #region Inner Access

        /// <summary>
        /// 创建一个当前 Section 对象的副本（深拷贝）。
        /// </summary>
        /// <returns>当前对象的副本</returns>
        internal Section DeepClone()
        {
            Section section = new Section();
            foreach (var item in _items)
            {
                section._items.Add(item.Key, item.Value);
            }

            section.AppendTopComments(_topComments);
            section.DanglingText = DanglingText;
            foreach (Node n in _nodes)
            {
                Node nn = new Node(n.Key);
                nn.AppendComments(n.Comments);
                section._nodes.Add(nn);
            }

            return section;
        }

        internal void ForEachKeysAndComments(Action<string, string, string> consumer)
        {
            // Action accepts: (String key, String value, String comment)
            if (_topComments != null && _topComments.Count > 0)
            {
                foreach (string comment in _topComments)
                {
                    consumer(null, null, comment);
                }
            }

            foreach (Node node in _nodes)
            {
                string key = node.Key;
                string value = _items[key];
                List<string> comments = node.Comments;
                bool emptyComments = comments.IsEmpty();
                consumer(key, value ?? string.Empty, null);
                if (!emptyComments)
                {
                    foreach (string comment in comments)
                    {
                        consumer(null, null, comment);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 查找键的索引。
        /// </summary>
        /// <param name="key">要查找的键</param>
        /// <returns>键的索引</returns>
        /// <exception cref="AccessValueException">如果键未找到</exception>
        private int FindKeyIndex(string key)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                Node node = _nodes[i];
                if (node.Key.Equals(key)) return i;
            }

            throw new AccessValueException($"Key \"{key}\" not found");
        }

        /// <summary>
        /// 返回指定键的节点。
        /// </summary>
        /// <param name="key">指定的键</param>
        /// <returns>节点</returns>
        /// <exception cref="AccessValueException">如果键未找到</exception>
        private Node FindKeyNode(string key)
        {
            return _nodes[FindKeyIndex(key)];
        }

        private void AppendTopComments(ICollection<string> comments)
        {
            if (comments.IsEmpty()) return;
            if (_topComments == null) _topComments = new List<string>(comments);
            else _topComments.AddRange(comments);
        }

        private static IList<string> FilterNonNull(ICollection<string> contents)
        {
            if (contents.IsEmpty()) return new List<string>();
            return contents.Where(c => c != null).ToList();
        }

        /// <summary>
        /// 区块中的数据节点，由键名 + 尾部注释组成
        /// </summary>
        private class Node : IEquatable<Node>
        {
            /// <summary>
            /// 节点的键名
            /// </summary>
            public string Key { get; set; }

            /// <summary>
            /// 节点的注释列表
            /// </summary>
            public List<string> Comments { get; private set; }

            /// <summary>
            /// 初始化一个新的 Node 实例
            /// </summary>
            /// <param name="key">节点的键名</param>
            public Node(string key)
            {
                Key = key;
            }

            /// <summary>
            /// 向节点追加注释
            /// </summary>
            /// <param name="comments">要追加的注释列表</param>
            public void AppendComments(ICollection<string> comments)
            {
                if (comments.IsEmpty()) return;
                if (Comments == null) Comments = new List<string>(comments);
                else Comments.AddRange(comments);
            }

            /// <summary>
            /// 移除节点的所有注释
            /// </summary>
            public void RemoveComments()
            {
                Comments?.Clear();
            }

            /// <summary>
            /// 获取节点的字符串表示形式
            /// </summary>
            /// <returns>节点的字符串表示</returns>
            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(Key).Append(" =").AppendLine();
                if (Comments != null && Comments.Any())
                    Comments.ForEach(s => builder.Append("# ").Append(s).AppendLine());
                if (builder.Length > 0) builder.Length--;
                return builder.ToString();
            }

            /// <summary>
            /// 判断两个节点是否相等
            /// </summary>
            /// <param name="obj">要比较的节点</param>
            /// <returns>是否相等</returns>
            public override bool Equals(object obj)
            {
                return obj is Node node && Equals(node);
            }

            /// <summary>
            /// 获取节点的哈希码
            /// </summary>
            /// <returns>节点的哈希码</returns>
            [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
            public override int GetHashCode()
            {
                return ValueTuple.Create(Key, Comments).GetHashCode();
            }

            public bool Equals(Node other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;
                return Key == other.Key && Comments.ContentEqual(other.Comments);
            }
        }
    }
}