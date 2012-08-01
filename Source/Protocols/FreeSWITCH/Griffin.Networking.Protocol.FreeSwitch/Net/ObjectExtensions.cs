using System;

namespace Griffin.Networking.Protocol.FreeSwitch.Net
{
    public static class ObjectExtensions
    {
        public static string ToStringOrClassName(this object instance)
        {
            var str = instance.ToString();
            if (str == instance.GetType().FullName)
                return instance.GetType().Name;

            return str;
        }

        public static bool IsTypeAnd<T>(this object instance, Func<T, bool> check) where T : class
        {
            var t = instance as T;
            return check(t);
        }
    }
}