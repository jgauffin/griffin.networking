using Griffin.Networking.Protocol.Http.Implementation;
using Xunit;

namespace Griffin.Networking.Protocol.Http.Tests.Infrastructure
{
    public class NameValueParserTests
    {
        [Fact]
        public void DecodeAuthorization()
        {
            var str =
                @"username=""ddssd"", realm=""DragonsDen"", nonce=""f09b846b702648ba871d82a6f908a6cc"", uri=""/"", algorithm=MD5, response=""d02b37c0e90773b21d3b8c8c448b1e9b"", qop=auth, nc=00000006, cnonce=""ad22c414546923eb""";
            var parameters = new ParameterCollection();
            var parser = new NameValueParser();

            parser.Parse(str, parameters);

            Assert.Equal("ddssd", parameters["username"]);
            Assert.Equal("DragonsDen", parameters["realm"]);
            Assert.Equal("f09b846b702648ba871d82a6f908a6cc", parameters["nonce"]);
            Assert.Equal("/", parameters["uri"]);
            Assert.Equal("MD5", parameters["algorithm"]);
            Assert.Equal("d02b37c0e90773b21d3b8c8c448b1e9b", parameters["response"]);
            Assert.Equal("auth", parameters["qop"]);
            Assert.Equal("00000006", parameters["nc"]);
            Assert.Equal("ad22c414546923eb", parameters["cnonce"]);
        }
    }
}