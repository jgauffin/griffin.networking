namespace Griffin.Networking.Protocol.FreeSwitch
{
    /// <summary>
    /// A freeswitch variable
    /// </summary>
    /// <remarks>
    /// Channel variables can be access from FreeSWITCH with the name
    /// "variable_[name]"
    /// </remarks>
    public class ChannelVariable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelVariable"/> class.
        /// </summary>
        /// <param name="name">Name of variable.</param>
        /// <param name="value">Value.</param>
        public ChannelVariable(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }

        public string Value { get; private set; }

        public override string ToString()
        {
            return Name + "=" + Value;
        }
    }
}