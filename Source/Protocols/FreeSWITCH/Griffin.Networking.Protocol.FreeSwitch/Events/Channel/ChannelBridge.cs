namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    [EventName("CHANNEL_BRIGE")]
    public class ChannelBridge : ChannelStateEvent
    {
        public override string ToString()
        {
            return "ChannelBridge." + base.ToString();
        }
    }
}