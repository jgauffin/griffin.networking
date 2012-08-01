using System;

namespace Griffin.Networking.Protocol.FreeSwitch.Events
{
    public class ReadOnlyChannelName : ChannelName
    {
        /// <summary>
        /// Extension / User name for the channel
        /// </summary>
        /// <value></value>
        public override string UserName
        {
            get { return base.UserName; }
            set { throw new InvalidOperationException("Channel name is read-only."); }
        }

        /// <summary>
        /// Where the extension/user is calling from.
        /// </summary>
        /// <value></value>
        public override string DomainName
        {
            get { return base.DomainName; }
            set { throw new InvalidOperationException("Channel name is read-only."); }
        }

        /// <summary>
        /// ProfileName for the dial plan/sip.
        /// </summary>
        /// <value></value>
        public override string ProfileName
        {
            get { return base.ProfileName; }
            set { throw new InvalidOperationException("Channel name is read-only."); }
        }

        /// <summary>
        /// Gets or sets protocol used for communication.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// 	<c>sofia</c> = sip
        /// </remarks>
        public override string EndpointTypeName
        {
            get { return base.EndpointTypeName; }
            set { throw new InvalidOperationException("Channel name is read-only."); }
        }
    }
}