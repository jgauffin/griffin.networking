using Griffin.Networking.Protocol.FreeSwitch.Events.Channel;

namespace Griffin.Networking.Protocol.FreeSwitch.Events.Sip
{
    [EventName("PRESENCE")]
    public class Presence : SipEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Presence"/> class.
        /// </summary>
        public Presence()
        {
            ChannelState = new ChannelStateEvent();
            PresenceEventType = string.Empty;
            Rpid = string.Empty;
            Status = string.Empty;
            Login = string.Empty;
        }

        public string Login { get; set; }

        /// <summary>
        /// "Click to call", "Registered", "unavailable", "Active (%d waiting)", "Idle"
        /// </summary>
        public string Status { get; set; }

        public string Rpid { get; set; }

        /// <summary>
        /// Caller is only specified on state CS_ROUTING
        /// and not on CS_HANGUP
        /// </summary>
        public PartyInfo Caller { get; set; }

        /// <summary>
        /// presence, 
        /// </summary>
        public string PresenceEventType { get; set; }

        /// <summary>
        /// We *may* have channel state info
        /// </summary>
        public ChannelStateEvent ChannelState { get; set; }


        /*
        proto: sip
        login: sip%3Amod_sofia%40192.168.0.58%3A5070
        rpid: unknown
        from: gauffin%40gauffin.com
        status: Registered
         * */

        protected ChannelInfo ChannelInfo { get; set; }

        protected string AlternativeEventType { get; set; }

        protected int EventCount { get; set; }

        protected ChannelState State { get; set; }

        protected bool ReSubscribe { get; set; }

        public override bool ParseParameter(string name, string value)
        {
            var res = ChannelState.ParseParameter(name, value);
            switch (name)
            {
                case "status":
                    Status = value;
                    break;
                case "rpid":
                    Rpid = value;
                    break;
                case "login":
                    Login = value;
                    break;
                case "event_type":
                    PresenceEventType = value;
                    break;
                case "resub":
                    ReSubscribe = value == "true";
                    break;
                case "alt_event_type":
                    AlternativeEventType = value;
                    break;
                case "event_count":
                    EventCount = int.Parse(value);
                    break;
                case "channel-state":
                    State = ChannelStateParser.Parse(value);
                    break;

                case "presence-calling-file":
                    CallingFile = value;
                    break;

                case "presence-calling-function":
                    CallingFunction = value;
                    break;
                case "presence-calling-line":
                    CallingLineNumber = int.Parse(value);
                    break;

                default:
                    if (name.Length > 8 && name.Substring(0, 8) == "channel-")
                    {
                        if (ChannelInfo == null)
                            ChannelInfo = new ChannelInfo();
                        return ChannelInfo.Parse(name, value);
                    }
                    if (name.Length > 7 && name.Substring(0, 7) == "caller-")
                    {
                        if (Caller == null)
                            Caller = new PartyInfo();
                        return Caller.Parse(name.Substring(7), value);
                    }

                    return base.ParseParameter(name, value) || res;
            }
            return true;
        }

        public override string ToString()
        {
            return "Presence(" + Login + ", " + Status + ")." + ChannelState + "." + base.ToString();
        }
    }
}