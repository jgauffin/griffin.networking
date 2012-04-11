using System;
using System.IO;
using System.Text;
using Griffin.Networking.Buffers;
using Griffin.Networking.Messages;
using Griffin.Networking.Protocols.Http.Messages;
using Griffin.Networking.Protocols.Http.Specification;

namespace Griffin.Networking.Protocols.Http
{
    /// <summary>
    /// Encode message to something that can be sent over the wire.
    /// </summary>
    public class Encoder : IDownstreamHandler
    {
        readonly BufferPool _pool = new BufferPool(65536, 100, 10000);

        public void HandleDownstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            var msg = message as SendHttpResponse;
            if (msg == null)
            {
                context.SendDownstream(message);
                return;
            }

            var stream = SerializeHeaders(msg.Response);
            context.SendDownstream(new SendStream(stream));
            context.SendDownstream(new SendStream(msg.Response.Body));
        }


        /// <summary>
        /// Send all headers to the client
        /// </summary>
        /// <param name="response">Response containing call headers.</param>
        public Stream SerializeHeaders(IResponse response)
        {
            var stream = new BufferPoolStream(_pool);
            var writer = new StreamWriter(stream);

            writer.WriteLine("{0} {1} {2}", response.ProtocolVersion, (int)response.StatusCode, response.StatusDescription);

            var contentType = response.ContentType ?? "text/html";
            if (response.ContentEncoding != null)
                contentType += ";charset=" + response.ContentEncoding.WebName;

            // go through all property headers.
            writer.WriteLine("Content-Type: {0}", contentType);
            writer.WriteLine("Content-Length: {0}", response.ContentLength);
            writer.WriteLine(response.KeepAlive ? "Connection: Keep-Alive" : "Connection: Close");

            if (response.Cookies != null && response.Cookies.Count > 0)
            {
                SerializeCookies(response, writer);
            }

            foreach (var header in response.Headers)
                writer.WriteLine("{0}: {1}\r\n", header.Key, header.Value);

            writer.WriteLine();
            return stream;
        }

        private static void SerializeCookies(IResponse response, TextWriter writer)
        {
            //Set-Cookie: <name>=<value>[; <name>=<value>][; expires=<date>][; domain=<domain_name>][; path=<some_path>][; secure][; httponly]
            
            foreach (var cookie in response.Cookies)
            {
                writer.Write("Set-Cookie: {0}={1}", cookie.Name, cookie.Value ?? string.Empty);

                if (cookie.Expires > DateTime.MinValue)
                    writer.Write(";expires={0}", cookie.Expires.ToString("R"));
                if (!string.IsNullOrEmpty(cookie.Path))
                    writer.Write(";path={0}", cookie.Path);

                writer.WriteLine();
            }
        }
    }
}