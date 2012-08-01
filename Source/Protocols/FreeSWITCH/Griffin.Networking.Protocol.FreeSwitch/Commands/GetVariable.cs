using System.Collections.Generic;

namespace Griffin.Networking.Protocol.FreeSwitch.Commands
{
    public class GetVariable : Api
    {
        private readonly string _channelId;
        private readonly string _name;

        public GetVariable(string uuid, string name)
            : base("uuid_getvar")
        {
            _channelId = uuid;
            _name = name;
        }

        public override IEnumerable<string> Arguments
        {
            get { return new[] {_channelId, _name}; }
        }
    }

    /*
    public class GetVariableResponseDecoder
    {
        public ICommandReply CreateReply(string dataToParse)
        {
            if (dataToParse.Substring(0, 2) == "OK")
            {
                return new GetVariableReply(true, dataToParse.Substring(3));
            }
            else
            {
                GetVariableReply reply = new GetVariableReply(false, string.Empty);
                reply.ErrorMessage = dataToParse;
                return reply;
            }
        }
    }


    public class GetVariableReply : CommandReply
    {
        private string _value;

        public GetVariableReply(bool success, string value)
            : base(success)
        {
            _value = value;
        }

        public string SessionId
        {
            get { return _value; }
        }

        public string Value
        {
            get { return _value; }
        }
    }
     * */
}