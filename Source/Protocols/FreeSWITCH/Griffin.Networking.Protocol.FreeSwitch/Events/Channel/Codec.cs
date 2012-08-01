namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    [EventName("CODEC")]
    public class Codec : ChannelStateEvent
    {
        public override string ToString()
        {
            return "Codec." + base.ToString();
        }
    }
}