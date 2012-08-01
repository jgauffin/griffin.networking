namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    [EventName("CHANNEL_CREATE")]
    public class ChannelCreate : ChannelStateEvent
    {
        public override string ToString()
        {
            return "ChannelCreate." + base.ToString();
        }
    }
}