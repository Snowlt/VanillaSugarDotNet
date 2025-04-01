namespace VanillaSugar.Ini
{
    namespace Exception
    {
        /// <summary>
        /// 抛出时表示遇到无法读取 INI 中特定值的情况。
        /// </summary>
        public class AccessValueException : System.Exception
        {
            /// <summary>
            /// 初始化一个新的异常。
            /// </summary>
            /// <param name="message">消息</param>
            public AccessValueException(string message) : base(message)
            {
            }
        }

        /// <summary>
        /// 抛出时表示通过 I/O 读写 INI 时出现意外的情况。
        /// </summary>
        public class ReadWriteException : System.Exception
        {
            /// <summary>
            /// 初始化一个新的异常。
            /// </summary>
            /// <param name="message">解释异常原因的消息</param>
            public ReadWriteException(string message) : base(message)
            {
            }

            /// <summary>
            /// 初始化一个新的异常。
            /// </summary>
            /// <param name="message">解释异常原因的消息</param>
            /// <param name="cause">导致此异常发生的内部异常</param>
            public ReadWriteException(string message, System.Exception cause) : base(message, cause)
            {
            }
        }
    }
}