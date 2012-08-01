namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    [EventName("CHANNEL_DESTROY")]
    public class ChannelDestroy : ChannelStateEvent
    {
        public override string ToString()
        {
            return "ChannelDestroy." + base.ToString();
        }
    }
}