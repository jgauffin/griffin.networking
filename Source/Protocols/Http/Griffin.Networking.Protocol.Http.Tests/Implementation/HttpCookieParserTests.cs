using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Protocol.Http.Implementation;
using Xunit;

namespace Griffin.Networking.Http.Tests.Implementation
{
    public class HttpCookieParserTests
    {
        [Fact]
        public void Test()
        {
            var parser = new HttpCookieParser(@"__qca=P0-1267122445-1340133197581; gauthed=1; km_lv=x; km_uq=; kvcd=1348223997344; km_ai=28603; km_ni=28603; usr=t=XXjSGVAt1Euf&s=ng3ePzX9NUKq; __utma=140029553.1710528871.1340133198.1355396765.1355406563.180; __utmb=140029553.1.10.1355406563; __utmc=140029553; __utmz=140029553.1355396765.179.62.utmcsr=google|utmccn=(organic)|utmcmd=organic|utmctr=(not%20provided)");
            var cookies = parser.Parse();

            Assert.Equal(12, cookies.Count);
            Assert.Equal("P0-1267122445-1340133197581", cookies["__qca"].Value);
            Assert.Equal("1", cookies["gauthed"].Value);
            Assert.Equal("x", cookies["km_lv"].Value);
            Assert.Equal("", cookies["km_uq"].Value);
            Assert.Equal("1348223997344", cookies["kvcd"].Value);
            Assert.Equal("28603", cookies["km_ai"].Value);
            Assert.Equal("28603", cookies["km_ni"].Value);
            Assert.Equal("140029553.1355396765.179.62.utmcsr=google|utmccn=(organic)|utmcmd=organic|utmctr=(not%20provided)", cookies["__utmz"].Value);
        }
    }
}
