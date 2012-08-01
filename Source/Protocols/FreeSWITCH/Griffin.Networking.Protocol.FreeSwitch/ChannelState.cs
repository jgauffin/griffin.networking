namespace Griffin.Networking.Protocol.FreeSwitch
{
    /// <summary>
    /// Gets state of an channel
    /// </summary>
    public enum ChannelState
    {
        /// <summary>
        /// Channel is created
        /// </summary>
        New,

        /// <summary>
        /// Initialized
        /// </summary>
        Init,

        /// <summary>
        /// Going through dial plan
        /// </summary>
        Routing,

        /// <summary>
        /// Passive transmit state
        /// </summary>
        SoftExecute,

        /// <summary>
        /// Executing the dial plan
        /// </summary>
        Execute,

        /// <summary>
        /// Connected to another channel
        /// </summary>
        ExchangeMedia,

        /// <summary>
        /// Being parked (not same as held)
        /// </summary>
        Park,

        /// <summary>
        /// Sending media (as .wav) to channel
        /// </summary>
        ConsumeMedia,

        /// <summary>
        /// Channel is sleeping
        /// </summary>
        Hibernate,

        /// <summary>
        /// Channel is being reset.
        /// </summary>
        Reset,

        /// <summary>
        /// Flagged for hangup but not yet terminated.
        /// </summary>
        Hangup,

        /// <summary>
        /// Flag is done and ready to be destroyed.
        /// </summary>
        Done,

        /// <summary>
        /// Remove the channel
        /// </summary>&
        Destroy,

        Reporting,

        /// <summary>
        /// Unknown state.
        /// </summary>
        Unknown
    }

    public static class ChannelStateParser
    {
        public static ChannelState Parse(string value)
        {
            // skip "CS_" prefix.
            return Enumm.Parse<ChannelState>(value.Substring(3).Replace("_", ""));
        }
    }
}