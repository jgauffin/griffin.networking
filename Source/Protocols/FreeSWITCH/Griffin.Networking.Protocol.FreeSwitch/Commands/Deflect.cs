using System;

namespace Griffin.Networking.Protocol.FreeSwitch.Commands
{
    /// <summary>
    /// Deflect an answered SIP call off of FreeSWITCH by sending the REFER method
    /// </summary>
    public class Deflect : IChannelCommand
    {
        private readonly SipAddress _address;
        private readonly UniqueId _id;

        /// <summary>
        /// Initializes a new instance of the <see cref="Deflect"/> class.
        /// </summary>
        /// <param name="id">Channel id</param>
        /// <param name="address">Sip address to deflect to</param>
        public Deflect(UniqueId id, SipAddress address)
        {
            if (id == null) throw new ArgumentNullException("id");
            if (address == null) throw new ArgumentNullException("address");

            _id = id;
            _address = address;
        }

        #region IChannelCommand Members

        /// <summary>
        /// Convert command to a string that can be sent to FreeSWITCH
        /// </summary>
        /// <returns>FreeSWITCH command</returns>
        public string ToFreeSwitchString()
        {
            return string.Format("uuid_deflect {0} {1}", _id, _address);
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