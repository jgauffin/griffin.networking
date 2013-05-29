using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Griffin.Networking.Protocol.Http.Implementation;
using Xunit;

namespace Griffin.Networking.Http.Tests.Implementation
{

    public class HttpMessageTests
    {
        [Fact]
        public void try_encoding_if_in_the_middle()
        {
            var sut = new HttpMessage();
            sut.ContentEncoding = Encoding.ASCII;

            sut.AddHeader("content-type", "text/html;charset=ISO-8859-1;some=more;");

            sut.ContentEncoding.Should().Be(Encoding.GetEncoding("ISO-8859-1"));
        }

        [Fact]
        public void try_encoding_if_in_the_end()
        {
            var sut = new HttpMessage();
            sut.ContentEncoding = Encoding.ASCII;

            sut.AddHeader("content-type", "text/html;charset=utf-8;");

            sut.ContentEncoding.Should().Be(Encoding.UTF8);
        }

        [Fact]
        public void try_encoding___no_semicolon()
        {
            var sut = new HttpMessage();
            sut.ContentEncoding = Encoding.ASCII;

            sut.AddHeader("content-type", "text/html;charset=ascii");

            sut.ContentEncoding.Should().Be(Encoding.ASCII);
        }
    }
}
