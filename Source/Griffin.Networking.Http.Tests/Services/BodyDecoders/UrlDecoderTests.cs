using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Http.Implementation.Infrastructure;
using Xunit;

namespace Griffin.Networking.Http.Tests.Services.BodyDecoders
{
    public class UrlDecoderTests
    {
        [Fact]
        public void TwoSimplePairs()
        {
            var actual = @"jonas%3Dking%26sara%3Dqueen";

            var decoder = new UrlDecoder();
            var result = decoder.Parse(actual);

            Assert.Equal("king", result["jonas"]);
            Assert.Equal("queen", result["sara"]);
        }
    }
}
