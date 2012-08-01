using Griffin.Networking.Protocol.FreeSwitch.Net;

namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    [EventName("CHANNEL_HANGUP")]
    public class ChannelHangup : ChannelStateEvent
    {
        private HangupCause _hangupCause;


        public HangupCause Cause
        {
            get { return _hangupCause; }
            set { _hangupCause = value; }
        }

        public override bool ParseParameter(string name, string value)
        {
            if (name == "hangupcause" || name == "hangup-cause")
            {
                var cause = value.UnderscoreToCamelCase();
                _hangupCause = Enumm.Parse<HangupCause>(cause);
            }
            else
                return base.ParseParameter(name, value);

            return true;
        }

        public override string ToString()
        {
            return "ChannelHangup(" + _hangupCause + ")." + base.ToString();
        }
    }
}