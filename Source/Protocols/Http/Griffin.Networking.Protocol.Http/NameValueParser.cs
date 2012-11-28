using System;

namespace Griffin.Networking.Protocol.Http
{
    /// <summary>
    /// Parses a namevalue collection
    /// </summary>
    public class NameValueParser
    {
        /// <summary>
        /// Parse string
        /// </summary>
        /// <param name="value">contains "a=b,c=d" etc</param>
        /// <param name="target">Collection to fill with the values</param>
        public void Parse(string value, IParameterCollection target)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (target == null) throw new ArgumentNullException("target");

            var index = 0;
            var lastCh = char.MinValue;

            var name = "";
            var oldPos = 0;
            while (index < value.Length)
            {
                var ch = value[index];
                switch (ch)
                {
                    case '=':
                        if (lastCh != '\\')
                        {
                            name = value.Substring(oldPos, index - oldPos).Trim(' ');
                            oldPos = index + 1;
                        }
                        break;
                    case ',':
                        if (lastCh != '\\')
                        {
                            target.Add(name, value.Substring(oldPos, index - oldPos).Trim(' ', '"'));
                            name = "";
                            oldPos = index + 1;
                        }
                        break;
                }
                lastCh = value[index];
                ++index;
            }

            if (name != "")
            {
                target.Add(name, value.Substring(oldPos).Trim(' ', '"'));
            }
        }
    }
}