namespace VanillaSugar.Core
{
    /// <summary>
    /// 提供检查多个对象的静态方法。
    /// </summary>
    public static class CheckHelper
    {
        /// <summary>
        /// 判断所有对象是否都为 null
        /// </summary>
        /// <param name="targets">判断的对象</param>
        /// <returns>所有对象都为 null 时返回 true</returns>
        public static bool AllNull(params object[] targets)
        {
            if (targets == null || targets.Length == 0) return false;
            foreach (object condition in targets)
            {
                if (condition != null) return false;
            }

            return true;
        }

        /// <summary>
        /// 判断是否有任意一个对象为 null
        /// </summary>
        /// <param name="targets">判断的对象</param>
        /// <returns>有任意对象为 null 时返回 true</returns>
        public static bool AnyNull(params object[] targets)
        {
            if (targets == null || targets.Length == 0) return false;
            foreach (object condition in targets)
            {
                if (condition == null) return true;
            }

            return false;
        }

        /// <summary>
        /// 判断是否有任意一个值不等效于 null
        /// </summary>
        /// <param name="targets">判断的对象</param>
        /// <returns>有任意对象不为 null 时返回 true</returns>
        public static bool AnyNotNull(params object[] targets)
        {
            if (targets == null || targets.Length == 0) return false;
            foreach (object condition in targets)
            {
                if (condition != null) return true;
            }

            return false;
        }

        /// <summary>
        /// 判断所有对象是否都不为 null
        /// </summary>
        /// <param name="targets">判断的对象</param>
        /// <returns>所有对象都不为 null 时返回 true</returns>
        public static bool NoneNull(params object[] targets)
        {
            return !AnyNull(targets);
        }

        /// <summary>
        /// 判断所有字符串是否都满足：为 null 或长度为空或只包含空白字符
        /// </summary>
        /// <param name="targets">判断的对象</param>
        /// <returns>所有字符串都满足条件时返回 true</returns>
        public static bool AllBlank(params string[] targets)
        {
            if (targets == null || targets.Length == 0) return false;
            foreach (string condition in targets)
            {
                if (!string.IsNullOrWhiteSpace(condition)) return false;
            }

            return true;
        }

        /// <summary>
        /// 判断是否有任意一个字符串满足：为 null 或长度为空或只包含空白字符
        /// </summary>
        /// <param name="targets">判断的对象</param>
        /// <returns>有任意一个字符串满足条件时返回 true</returns>
        public static bool AnyBlank(params string[] targets)
        {
            if (targets == null || targets.Length == 0) return false;
            foreach (string condition in targets)
            {
                if (string.IsNullOrWhiteSpace(condition)) return true;
            }

            return false;
        }

        /// <summary>
        /// 判断是否有任意一个字符串满足：不为 null 且长度不为空且包含非空白字符
        /// </summary>
        /// <param name="targets">判断的对象</param>
        /// <returns>有任意一个字符串满足条件时返回 true</returns>
        public static bool AnyNotBlank(params string[] targets)
        {
            if (targets == null || targets.Length == 0) return false;
            foreach (string condition in targets)
            {
                if (!string.IsNullOrWhiteSpace(condition)) return true;
            }

            return false;
        }

        /// <summary>
        /// 判断所有字符串是否都满足：不为 null 且长度不为空且包含非空白字符
        /// </summary>
        /// <param name="targets">判断的对象</param>
        /// <returns>所有字符串都满足条件时返回 true</returns>
        public static bool NoneBlank(params string[] targets)
        {
            return !AnyBlank(targets);
        }
    }
}