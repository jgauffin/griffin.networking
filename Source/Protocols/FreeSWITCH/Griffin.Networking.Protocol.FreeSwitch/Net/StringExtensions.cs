namespace Griffin.Networking.Protocol.FreeSwitch.Net
{
    public static class StringExtensions
    {
        /// <summary>
        /// This method converts a enum value to the correct format
        /// that FreeSwitch uses (WORD1_WORD2) instead of the CamelCase that
        /// we use in .net.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string UnderscoreToCamelCase(this string name)
        {
            var isFirst = true;
            var result = string.Empty;
            foreach (var ch in name)
            {
                if (isFirst)
                {
                    result += char.ToUpper(ch);
                    isFirst = false;
                }
                else
                {
                    if (ch == '_')
                        isFirst = true;
                    else
                        result += char.ToLower(ch);
                }
            }
            return result;
        }

        public static string CamelCaseToUpperCase(this string name)
        {
            var result = string.Empty;
            var isFirst = true;
            foreach (var ch in name)
            {
                if (char.IsUpper(ch))
                {
                    if (!isFirst)
                        result += '_';
                    else
                        isFirst = false;
                    result += char.ToUpper(ch);
                }
                else
                    result += char.ToUpper(ch);
            }

            return result;
        }
    }
}