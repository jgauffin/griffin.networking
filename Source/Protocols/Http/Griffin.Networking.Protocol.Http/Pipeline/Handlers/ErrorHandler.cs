using System;
using System.IO;
using System.Text;
using Griffin.Networking.Http.Messages;
using Griffin.Networking.Http.Pipeline.Handlers;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Http.Services.Authentication;
using Griffin.Networking.Http.Services.Errors;
using Griffin.Networking.Logging;
using Griffin.Networking.Pipelines;

namespace Griffin.Networking.Http.Handlers
{
    /// <summary>
    /// Pipeline handler catching unhandled exceptions
    /// </summary>
    /// <remarks>
    /// <para>
    /// The handler uses try/catch around <c>context.SendUpstream</c> which allows it to catch any unhandled
    /// exceptions that all upstreams handler after this one throws. It then logs the exception, all request parameters and finally invokes the 
    /// <see cref="IErrorFormatter"/> before sending the response back to the client.
    /// </para>
    /// <para>Should typically be placed right after the <see cref="HeaderDecoder"/> in the pipeline</para>
    /// <para>
    /// You need to implement your own adapter for the <see cref="LogManager"/> to receive all errors.
    /// </para>
    /// </remarks>
    public class HttpErrorHandler : IUpstreamHandler
    {
        private readonly IErrorFormatter _formatter;
        private readonly ILogger _logger = LogManager.GetLogger<HttpErrorHandler>();

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpErrorHandler"/> class.
        /// </summary>
        /// <param name="formatter">Used to format the response using the uncaught exception.</param>
        public HttpErrorHandler(IErrorFormatter formatter)
        {
            if (formatter == null) throw new ArgumentNullException("formatter");
            _formatter = formatter;
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

            try
            {
                context.SendUpstream(message);
            }
            catch (HttpException err)
            {
                var response = msg.HttpRequest.CreateResponse(err.StatusCode, err.Message);

                FormatException(response, msg, err);
                response.Body.Position = 0;

                context.SendDownstream(new SendHttpResponse(msg.HttpRequest, response));
            }
        }

        #endregion

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="request">Request that failed</param>
        /// <param name="err">Exception which was thrown</param>
        protected virtual void LogError(IRequest request, Exception err)
        {
            var sb = new StringBuilder();
            sb.Append("Failed to handle request ");
            sb.AppendLine(request.Uri.ToString());
            if (request.QueryString.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Query string: ");
                foreach (var var in request.QueryString)
                {
                    sb.AppendLine(string.Format("{0}: {1}", var.Name, string.Join(",", var)));
                }
            }
            if (request.Form.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Form: ");
                foreach (var var in request.QueryString)
                {
                    sb.AppendLine(string.Format("{0}: {1}", var.Name, string.Join(",", var)));
                }
            }
            sb.AppendLine();
            sb.AppendLine("Headers: ");
            foreach (var var in request.Headers)
            {
                sb.AppendLine(string.Format("{0}: {1}", var.Name, var.Value));
            }
            

            _logger.Error(sb.ToString(), err);
        }

        /// <summary>
        /// Invokes the <see cref="IErrorFormatter.Format"/> and guards against any exceptions that it might throw.
        /// </summary>
        /// <param name="response">Response to send back</param>
        /// <param name="msg">Request pipeline message</param>
        /// <param name="exception">Caught exception</param>
        protected virtual void FormatException(IResponse response, ReceivedHttpRequest msg, HttpException exception)
        {
            var formatterContext = new ErrorFormatterContext(exception, msg.HttpRequest, response);
            try
            {
                _formatter.Format(formatterContext);
            }
            catch (Exception err)
            {
                _logger.Error(string.Format("Formatter '{0}' failed to process request.", _formatter.GetType().FullName), err);
                var formatter = new SimpleErrorFormatter();
                formatter.Format(formatterContext);
            }
        }
    }
}