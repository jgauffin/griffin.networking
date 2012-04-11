using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Channels;

namespace Griffin.Networking.Messages
{
    class ChildChannelConnected : IPipelineMessage
    {
        public ChildChannelConnected(TcpServerChildChannel client)
        {
            
        }
    }
}
