namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    [EventName("CHANNEL_UNBRIDGE")]
    public class ChannelUnbridge : ChannelStateEvent
    {
        public override string ToString()
        {
            return "ChannelUnbridge." + base.ToString();
        }
    }
}