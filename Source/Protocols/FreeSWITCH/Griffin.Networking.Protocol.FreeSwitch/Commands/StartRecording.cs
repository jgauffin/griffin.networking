using System;

namespace Griffin.Networking.Protocol.FreeSwitch.Commands
{
    /// <summary>
    /// Record a channel
    /// </summary>
    public class StartRecording : IChannelCommand
    {
        private readonly UniqueId _id;
        private readonly string _path;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartRecording"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="path">FreeSwitch relative path (directory + filename).</param>
        public StartRecording(UniqueId id, string path)
        {
            if (id == null) throw new ArgumentNullException("id");
            if (path == null) throw new ArgumentNullException("path");
            _id = id;
            _path = path;
            RecordingLimit = TimeSpan.MinValue;
        }

        /// <summary>
        /// Gets or sets number of seconds that can be recorded.
        /// </summary>
        public TimeSpan RecordingLimit { get; set; }

        #region IChannelCommand Members

        /// <summary>
        /// Convert command to a string that can be sent to FreeSWITCH
        /// </summary>
        /// <returns>FreeSWITCH command</returns>
        public string ToFreeSwitchString()
        {
            return RecordingLimit != TimeSpan.MinValue
                       ? string.Format("uuid_record {0} start '{1}' {2}", _id, _path, RecordingLimit)
                       : string.Format("uuid_record {0} start '{1}'", _id, _path);
        }

        /// <summary>
        /// Gets channel id
        /// </summary>
        public UniqueId ChannelId
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}