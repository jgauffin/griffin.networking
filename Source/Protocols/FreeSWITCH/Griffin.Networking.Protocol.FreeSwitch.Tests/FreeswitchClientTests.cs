using System;
using System.Net;
using System.Security;
using System.Threading;
using Xunit;

namespace Griffin.Networking.Protocol.FreeSwitch.Tests
{
    public class FreeswitchClientTests
    {
        [Fact]
        public void Connect()
        {
            var pwd = new SecureString();
            foreach (var ch in "ClueCon".ToCharArray())
                pwd.AppendChar(ch);
            var client = new FreeSwitchClient(pwd, FreeSwitchEventCollection.GetChannelEvents());
            client.Connect(new IPEndPoint(IPAddress.Loopback, 8021));
            Thread.Sleep(5000);
        }
    }

}
