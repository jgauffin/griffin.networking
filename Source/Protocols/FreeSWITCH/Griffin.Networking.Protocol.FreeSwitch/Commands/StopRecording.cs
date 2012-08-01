namespace Griffin.Networking.Protocol.FreeSwitch.Commands
{
    /// <summary>
    /// Stop recording sound on the channel
    /// </summary>
    public class StopRecording : IChannelCommand
    {
        private readonly UniqueId _channelId;

        /// <summary>
        /// Initializes a new instance of the <see cref="StopRecording"/> class.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        public StopRecording(UniqueId channelId)
        {
            _channelId = channelId;
        }

        #region IChannelCommand Members

        /// <summary>
        /// Convert command to a string that can be sent to FreeSWITCH
        /// </summary>
        /// <returns>FreeSWITCH command</returns>
        public string ToFreeSwitchString()
        {
            return string.Format("uuid_record {0}", _channelId);
        }

        /// <summary>
        /// Gets channel id
        /// </summary>
        public UniqueId ChannelId
        {
            get { return _channelId; }
        }

        #endregion
    }
}