using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Buffers;
using Griffin.Networking.Http.Pipeline.Handlers;
using Xunit;

namespace Griffin.Networking.Http.Tests.Implementation
{
    public class HttpParserTests
    {
        [Fact]
        public void Parse()
        {
            var buffer = Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\nSERVER: LOCALHOST\r\n\r\n");
            var slice = new BufferSlice(buffer, 0, buffer.Length);
            var reader = new SliceStream(slice);
            
            
            var parser = new MyHttpParser();
            parser.HeaderParsed += (sender, args) => Console.WriteLine(args.Name + ": " + args.Value);
            parser.Parse(reader);
        }

    }
}
