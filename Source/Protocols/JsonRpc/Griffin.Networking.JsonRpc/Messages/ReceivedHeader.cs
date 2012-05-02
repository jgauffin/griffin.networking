using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Networking.JsonRpc.Messages
{
    class ReceivedHeader : IPipelineMessage
    {
        public SimpleHeader Header { get; private set; }

        public ReceivedHeader(SimpleHeader header)
        {
            if (header == null) throw new ArgumentNullException("header");
            Header = header;
        }
    }
}
