namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    [EventName("SESSION_CRASH")]
    public class SessionCrash : ChannelBase
    {
        public override string ToString()
        {
            return "SessionCrash." + base.ToString();
        }
    }
}