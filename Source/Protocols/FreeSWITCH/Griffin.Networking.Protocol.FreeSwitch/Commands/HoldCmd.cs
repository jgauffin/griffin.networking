using System.Collections.Generic;

namespace Griffin.Networking.Protocol.FreeSwitch.Commands
{
    public class HoldCmd : Api
    {
        private readonly bool _activate;
        private readonly string _sessionId;

        public HoldCmd(string sessionId, bool activate)
            : base("uuid_hold")
        {
            _sessionId = sessionId;
            _activate = activate;
        }

        public override IEnumerable<string> Arguments
        {
            get { return new[] {_activate ? string.Empty : "off ", _sessionId}; }
        }
    }
}