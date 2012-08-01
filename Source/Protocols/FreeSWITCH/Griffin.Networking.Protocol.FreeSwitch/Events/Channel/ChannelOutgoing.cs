namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    [EventName("CHANNEL_OUTGOING")]
    public class ChannelOutgoing : ChannelStateEvent
    {
        public override string ToString()
        {
            return "ChannelOutgoing." + base.ToString();
        }
    }
}