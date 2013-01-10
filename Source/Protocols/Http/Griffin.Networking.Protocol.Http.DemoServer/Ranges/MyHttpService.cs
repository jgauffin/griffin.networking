using System;
using System.IO;
using System.Net;
using Griffin.Networking.Buffers;
using Griffin.Networking.Protocol.Http.Implementation;
using Griffin.Networking.Protocol.Http.Protocol;

namespace Griffin.Networking.Protocol.Http.DemoServer.Ranges
{
    public class MyHttpService : HttpService
    {
        private static readonly BufferSliceStack Stack = new BufferSliceStack(50, 32000);

        public MyHttpService()
            : base(Stack)
        {
        }

        public override void Dispose()
        {
        }

        public override void HandleReceive(object message)
        {
            var request = (IRequest)message;

            var rangeHeader = request.Headers["Range"];
            if (rangeHeader != null && !string.IsNullOrEmpty(rangeHeader.Value))
            {
                HandleRangeRequest(request);
                return;

            }
            // Indicate that we support ranges.
            var response = request.CreateResponse(HttpStatusCode.OK, "Welcome");

            response.ContentType = "application/octet-stream";
            response.AddHeader("Content-Disposition", @"attachment;filename=""ReallyBigFile.Txt""");
            response.AddHeader("Accept-Ranges", "bytes");
            if (request.Method != "HEAD")
                response.Body = new FileStream(Environment.CurrentDirectory + @"\Ranges\ReallyBigFile.Txt", FileMode.Open,
                                               FileAccess.Read, FileShare.ReadWrite);

            Send(response);
        }

        private void HandleRangeRequest(IRequest request)
        {
            var rangeHeader = request.Headers["Range"];
            var response = request.CreateResponse(HttpStatusCode.PartialContent, "Welcome");

            response.ContentType = "application/octet-stream";
            response.AddHeader("Accept-Ranges", "bytes");
            response.AddHeader("Content-Disposition", @"attachment;filename=""ReallyBigFile.Txt""");

            //var fileStream = new FileStream(Environment.CurrentDirectory + @"\Ranges\ReallyBigFile.Txt", FileMode.Open,
            //                                FileAccess.Read, FileShare.ReadWrite);
            var fileStream = new FileStream(@"C:\Users\jgauffin\Downloads\AspNetMVC3ToolsUpdateSetup.exe", FileMode.Open,
                                                FileAccess.Read, FileShare.ReadWrite);
            var ranges = new RangeCollection();
            ranges.Parse(rangeHeader.Value, (int)fileStream.Length);

            response.AddHeader("Content-Range", ranges.ToHtmlHeaderValue((int)fileStream.Length));
            response.Body = new ByteRangeStream(ranges, fileStream);
            Send(response);
        }
    }
}