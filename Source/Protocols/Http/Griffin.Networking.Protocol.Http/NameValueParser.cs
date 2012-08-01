using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Networking.Http
{
    public class NameValueParser
    {
        public void Parse(string value, IParameterCollection target)
        {
            int index = 0;
            var lastCh = char.MinValue;

            var name = "";
            int oldPos = 0;
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
