namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    [EventName("CHANNEL_UNPARK")]
    public class ChannelUnpark : ChannelStateEvent
    {
        public override string ToString()
        {
            return "ChannelUnpark." + base.ToString();
        }
    }
}