using System;

namespace Griffin.Networking.Protocol.FreeSwitch
{
    public static class DateTimeExtensions
    {
        public static readonly DateTime UnixEpoch = new DateTime(1970, 01, 01);

        public static DateTime FromUnixTime(this string value)
        {
            int time;
            return int.TryParse(value, out time) ? UnixEpoch.AddSeconds(time) : DateTime.MinValue;
        }
    }
}