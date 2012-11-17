using System;
using System.Net;
using Griffin.Networking.Http.Messages;
using Griffin.Networking.Http.Pipeline.Messages;
using Griffin.Networking.Http.Services.Files;
using Griffin.Networking.Pipelines;

namespace Griffin.Networking.Http.Handlers
{
    /// <summary>
    /// Serves files 
    /// </summary>
    public class FileHandler : IUpstreamHandler
    {
        private readonly IFileService _fileService;
        private readonly MimeTypeProvider _mimeTypeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileHandler"/> class.
        /// </summary>
        /// <param name="fileService">The file service.</param>
        public FileHandler(IFileService fileService, MimeTypeProvider mimeTypeProvider)
        {
            _fileService = fileService;
            _mimeTypeProvider = mimeTypeProvider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileHandler"/> class.
        /// </summary>
        /// <remarks>Initializes using a <see cref="DiskFileService"/> with current directory as root.</remarks>
        public FileHandler()
        {
            _fileService = new DiskFileService("/", Environment.CurrentDirectory);
            _mimeTypeProvider = new MimeTypeProvider();
        }

        #region IUpstreamHandler Members

        /// <summary>
        /// Handle an message
        /// </summary>
        /// <param name="context">Context unique for this handler instance</param>
        /// <param name="message">Message to process</param>
        /// <remarks>
        /// All messages that can't be handled MUST be send up the chain using <see cref="IPipelineHandlerContext.SendUpstream"/>.
        /// </remarks>
        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            var msg = message as ReceivedHttpRequest;
            if (msg == null)
            {
                context.SendUpstream(message);
                return;
            }

            var ifModifiedSince = DateTime.MinValue;
            var header = msg.HttpRequest.Headers["If-Modified-Since"];
            if (header != null)
            {
                ifModifiedSince = DateTime.Parse(header.Value).ToUniversalTime();

                // Allow for file systems with subsecond time stamps
                ifModifiedSince = new DateTime(ifModifiedSince.Year, ifModifiedSince.Month, ifModifiedSince.Day,
                                               ifModifiedSince.Hour, ifModifiedSince.Minute, ifModifiedSince.Second,
                                               ifModifiedSince.Kind);
            }


            var fileContext = new FileContext(msg.HttpRequest, ifModifiedSince);
            _fileService.GetFile(fileContext);
            if (fileContext.LastModifiedAtUtc > DateTime.MinValue)
            {
                var response = msg.HttpRequest.CreateResponse(HttpStatusCode.OK, "File found");
                var filename = msg.HttpRequest.Uri.Segments[msg.HttpRequest.Uri.Segments.Length - 1];
                response.ContentType = _mimeTypeProvider.Get(filename);
                if (fileContext.FileStream == null)
                {
                    response.StatusDescription = "File have not changed since " + fileContext.LastModifiedAtUtc;
                    response.StatusCode = (int) HttpStatusCode.NotModified;
                }
                else
                {
                    response.AddHeader("Last-Modified", fileContext.LastModifiedAtUtc.ToString("R"));
                    response.Body = fileContext.FileStream;
                }

                context.SendDownstream(new SendHttpResponse(msg.HttpRequest, response));
                return;
            }


            context.SendUpstream(message);
        }

        #endregion
    }
}