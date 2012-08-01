using System;

namespace Griffin.Networking.Protocol.FreeSwitch.Events.Sip
{
    [EventName("SOFIA_EXPIRE", IsCustom = true)]
    public class SofiaExpire : Presence
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);

        public string ProfileName { get; set; }

        public string UserName { get; set; }

        public string Domain { get; set; }

        public string UserAgent { get; set; }

        public DateTime Expires { get; set; }

        /*
				switch_event_add_header(s_event, SWITCH_STACK_BOTTOM, "profile-name", "%s", argv[6]);
			switch_event_add_header(s_event, SWITCH_STACK_BOTTOM, "user", "%s", argv[1]);
			switch_event_add_header(s_event, SWITCH_STACK_BOTTOM, "host", "%s", argv[2]);
			switch_event_add_header(s_event, SWITCH_STACK_BOTTOM, "user-agent", "%s", argv[5]);	
         * */

        public override bool ParseParameter(string name, string value)
        {
            switch (name)
            {
                case "profile-name":
                    ProfileName = value;
                    break;
                case "user":
                    UserName = value;
                    break;
                case "host":
                    Domain = value;
                    break;
                case "expires":
                    int seconds;
                    if (int.TryParse(value, out seconds))
                        Expires = UnixEpoch.AddSeconds(seconds);
                    break;
                case "user-agent":
                    UserAgent = value;
                    break;
                default:
                    return base.ParseParameter(name, value);
            }
            return true;
        }

        public override string ToString()
        {
            return "SofiaExpire(" + UserName + "@" + Domain + ")." + base.ToString();
        }
    }
}