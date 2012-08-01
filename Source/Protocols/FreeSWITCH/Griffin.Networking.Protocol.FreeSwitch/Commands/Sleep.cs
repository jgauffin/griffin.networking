using System;

namespace Griffin.Networking.Protocol.FreeSwitch.Commands
{
    /// <summary>
    /// Sleep a period of time
    /// </summary>
    [ContainerType(ContainerType.SendMsg)]
    public class Sleep : IChannelCommand
    {
        private readonly TimeSpan _duration;
        private readonly UniqueId _id;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sleep"/> class.
        /// </summary>
        /// <param name="id">Channel id.</param>
        /// <param name="duration">The duration.</param>
        public Sleep(UniqueId id, TimeSpan duration)
        {
            if (id == null) throw new ArgumentNullException("id");
            if (_duration == TimeSpan.MinValue)
                throw new ArgumentException("Duration must be larger than 0 ms.", "duration");

            _id = id;
            _duration = duration;
        }

        #region IChannelCommand Members

        /// <summary>
        /// Convert command to a string that can be sent to FreeSWITCH
        /// </summary>
        /// <returns>FreeSWITCH command</returns>
        public string ToFreeSwitchString()
        {
            return string.Format("sleep {0}", _duration.TotalMilliseconds);
        }

        /// <summary>
        /// Gets channel id
        /// </summary>
        public UniqueId ChannelId
        {
            get { return _id; }
        }

        #endregion
    }
}