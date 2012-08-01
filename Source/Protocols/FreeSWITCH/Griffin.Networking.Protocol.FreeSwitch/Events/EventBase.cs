using System;
using System.Collections.Specialized;
using System.Net;
using Griffin.Networking.Logging;

namespace Griffin.Networking.Protocol.FreeSwitch.Events
{
    public abstract class EventBase : IFreeSwitchEvent
    {
        private readonly ILogger _logger = LogManager.GetLogger(typeof (EventBase));
        private readonly NameValueCollection _parameters = new NameValueCollection();
        private int _callingLineNumber;
        private string _name;

        public string CallingFile { get; set; }

        public string CallingFunction { get; set; }

        public int CallingLineNumber
        {
            get { return _callingLineNumber; }
            set { _callingLineNumber = value; }
        }

        public int EventSequence { get; set; }


        /// <summary>
        /// Get whether this event is for a channel
        /// </summary>
        public virtual bool IsChannelEvent
        {
            get { return false; }
        }

        /// <summary>
        /// Gets parameters that has not been parsed.
        /// </summary>
        public NameValueCollection UnmappedParameters
        {
            get { return _parameters; }
        }


        protected string SwitchHostName { get; set; }

        protected DateTime EventTimeStamp { get; set; }

        protected IPAddress SwitchIp6 { get; set; }

        protected IPAddress SwitchIp4 { get; set; }

        protected string SwitchName { get; set; }

        #region IFreeSwitchEvent Members

        public UniqueId CoreId { get; set; }
        public DateTime LocalDate { get; set; }
        public DateTime UtcDate { get; set; }

        #endregion

        public void Parse(NameValueCollection parameters)
        {
            for (var i = 0; i < parameters.Count; ++i)
            {
                var name = parameters.GetKey(i).ToLower();
                var value = parameters.Get(i);
                ParseParameter(name, value);
            }
        }

        /// <summary>
        /// Parse a parameter from FreeSWITCH
        /// </summary>
        /// <param name="name">Parameter name as defined by FS</param>
        /// <param name="value">Parameter value</param>
        /// <returns>true if parsed sucessfully; otherwise false.</returns>
        public virtual bool ParseParameter(string name, string value)
        {
            switch (name)
            {
                case "event-name":
                    _name = value;
                    break;
                case "event-sequence":
                    EventSequence = int.Parse(value);
                    break;
                case "core-uuid":
                    CoreId = new UniqueId(value);
                    break;
                case "event-date-local":
                    try
                    {
                        LocalDate = DateTime.Parse(value);
                    }
                    catch (Exception err)
                    {
                        _logger.Error("Failed to parse event-date-local: " + value, err);
                        Console.WriteLine("event-date-local(" + value + "): " + err);
                    }
                    break;
                case "event-date-gmt":
                    try
                    {
                        UtcDate = DateTime.Parse(value);
                    }
                    catch (Exception err)
                    {
                        _logger.Error("Failed to parse event-date-gmt: " + value, err);
                        Console.WriteLine("event-date-gmt(" + value + "): " + err);
                    }
                    break;
                case "event-calling-file":
                    CallingFile = value;
                    break;
                case "event-calling-function":
                    CallingFunction = value;
                    break;
                case "event-calling-line-number":
                    int.TryParse(value, out _callingLineNumber);
                    break;
                case "freeswitch-switchname":
                    SwitchName = value;
                    break;
                case "freeswitch-hostname":
                    SwitchHostName = value;
                    break;
                case "freeswitch-ipv4":
                    SwitchIp4 = IPAddress.Parse(value);
                    break;

                case "freeswitch-ipv6":
                    SwitchIp6 = IPAddress.Parse(value);
                    break;

                case "event-date-timestamp":
                    EventTimeStamp = value.FromUnixTime();
                    break;

                default:
                    _logger.Debug(string.Format("Unmapped: {0} = {1}", name, value));
                    UnmappedParameters.Add(name, value);
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            return "Event(" + _name + ")";
        }
    }
}