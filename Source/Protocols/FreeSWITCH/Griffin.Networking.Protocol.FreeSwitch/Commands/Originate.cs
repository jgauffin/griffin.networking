using System.Collections.Generic;
using System.Linq;
using Griffin.Networking.Protocol.FreeSwitch.Net.Messages;

namespace Griffin.Networking.Protocol.FreeSwitch.Commands
{
    public class OriginateReply : CommandReply
    {
        private readonly UniqueId _sessionId;

        public OriginateReply(ICommand command, bool result, UniqueId sessionId)
            : base(command, result, sessionId.ToString())
        {
            _sessionId = sessionId;
        }

        public UniqueId SessionId
        {
            get { return _sessionId; }
        }
    }

    public class Originate : ICommand
    {
        private readonly IList<ChannelVariable> _variables = new List<ChannelVariable>();
        private IEndpointAddress _caller;

        /// <summary>
        /// Initializes a new instance of the <see cref="Originate"/> class.
        /// </summary>
        public Originate()
        {
        }

        public Originate(IEndpointAddress caller, IEndpointAddress destination)
        {
            _caller = caller;
            Destination = destination;
        }

        public IEndpointAddress Caller
        {
            get { return _caller; }
            set { _caller = value; }
        }

        public IEndpointAddress Destination { get; set; }

        public IEnumerable<ChannelVariable> Variables
        {
            get { return _variables; }
        }

        /// <summary>
        /// Gets or sets caller id
        /// </summary>
        /// <remarks>Overrides <see cref="Caller"/> (if it implements <see cref="ICallerIdProvider"/>).</remarks>
        public CallerId CallerId { get; set; }


        public bool AutoAnswer { get; set; }

        #region ICommand Members

        /// <summary>
        /// Convert command to a string that can be sent to FreeSWITCH
        /// </summary>
        /// <returns>FreeSWITCH command</returns>
        public string ToFreeSwitchString()
        {
            if (CallerId != null)
            {
                var name = CallerId.Name;
                if (name != null)
                    AddVariable(Variable.Originator.CallerId.Name, "'" + name + "'");
                if (CallerId.Number != null)
                    AddVariable(Variable.Originator.CallerId.Number, CallerId.Number.ToString());
            }
            if (Caller is ICallerIdProvider && !Variables.Any(x => x.Name == Variable.Originator.CallerId.Number))
            {
                var callerId = (ICallerIdProvider)Caller;
                if (callerId.CallerIdName != null)
                    AddVariable(Variable.Originator.CallerId.Name, "'" + callerId.CallerIdName + "'");
                if (callerId.CallerIdNumber != null)
                    AddVariable(Variable.Originator.CallerId.Number, callerId.CallerIdNumber);
            }

            if (AutoAnswer)
            {
                /*_variables.Add(new CallVariable("sip_invite_params", "intercom=true"));*/
                AddVariable(Variable.Sip.CallInfo, "answer-after=0");
                AddVariable(Variable.Sip.AutoAnswer, "true");
            }


            var variables = string.Empty;
            foreach (var variable in _variables)
                variables += variable + ",";

            if (variables.Length > 0)
                variables = "{" + variables.Remove(variables.Length - 1, 1) + "}";

            return string.Format("originate {0}{1} {2}", variables, Caller.ToDialString(), Destination.ToDialString());
        }

        #endregion

        public void AddVariable(string name, string value)
        {
            _variables.Add(new ChannelVariable(name, value));
        }

        public CommandReply CreateReply(string dataToParse)
        {
            var nameValue = dataToParse.Split(' ');
            if (nameValue[0].Length > 0 && nameValue[0][0] == '+')
                return new OriginateReply(this, true, new UniqueId(nameValue[1]));

            return new CommandReply(this, false, nameValue.Length > 1 ? nameValue[1] : dataToParse);
        }
    }
}