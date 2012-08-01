using System;
using System.Collections.Generic;
using System.Linq;
using Griffin.Networking.Protocol.FreeSwitch.Events;

namespace Griffin.Networking.Protocol.FreeSwitch
{
    public static class TypeExtensions
    {
        public static IEnumerable<string> GetEventNames(this Type type)
        {
            return
                type.GetCustomAttributes(typeof (EventNameAttribute), true).Cast<EventNameAttribute>().Select(
                    attribute => attribute.Name);
        }
    }
}