namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    /// <summary>
    /// Reports when DTMF digits have been pressed
    /// </summary>
    [EventName("DTMF_STATUS")]
    public class DtmfStatus : ChannelBase
    {
        private int _count;

        /// <summary>
        /// Number of digits in the buffer.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }


        public override bool ParseParameter(string name, string value)
        {
            switch (name)
            {
                case "dtmf-count":
                    int.TryParse(value, out _count);
                    break;
            }

            return base.ParseParameter(name, value);
        }

        public override string ToString()
        {
            return "DtmfStatus." + base.ToString();
        }
    }
}