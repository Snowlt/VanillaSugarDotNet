namespace VanillaSugar.Ini
{
    namespace Exception
    {
        /// <summary>
        /// 抛出时表示遇到无法读取 INI 中特定值的情况
        /// </summary>
        public class AccessValueException : System.Exception
        {
            public AccessValueException(string message) : base(message)
            {
            }
        }

        /// <summary>
        /// 抛出时表示通过 I/O 读写 INI 时出现意外的情况
        /// </summary>
        public class ReadWriteException : System.Exception
        {
            public ReadWriteException(string message) : base(message)
            {
            }

            public ReadWriteException(string message, System.Exception cause) : base(message, cause)
            {
            }
        }
    }
}