using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VanillaSugar.Ini.Exception;

namespace VanillaSugar.Ini
{
    /// <summary>
    /// 表示一个 INI 文件内容。
    /// <para>一个 INI 由多个区块 <see cref="Section"/> 组成。</para>
    /// </summary>
    public class Ini : IEnumerable<KeyValuePair<string, Section>>, IEquatable<Ini>
    {
        private readonly IDictionary<string, Section> _sections = new Dictionary<string, Section>();

        /// <summary>
        /// 获取无标题区块（处于文件头部，没有指定区块名的键值对和数据会被存在这里)。
        /// </summary>
        /// <value>无标题区块</value>
        public Section UntitledSection { get; private set; } = new Section();

        /// <summary>
        /// 获取所有的区块名。
        /// 区块名按添加顺序排列。
        /// </summary>
        public IReadOnlyList<string> SectionNames => UnmodifiableList<string>.Wrap(_sections.Keys.ToList());

        /// <summary>
        /// 获取此 INI 中的某一个区块。
        /// 如果区块名不存在，则返回 <c>null</c>。
        /// </summary>
        /// <param name="name">区块名</param>
        /// <returns>区块。</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="name"/> 为 <c>null</c>。</exception>
        public Section Get(string name)
        {
            ObjectHelper.AssertNotNull(name, nameof(name));
            return _sections.TryGetValue(name, out var section) ? section : null;
        }

        /// <summary>
        /// 根据区块名获取区块。如果指定的区块不存在，则先创建再返回区块。
        /// </summary>
        /// <param name="name">区块名</param>
        /// <returns>区块。</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="name"/> 为 <c>null</c>。</exception>
        public Section GetOrAdd(string name)
        {
            if (_sections.TryGetValue(name, out var section)) return section;
            section = new Section();
            _sections[name] = section;
            return section;
        }

        /// <summary>
        /// 获取区块的总数（不含 <see cref="UntitledSection"/>）。
        /// </summary>
        /// <value>区块数量</value>
        public int Count => _sections.Count;

        /// <summary>
        /// 构造一个 INI 对象。
        /// </summary>
        public Ini()
        {
        }

        /// <summary>
        /// 检测此 INI 中是否包含某区块。
        /// </summary>
        /// <param name="name">区块名</param>
        /// <returns>包含时返回 <c>true</c>, 不包含返回 <c>false</c>。</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="name"/> 为 <c>null</c>。</exception>
        public bool Contains(string name)
        {
            return _sections.ContainsKey(name);
        }

        /// <summary>
        /// 移除某一个区块。
        /// </summary>
        /// <param name="name">区块名</param>
        /// <returns>成功移除返回 <c>true</c>, 否则返回 <c>false</c>。</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="name"/> 为 <c>null</c>。</exception>
        public bool Remove(string name)
        {
            return _sections.Remove(name);
        }

