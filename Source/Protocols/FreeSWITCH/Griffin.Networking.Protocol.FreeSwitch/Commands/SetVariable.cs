using System;

namespace Griffin.Networking.Protocol.FreeSwitch.Commands
{
    /// <summary>
    /// Set a variable on a channel
    /// </summary>
    public class SetVariable : IChannelCommand
    {
        private readonly UniqueId _channelId;
        private readonly string _name;
        private readonly string _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetVariable"/> class.
        /// </summary>
        /// <param name="id">Channel id.</param>
        /// <param name="name">Variable name as declared by FreeSWITCH.</param>
        /// <param name="value">Variable value, <see cref="string.Empty"/> if you which to unset it.</param>
        public SetVariable(UniqueId id, string name, string value)
        {
            if (id == null) throw new ArgumentNullException("id");
            if (name == null) throw new ArgumentNullException("name");
            if (value == null) throw new ArgumentNullException("value");
            _channelId = id;
            _name = name;
            _value = value;
        }

        #region IChannelCommand Members

        /// <summary>
        /// Convert command to a string that can be sent to FreeSWITCH
        /// </summary>
        /// <returns>FreeSWITCH command</returns>
        public string ToFreeSwitchString()
        {
            return string.Format("uuid_setvar {0} {1} {2}", _channelId, _name, _value);
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