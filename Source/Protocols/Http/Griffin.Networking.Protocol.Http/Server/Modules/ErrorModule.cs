using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using Griffin.Networking.Logging;

namespace Griffin.Networking.Http.Server.Modules
{
    /// <summary>
    /// Reports errors to different sources.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// var module = new ErrorModule();
    /// module.SendEmailTo("arne@somewhere.com", "myWebserver@mydomain.com");
    /// module.SendEmailTo("webmaster@mydomain.com", "myWebserver@mydomain.com");
    /// module.BuildCustomErrorPage(context => "<html><body>Opps, fail with exception: " + context.LastException + ". Too bad :(</html></body>");
    /// module.LogDetails(details => _logger.Error("Request failed: " + details);
    /// module.LogDetails(details => EventLog.WriteEntry(details);
    /// ]]>
    /// </code>
    /// </example>
    public class ErrorModule : IHttpModule
    {
        private readonly List<Action<IRequestContext>> _actions = new List<Action<IRequestContext>>();
        private readonly ILogger _logger = LogManager.GetLogger<ErrorModule>();

        #region IHttpModule Members

        /// <summary>
        /// Invoked before anything else
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <remarks>
        /// <para>The first method that is exeucted in the pipeline.</para>
        /// Try to avoid throwing exceptions if you can. Let all modules have a chance to handle this method. You may break the processing in any other method than the Begin/EndRequest methods.</remarks>
        public void BeginRequest(IRequestContext context)
        {
        }

        /// <summary>
        /// End request is typically used for post processing. The response should already contain everything required.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <remarks>
        /// <para>The last method that is executed in the pipeline.</para>
        /// Try to avoid throwing exceptions if you can. Let all modules have a chance to handle this method. You may break the processing in any other method than the Begin/EndRequest methods.</remarks>
        public void EndRequest(IRequestContext context)
        {
            if (context.LastException == null)
                return;

            foreach (var action in _actions)
            {
                action(context);
            }
        }

        #endregion

        /// <summary>
        /// Send the error to an email address
        /// </summary>
        /// <param name="toAddress">Recipient.</param>
        /// <param name="fromAddress">Who the mail should be sent from.</param>
        /// <remarks>You have to configure your SMTP server in app.config under system.net. google.</remarks>
        public void SendEmailTo(string toAddress, string fromAddress)
        {
            _actions.Add(x => SendEmail(x, toAddress, fromAddress));
        }

        /// <summary>
        /// Log error details to somemwhere
        /// </summary>
        /// <param name="action"></param>
        /// <remarks>Will include request information, the logged in user and the exception details.</remarks>
        public void LogDetails(Action<string> action)
        {
            if (action == null) throw new ArgumentNullException("action");
            _actions.Add(x => action(GenerateErrorInfo(x)));
        }

        /// <summary>
        /// Build a custom error page
        /// </summary>
        /// <param name="action">Should return a string which corresponds to the error page that should be displayed.</param>
        public void BuildCustomErrorPage(Func<IRequestContext, string> action)
        {
            if (action == null) throw new ArgumentNullException("action");
            _actions.Add(x => SetErrorPage(x, action(x)));
        }

        private void SetErrorPage(IRequestContext requestContext, string errorPage)
        {
            var bytes = requestContext.Response.ContentEncoding.GetBytes(errorPage);
            requestContext.Response.Body = new MemoryStream();
            requestContext.Response.Body.Write(bytes, 0, bytes.Length);
            requestContext.Response.ContentType = "text/html";
        }

        private void SendEmail(IRequestContext requestContext, string toAddress, string fromAddress)
        {
            var client = new SmtpClient();
            try
            {
                var errorInfo = GenerateErrorInfo(requestContext);
                var msg = new MailMessage(fromAddress, toAddress, "HTTP Server Error", errorInfo);
                client.Send(msg);
            }
            catch (Exception err)
            {
                _logger.Error("Failed to send email.", err);
            }
        }

        private static string GenerateErrorInfo(IRequestContext requestContext)
        {
            var errorInfo = requestContext.LastException + "\r\n============================\r\n" +
                            requestContext.Request.BuildErrorInfo() +
                            "User: " + requestContext.User.Identity;
            return errorInfo;
        }
    }
}