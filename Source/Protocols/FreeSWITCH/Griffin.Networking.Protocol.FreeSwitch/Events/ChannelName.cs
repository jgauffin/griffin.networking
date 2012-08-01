using System.Diagnostics;

namespace Griffin.Networking.Protocol.FreeSwitch.Events
{
    /// <summary>
    /// Converts FS channel name into small
    /// </summary>
    public class ChannelName
    {
        /// <summary>
        /// Empty instance
        /// </summary>
        public static readonly ChannelName Empty = new ReadOnlyChannelName();

        private string _domainName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelName"/> class.
        /// </summary>
        public ChannelName()
        {
            _domainName = string.Empty;
        }

        /// <summary>
        /// ProfileName for the dial plan/sip.
        /// </summary>
        public virtual string ProfileName { get; set; }

        /// <summary>
        /// Gets or sets user name for the channel
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// Where the extension/user is calling from.
        /// </summary>
        public virtual string DomainName
        {
            get { return _domainName; }
            set
            {
                if (value == null)
                {
                    _domainName = string.Empty;
                    return;
                }

                var pos = value.IndexOf(':');
                _domainName = pos == -1 ? value : value.Substring(0, pos);
            }
        }

        /// <summary>
        /// Gets or sets enpoint type such as "sofia"
        /// </summary>
        public virtual string EndpointTypeName { get; set; }


        /// <summary>
        /// Gets or set endpoint protocol such as "sip"
        /// </summary>
        protected string Protocol { get; set; }

        /// <summary>
        /// Parse FS formatted channel name
        /// </summary>
        /// <param name="value">String to parse</param>
        public void Parse(string value)
        {
            Debug.Assert(!string.IsNullOrEmpty(value));

            var bits = value.Split('/');
            if (bits.Length != 3)
                return;

            EndpointTypeName = bits[0];
            ProfileName = bits[1];
            var userParts = bits[2].Split('@');
            if (userParts.Length == 2)
            {
                UserName = userParts[0];
                DomainName = userParts[1];
            }
            else
                UserName = bits[2];

            var colonPos = UserName.IndexOf(":");
            var atPos = UserName.IndexOf("@");
            if (colonPos != -1)
            {
                if (atPos > colonPos || atPos == -1)
                {
                    Protocol = UserName.Substring(0, colonPos);
                    UserName = UserName.Remove(0, colonPos + 1);
                }
            }
        }


        /// <summary>
        /// Returns FS formatted string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(UserName))
                return "NoName";

            return EndpointTypeName + "/" + ProfileName + "/" + UserName + "@" + _domainName;
        }
    }
}