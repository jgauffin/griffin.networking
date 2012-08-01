namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    [EventName("CHANNEL_CALLSTATE")]
    public class ChannelCallState : ChannelStateEvent
    {
        protected CallState OriginalChannelCallState { get; set; }

        /// <summary>
        /// Parse a parameter from FreeSWITCH
        /// </summary>
        /// <param name="name">Parameter name as defined by FS</param>
        /// <param name="value">Parameter value</param>
        /// <returns>
        /// true if parsed sucessfully; otherwise false.
        /// </returns>
        public override bool ParseParameter(string name, string value)
        {
            switch (name)
            {
                case "original-channel-call-state":
                    OriginalChannelCallState = Enumm.Parse<CallState>(value);
                    break;
                default:
                    return base.ParseParameter(name, value);
            }

            return true;
        }
    }
}