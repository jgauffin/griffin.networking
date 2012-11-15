using System;
using Griffin.Networking.Pipelines;

namespace Griffin.Networking.JsonRpc.Messages
{
    internal class ReceivedHeader : IPipelineMessage
    {
        public ReceivedHeader(SimpleHeader header)
        {
            if (header == null) throw new ArgumentNullException("header");
            Header = header;
        }

        public SimpleHeader Header { get; private set; }
    }
}