using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Http.Implementation;
using Xunit;

namespace Griffin.Networking.Http.Tests.Infrastructure
{
    public class NameValueParserTests
    {
        [Fact]
        public void DecodeAuthorization()
        {
            var str =
                @"username=""Jonas"", realm=""localhost"", nonce=""836e689049bc4d7786d924c74fd03154"", uri=""/"", algorithm=MD5, response=""6585f223a56ddaafafff7f8db5aa77e0"", opaque=""b336fbc1c26c473580ec730851e71aa3"", qop=auth, nc=00000001, cnonce=""a9b3b4d9aa523026""";
            var parameters = new ParameterCollection();
            var parser = new NameValueParser();

            parser.Parse(str, parameters);

            Assert.Equal("Jonas", parameters["username"]);
            Assert.Equal("localhost", parameters["realm"]);
            Assert.Equal("836e689049bc4d7786d924c74fd03154", parameters["nonce"]);
            Assert.Equal("/", parameters["uri"]);
            Assert.Equal("MD5", parameters["algorithm"]);
            Assert.Equal("6585f223a56ddaafafff7f8db5aa77e0", parameters["response"]);
            Assert.Equal("b336fbc1c26c473580ec730851e71aa3", parameters["opaque"]);
            Assert.Equal("00000001", parameters["nc"]);
        }
    }
}
