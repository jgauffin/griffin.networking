namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    [EventName("CHANNEL_APPLICATION")]
    public class ChannelApplication : ChannelStateEvent
    {
        public override string ToString()
        {
            return "ChannelApplication." + base.ToString();
        }
    }
}