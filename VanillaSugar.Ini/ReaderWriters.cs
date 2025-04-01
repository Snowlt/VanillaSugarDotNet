using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VanillaSugar.Ini.Exception;

namespace VanillaSugar.Ini
{
    /// <summary>
    /// INI 文件读写方法入口类
    /// 提供简化的静态方法，用于从文件读取出 INI，或导出 INI 到文件中。
    /// </summary>
    public class IniReaderWriter
    {
        private static readonly IniSerializer FileWriter = new IniSerializer();

        private static readonly IniSerializer PrettyFileWriter = new IniSerializer
        {
            AddSpaceAroundEqualizer = true,
            AddSpaceBeforeComment = true
        };

        private static readonly IniDeserializer FileReader = new IniDeserializer();

        /// <summary>
        /// 从文件读取解析并返回为 INI 对象。
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>读取出的 INI。</returns>
        /// <exception cref="ReadWriteException">当 IO 异常时抛出。</exception>
        public static Ini LoadFromFile(string path)
        {
            return LoadFromFile(path, Encoding.UTF8);
        }

        /// <summary>
        /// 从文件读取解析并返回为 INI 对象。
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">文件的字符集</param>
        /// <returns>读取出的 INI。</returns>
        /// <exception cref="ReadWriteException">当 IO 异常时抛出。</exception>
        public static Ini LoadFromFile(string path, Encoding encoding)
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    return FileReader.Read(new StreamReader(stream, encoding));
                }
            }
            catch (IOException e)
            {
                throw new ReadWriteException("Failed to load file", e);
            }
        }

        /// <summary>
        /// 将 INI 的内容保存到文件中。
        /// </summary>
        /// <param name="ini">要保存的 INI 对象</param>
        /// <param name="path">文件路径</param>
        /// <exception cref="ReadWriteException">当 IO 异常时抛出。</exception>
        public static void SaveToFile(Ini ini, string path)
        {
            SaveToFile(ini, path, Encoding.UTF8);
        }

        /// <summary>
        /// 将 INI 的内容保存到文件中。
        /// </summary>
        /// <param name="ini">要保存的 INI 对象</param>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">文件的字符集</param>
        /// <exception cref="ReadWriteException">当 IO 异常时抛出。</exception>
        public static void SaveToFile(Ini ini, string path, Encoding encoding)
        {
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    FileWriter.Write(ini, new StreamWriter(stream, encoding));
                }
            }
            catch (IOException e)
            {
                throw new ReadWriteException("Failed to save file", e);
            }
        }

        /// <summary>
        /// 将 INI 的内容保存到文件中。
        /// 自动在注释内容前、键值对等号两侧添加空格。
        /// </summary>
        /// <param name="ini">要保存的 INI 对象</param>
        /// <param name="path">文件路径</param>
        /// <exception cref="ReadWriteException">当 IO 异常时抛出。</exception>
        public static void SaveToPrettyFile(Ini ini, string path)
        {
            SaveToPrettyFile(ini, path, Encoding.UTF8);
        }

        /// <summary>
        /// 将 INI 的内容保存到文件中。
        /// 自动在注释内容前、键值对等号两侧添加空格。
        /// </summary>
        /// <param name="ini">要保存的 INI 对象</param>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">文件的字符集</param>
        /// <exception cref="ReadWriteException">当 IO 异常时抛出。</exception>
        public static void SaveToPrettyFile(Ini ini, string path, Encoding encoding)
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    PrettyFileWriter.Write(ini, new StreamWriter(stream, encoding));
                }
            }
            catch (IOException e)
            {
                throw new ReadWriteException("Failed to save file", e);
            }
        }

        private IniReaderWriter()
        {
        }
    }

    /// <summary>
    /// 导出 INI 为文本到文件或流中
    /// </summary>
    public class IniSerializer
    {
        private string _commentPrefix = ";";
        private string _lineSeparator = Environment.NewLine;

        /// <summary>
        /// 注释的前缀符号（默认 <c>";"</c>）
        /// </summary>
        /// <value>注释的前缀符号</value>
        public string CommentPrefix
        {
            get => _commentPrefix;
            set => _commentPrefix = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 获取输出时使用的换行符（默认 <see cref="Environment.NewLine"/>）
        /// </summary>
        /// <value>换行符</value>
        public string LineSeparator
        {
            get => _lineSeparator;
            set => _lineSeparator = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 是否在注释前（<see cref="CommentPrefix"/> 后）插入空格（默认 <c>false</c>）
        /// </summary>
        /// <value><c>true</c> 插入，<c>false</c> 不插入</value>
        public bool AddSpaceBeforeComment { get; set; } = false;


        /// <summary>
        /// 是否在项（键值对）的等号两侧插入空格（默认 <c>false</c>）
        /// </summary>
        /// <value><c>true</c> 插入，<c>false</c> 不插入</value>
        public bool AddSpaceAroundEqualizer { get; set; } = false;


        /// <summary>
        /// 将 INI 内容写出到 <see cref="TextWriter"/> 中。
        /// </summary>
        /// <param name="ini">要导出的 INI 对象</param>
        /// <param name="writer">输出流</param>
        /// <exception cref="ArgumentNullException">如果 <paramref name="ini"/> / <paramref name="writer"/> 中含有 <c>null</c>。</exception>
        /// <exception cref="ReadWriteException">如果读取时发生 IO 异常。</exception>
        public void Write(Ini ini, TextWriter writer)
        {
            ObjectHelper.AssertNotNull(ini, nameof(ini));
            ObjectHelper.AssertNotNull(writer, nameof(writer));
            try
            {
                Section untitledSection = ini.UntitledSection;
                if (untitledSection != null)
                {
                    WriteSection(untitledSection, writer);
                }

                foreach (var entry in ini)
                {
                    writer.Write('[');
                    writer.Write(entry.Key);
                    writer.Write(']');
                    writer.Write(_lineSeparator);
                    WriteSection(entry.Value, writer);
                }

                writer.Flush();
            }
            catch (IOException e)
            {
                throw new ReadWriteException("Error occurred when serializing content", e);
            }
        }

        /// <summary>
        /// 将 INI 内容写出到 <see cref="Stream"/> 中。
        /// </summary>
        /// <param name="ini">要导出的 INI 对象</param>
        /// <param name="stream">输出流</param>
        /// <param name="encoding">字符编码 <see cref="Encoding"/></param>
        /// <exception cref="ArgumentNullException">如果 <paramref name="ini"/> / <paramref name="stream"/> / <paramref name="encoding"/> 中含有 <c>null</c>。</exception>
        /// <exception cref="ReadWriteException">如果读取时发生 IO 异常。</exception>
        public void Write(Ini ini, Stream stream, Encoding encoding)
        {
            ObjectHelper.AssertNotNull(ini, nameof(ini));
            ObjectHelper.AssertNotNull(stream, nameof(stream));
            ObjectHelper.AssertNotNull(encoding, nameof(encoding));
            using (var writer = new StreamWriter(stream, encoding))
            {
                Write(ini, writer);
            }
        }

        private void WriteSection(Section section, TextWriter writer)
        {
            if (section.DanglingText != null)
            {
                writer.Write(section.DanglingText);
                writer.Write(_lineSeparator);
            }

            StringBuilder prefixBuilder = new StringBuilder(CommentPrefix);
            if (AddSpaceBeforeComment) prefixBuilder.Append(' ');
            string combinedPrefix = prefixBuilder.ToString();
            string equalizer = AddSpaceAroundEqualizer ? " = " : "=";
            section.ForEachKeysAndComments((key, value, comment) =>
            {
                if (key != null)
                {
                    writer.Write(key);
                    writer.Write(equalizer);
                    writer.Write(value);
                }
                else
                {
                    writer.Write(combinedPrefix);
                    writer.Write(comment);
                }

                writer.Write(_lineSeparator);
            });
        }
    }


    /// <summary>
    /// 解析和读取 INI 文件数据
    /// </summary>
    public class IniDeserializer
    {
        /// <summary>
        /// 对于区块顶部文本（非注释文本）的处理方式
        /// </summary>
        public enum DanglingTextOptions
        {
            /// <summary>
            /// 保留原文
            /// </summary>
            Keep,

            /// <summary>
            /// 当作注释处理
            /// </summary>
            ToComment,

            /// <summary>
            /// 丢弃此行
            /// </summary>
            Drop
        }

        private IReadOnlyCollection<string> _commentPrefixes = new List<string>(2) {";", "#"};

        /// <summary>
        /// 获取解析 INI 时如何处理区块顶部非注释文本（默认为 <see cref="DanglingTextOptions.Keep"/>）
        /// </summary>
        /// <value>处理方式</value>
        public DanglingTextOptions DanglingTextOption { get; set; } = DanglingTextOptions.Keep;

        /// <summary>
        /// 获取要解析的文件中注释的前缀（默认为 <c>";", "#"</c>）
        /// </summary>
        /// <value>注释的前缀</value>
        public IReadOnlyCollection<string> CommentPrefixes
        {
            get => new List<string>(_commentPrefixes);
            set
            {
                if (value == null || value.Count == 0)
                    throw new ArgumentException("Comment prefixes cannot be null or empty.");
                _commentPrefixes = new HashSet<string>(value);
            }
        }

        /// <summary>
        /// 获取读取区块标题（区块名）的时候是否去除首尾空白（默认 <c>true</c>）
        /// </summary>
        /// <value>值</value>
        public bool TrimSectionName { get; set; } = true;

        /// <summary>
        /// 获取读取区块中键名的时候是否去除首尾空白（默认 <c>true</c>）
        /// </summary>
        /// <value>值</value>
        public bool TrimKey { get; set; } = true;

        /// <summary>
        /// 获取读取区块中值的时候是否去除首尾空白（默认 <c>true</c>）
        /// </summary>
        /// <value>值</value>
        public bool TrimValue { get; set; } = true;

        /// <summary>
        /// 获取读取注释的时候是否去除首尾空白（默认 <c>true</c>）
        /// </summary>
        /// <value>值</value>
        public bool TrimComment { get; set; } = true;

        /// <summary>
        /// 从 <see cref="TextReader"/> 中读取 INI 内容并解析。
        /// </summary>
        /// <param name="reader">输入流</param>
        /// <returns>读取的 INI 对象。</returns>
        /// <exception cref="ArgumentNullException">如果 <paramref name="reader"/> 为 <c>null</c>。</exception>
        /// <exception cref="ReadWriteException">如果读取时发生 IO 异常。</exception>
        public Ini Read(TextReader reader)
        {
            ObjectHelper.AssertNotNull(reader, nameof(reader));
            Ini ini = new Ini();
            try
            {
                LoadToIni(reader, ini);
            }
            catch (IOException e)
            {
                throw new ReadWriteException("Error occurred when deserializing content", e);
            }

            return ini;
        }

        /// <summary>
        /// 从 <see cref="Stream"/> 中读取 INI 内容并解析。
        /// </summary>
        /// <param name="stream">输入流</param>
        /// <param name="charset">字符编码 <see cref="Encoding"/></param>
        /// <returns>读取的 INI 对象。</returns>
        /// <exception cref="ArgumentNullException">如果 <paramref name="stream"/> / <paramref name="charset"/> 中含有 <c>null</c>。</exception>
        /// <exception cref="ReadWriteException">如果读取时发生 IO 异常。</exception>
        public Ini Read(Stream stream, Encoding charset)
        {
            ObjectHelper.AssertNotNull(stream, nameof(stream));
            ObjectHelper.AssertNotNull(charset, nameof(charset));
            try
            {
                using (var reader = new StreamReader(stream, charset))
                {
                    return Read(reader);
                }
            }
            catch (ArgumentException e)
            {
                throw new ReadWriteException("Unable to read from stream", e);
            }
        }

        private void LoadToIni(TextReader reader, Ini ini)
        {
            var prefixes = _commentPrefixes;
            Section sec = ini.UntitledSection;
            // Uses a head node to store dangling text (if exist), and makes adding node easier
            Content head = new Content();
            Content tail = head;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string parsed;
                if ((parsed = ParseComment(line, prefixes)) != null)
                {
                    tail.Next = Content.OfComment(parsed);
                    tail = tail.Next;
                    continue;
                }

                if ((parsed = ParseSectionName(line)) != null)
                {
                    FlushIntoSection(sec, head);
                    tail = head;
                    sec = ini.GetOrAdd(parsed);
                    continue;
                }

                int eqIdx = line.IndexOf('=');
                if (eqIdx != -1)
                {
                    string key = line.Substring(0, eqIdx);
                    string value = line.Substring(eqIdx + 1);
                    tail.Next = Content.OfKey(key, value);
                    tail = tail.Next;
                    continue;
                }

                tail.Values.Add(line);
            }

            FlushIntoSection(sec, head);
        }

        private void FlushIntoSection(Section sec, Content head)
        {
            if (head.Values.Count > 0)
            {
                if (DanglingTextOption == DanglingTextOptions.Keep)
                {
                    sec.DanglingText = head.JoinValues();
                }
                else if (DanglingTextOption == DanglingTextOptions.ToComment)
                {
                    sec.AddComments(new List<string> {head.JoinValues()});
                }
            }

            for (Content p = head.Next; p != null;)
            {
                if (p.Key != null)
                {
                    string key = TrimKey ? p.Key.Trim() : p.Key;
                    string value = TrimValue ? p.JoinValues().Trim() : p.JoinValues();
                    sec.Set(key, value);
                }
                else
                {
                    string comment = TrimComment ? p.JoinValues().Trim() : p.JoinValues();
                    sec.AddComments(new List<string> {comment});
                }

                // Manually clear node can help GC, which like LinkedList
                Content next = p.Next;
                p.Next = null;
                p.Values = null;
                p.Key = null;
                p = next;
            }

            // Resets head node
            head.Values.Clear();
            head.Next = null;
        }

        private string ParseSectionName(string s)
        {
            int beg = s.IndexOf('[');
            if (beg == -1) return null;
            int end = s.LastIndexOf(']');
            if (end == -1 || end < beg) return null;
            string name = s.Substring(beg + 1, end - beg - 1);
            return TrimSectionName ? name.Trim() : name;
        }

        private string ParseComment(string s, IEnumerable<string> prefixes)
        {
            foreach (string prefix in prefixes)
            {
                int i = s.IndexOf(prefix, StringComparison.Ordinal);
                if (i == 0 || (i != -1 && IsBlank(s, i)))
                    return s.Substring(i + prefix.Length);
            }

            return null;
        }

        private static bool IsBlank(string s, int end)
        {
            for (int i = 0; i < end; i++)
                if (!char.IsWhiteSpace(s[i]))
                    return false;
            return true;
        }

        private class Content
        {
            public string Key;
            public List<string> Values = new List<string>();
            public Content Next;

            public string JoinValues() => string.Join("\n", Values);

            public static Content OfKey(string key, string value)
            {
                Content content = new Content();
                content.Key = key;
                content.Values.Add(value);
                return content;
            }

            public static Content OfComment(string comment)
            {
                Content content = new Content();
                content.Values.Add(comment);
                return content;
            }
        }
    }
}