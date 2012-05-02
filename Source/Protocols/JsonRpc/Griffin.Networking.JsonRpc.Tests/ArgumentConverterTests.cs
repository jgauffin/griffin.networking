using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Xunit;

namespace Griffin.Networking.JsonRpc.Tests
{
    public class ArgumentConverterTests
    {
        private ArgumentConverter _converter = new ArgumentConverter();

        [Fact]
        public void DeserializeArray()
        {
            var json = @"[42, 23]";
            var reader= new JsonTextReader(new StringReader(json));
            reader.Read();
            var request = new Request();

            var result = _converter.ReadJson(reader, typeof(Request), request, new JsonSerializer()) as object[];

            Assert.NotNull(result);
            Assert.Equal(42L, result[0]);
            Assert.Equal(23L, result[1]);
        }

        [Fact]
        public void DeserializeObjects()
        {
            var json = @"{""subtrahend"": 23, ""minuend"": 42}";
            var reader = new JsonTextReader(new StringReader(json));
            reader.Read();
            var request = new Request();

            var result = _converter.ReadJson(reader, typeof(Request), request, new JsonSerializer()) as IDictionary<string,object>;

            Assert.NotNull(result);
            Assert.Equal(23L, result["subtrahend"]);
            Assert.Equal(42L, result["minuend"]);
        }

        [Fact]
        public void UnexpectedTokenType()
        {
            var json = @"42";
            var reader = new JsonTextReader(new StringReader(json));
            reader.Read();
            var request = new Request();

            Assert.Throws<FormatException>(
                () =>
                _converter.ReadJson(reader, typeof (Request), request, new JsonSerializer()) as
                IDictionary<string, object>);
        }
    }
}
