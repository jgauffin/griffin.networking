using System;

namespace Griffin.Networking.Protocol.FreeSwitch
{
    public static class Enumm
    {
        public static T Parse<T>(string name) where T : struct
        {
            try
            {
                return (T) Enum.Parse(typeof (T), name, true);
            }
            catch (Exception err)
            {
#if DEBUG
                throw;
#else
                return 0;
#endif
            }
        }
    }
}