        /// <summary>
        /// 将指定区块的名字更改为新区块名，如果区块名不存在则什么也不做。
        /// </summary>
        /// <param name="name">区块名</param>
        /// <param name="newName">新的区块名</param>
        /// <returns>成功修改返回 <c>true</c>, 否则返回 <c>false</c>。</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="name"/> 或 <paramref name="newName"/> 为 <c>null</c>。</exception>
        public bool Rename(string name, string newName)
        {
            ObjectHelper.AssertNotNull(name, nameof(name));
            ObjectHelper.AssertNotNull(newName, nameof(newName));
            if (name == newName) return false;
            if (_sections.ContainsKey(name))
            {
                var section = _sections[name];
                _sections.Remove(name);
                _sections[newName] = section;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 创建一个当前 INI 对象的副本（深拷贝）。
        /// </summary>
        /// <returns>当前对象的副本。</returns>
        public Ini DeepClone()
        {
            var ini = new Ini();
            ini.UntitledSection = UntitledSection.DeepClone();
            foreach (var kv in _sections)
            {
                ini._sections[kv.Key] = kv.Value.DeepClone();
            }

            return ini;
        }

        /// <summary>
        /// 清空当前 INI 对象的所有内容。
        /// </summary>
        /// <param name="includingUntitled">是否包含 <see cref="UntitledSection"/>。传入 true 会清理，false 反之</param>
        public void Clear(bool includingUntitled = true)
        {
            if (includingUntitled)
            {
                UntitledSection.Clear();
            }

            foreach (var section in _sections.Values)
            {
                section.Clear();
            }

            _sections.Clear();
        }

        /// <summary>
        /// 以链式访问操作此 INI 对象
        /// </summary>
        /// <returns>链式访问器。</returns>
        public ChainIniAccessor ChainAccess()
        {
            return new ChainIniAccessor(this);
        }

        /// <summary>
        /// 获取此 INI 中指定项（键值对）中的值。
        /// 如果区块和键名都存在则返回对应的值，否则返回 <paramref name="def"/> 替代。
        /// </summary>
        /// <param name="name">区块名</param>
        /// <param name="key">键名</param>
        /// <param name="def">当区块名或键名不存在时的替代返回值</param>
        /// <returns>值或替代值。</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="name"/> 或 <paramref name="key"/> 含有 <c>null</c>。</exception>
        public string GetItemValue(string name, string key, string def = null)
        {
            ObjectHelper.AssertNotNull(key, nameof(key));
            Section section = Get(name);
            return section != null ? section.Get(key, def) : def;
        }

        /// <summary>
        /// 向 INI 中添加新项（键值对），如果区块中已有相同键名（Key）的项则会覆盖。
        /// 如果指定的区块不存在，则先创建再区块中写入。
        /// </summary>
        /// <param name="name">区块名</param>
        /// <param name="key">键名</param>
        /// <param name="value">值</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="name"/>，<paramref name="key"/> 或 <paramref name="value"/> 含有 <c>null</c>。</exception>
        public void SetItemValue(string name, string key, string value)
        {
            GetOrAdd(name).Set(key, value);
        }

        /// <summary>
        /// 获取此 INI 中指定项（键值对）中的值，并转为 int 返回。
        /// </summary>
        /// <param name="name">区块名</param>
        /// <param name="key">键名</param>
        /// <returns>值。</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="name"/> 或 <paramref name="key"/> 为 <c>null</c>。</exception>
        /// <exception cref="AccessValueException">当区块名/键（Key）不存在，或值无法转换为 <c>int</c>。</exception>
        public int GetItemValueAsInt(string name, string key)
        {
            Section section = Get(name);
            if (section == null) throw new AccessValueException("Section " + name + " not found");
            return section.GetAsInt(key);
        }

        /// <summary>
        /// 获取此 INI 中指定项（键值对）中的值，并转为 boolean 返回。
        /// </summary>
        /// <param name="name">区块名</param>
        /// <param name="key">键名</param>
        /// <returns>值。</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="name"/> 或 <paramref name="key"/> 为 <c>null</c>。</exception>
        /// <exception cref="AccessValueException">当区块名/键（Key）不存在，或值无法转换为 <c>bool</c>。</exception>
        public bool GetItemValueAsBool(string name, string key)
        {
            Section section = Get(name);
            if (section == null) throw new AccessValueException("Section " + name + " not found");
            return section.GetAsBool(key);
        }

        /// <summary>
        /// 检测此 INI 中是否包含某区块，且区块中包含某项（键值对）。
        /// </summary>
        /// <param name="name">区块名</param>
        /// <param name="key">键名</param>
        /// <returns>存在则返回 <c>true</c>，否则返回 <c>false</c>。</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="name"/> 或 <paramref name="key"/> 为 <c>null</c>。</exception>
        public bool ContainsItemValue(string name, string key)
        {
            ObjectHelper.AssertNotNull(key, nameof(key));
            Section section = Get(name);
            return section != null && section.Contains(key);
        }

        /// <summary>
        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        /// </summary>
        /// <returns>可返回区块名称、区块的 <see cref="IEnumerator{T}"/>。</returns>
        public IEnumerator<KeyValuePair<string, Section>> GetEnumerator()
        {
            return _sections.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 比较当前对象和 <paramref name="other"/> 内容是否相同。
        /// <para>只有两个对象的 <see cref="UntitledSection"/> 相同、包含相同名称的区块且同名 <see cref="Section"/> 的内容也相同，才视为内容相同。
        /// 传入 <c>null</c> 则总是返回 <c>false</c>。
        /// </para>
        /// </summary>
        /// <param name="other">要比较的对象</param>
        /// <returns>如果内容相同则返回 <c>true</c>，否则返回 <c>false</c>。</returns>
        public bool Equals(Ini other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return _sections.DictEqual(other._sections) && Equals(UntitledSection, other.UntitledSection);
        }

        /// <summary>
        /// 比较当前对象和 <paramref name="obj"/> 内容是否相同。
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>如果要比较的对象是 <see cref="Ini"/> 且内容相同则返回 <c>true</c>，否则返回 <c>false</c>。</returns>
        /// <seealso cref="Equals(VanillaSugar.Ini.Ini)"/>
        public override bool Equals(object obj)
        {
            return obj is Ini ini && Equals(ini);
        }

        /// <summary>
        /// 计算当前对象的 hash 码。
        /// </summary>
        /// <returns>hash 码。</returns>
        public override int GetHashCode()
        {
            return _sections.GetHashCode() * 23 + UntitledSection.GetHashCode();
        }
    }
}