using System.Collections.Generic;

namespace Griffin.Networking.Protocol.FreeSwitch.Commands
{
    public class ExecuteJavascript : Api
    {
        private readonly IList<string> _arguments = new List<string>();
        private readonly string[] _scriptArguments;
        private readonly string _scriptName;

        public ExecuteJavascript(string scriptName, params string[] scriptArguments)
            : base("jsrun")
        {
            _scriptName = scriptName;
            _scriptArguments = scriptArguments;
        }

        /// <summary>
        /// Gets command arguments
        /// </summary>
        public override IEnumerable<string> Arguments
        {
            get
            {
                var items = new List<string> {_scriptName};
                items.AddRange(_scriptArguments);
                return items;
            }
        }
    }
}