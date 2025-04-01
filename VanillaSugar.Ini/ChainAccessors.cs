namespace VanillaSugar.Ini
{
    /// <summary>
    /// <see cref="Ini"/> 的链式访问器
    /// </summary>
    public class ChainIniAccessor
    {
        private readonly Ini _ini;

        /// <summary>
        /// 初始化 Ini 链式访问器
        /// </summary>
        /// <param name="ini">Ini 实例</param>
        public ChainIniAccessor(Ini ini)
        {
            _ini = ini;
        }

        /// <summary>
        /// 将 Ini 中指定区块的名字更改为新区块名，如果区块名不存在则什么也不做。
        /// </summary>
        /// <param name="sectionName">区块名</param>
        /// <param name="newSectionName">新的区块名</param>
        /// <returns>Ini 链式访问器。</returns>
        public ChainIniAccessor RenameSection(string sectionName, string newSectionName)
        {
            _ini.Rename(sectionName, newSectionName);
            return this;
        }

        /// <summary>
        /// 移除 Ini 中指定的区块。
        /// </summary>
        /// <param name="sectionName">区块名</param>
        /// <returns>Ini 链式访问器。</returns>
        public ChainIniAccessor RemoveSection(string sectionName)
        {
            _ini.Remove(sectionName);
            return this;
        }

        /// <summary>
        /// 打开并操作 INI 中的指定区块，如果区块不存在则会自动添加。
        /// </summary>
        /// <param name="sectionName">区块名</param>
        /// <returns>区块的链式访问器。</returns>
        public ChainSectionAccessor OpenSection(string sectionName)
        {
            return new ChainSectionAccessor(this, _ini.GetOrAdd(sectionName));
        }

        /// <summary>
        /// 打开并操作 INI 中的无标题区块。
        /// </summary>
        /// <returns>区块的链式访问器。</returns>
        public ChainSectionAccessor OpenUntitledSection()
        {
            return new ChainSectionAccessor(this, _ini.UntitledSection);
        }
    }

    /// <summary>
    /// <see cref="Section"/> 的链式访问器
    /// </summary>
    public class ChainSectionAccessor
    {
        private readonly ChainIniAccessor _accessor;
        private readonly Section _section;

        internal ChainSectionAccessor(ChainIniAccessor accessor, Section section)
        {
            _accessor = accessor;
            _section = section;
        }

        /// <summary>
        /// 向区块中添加新项，如果区块中已有相同键名（Key）的项则会覆盖。
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="value">值</param>
        /// <returns>区块的链式访问器。</returns>
        public ChainSectionAccessor Set(string key, string value)
        {
            _section.Set(key, value);
            return this;
        }

        /// <summary>
        /// 将区块中指定项的键名（Key）更改为新键名，如果键名（Key）不存在则什么也不做。
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="newKey">新的键名</param>
        /// <returns>区块的链式访问器。</returns>
        public ChainSectionAccessor Rename(string key, string newKey)
        {
            _section.Rename(key, newKey);
            return this;
        }

        /// <summary>
        /// 移除区块中指定的项。
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>区块的链式访问器。</returns>
        public ChainSectionAccessor Remove(string key)
        {
            _section.Remove(key);
            return this;
        }

        /// <summary>
        /// 在区块末尾添加一条注释。
        /// </summary>
        /// <param name="comment">注释</param>
        /// <returns>区块的链式访问器。</returns>
        public ChainSectionAccessor AddComment(string comment)
        {
            _section.AddComments(comment);
            return this;
        }

        /// <summary>
        /// 结束操作当前区块，并返回到上级 Ini。
        /// </summary>
        /// <returns>Ini 的链式访问器。</returns>
        public ChainIniAccessor CloseSection()
        {
            return _accessor;
        }
    }
}