using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Griffin.Networking.Http.Services.Errors
{

    /// <summary>
    /// Used to format all HTTP error messages
    /// </summary>
    public class SimpleErrorFormatter : IErrorFormatter
    {
        /// <summary>
        /// Format the response into something that the user understands.
        /// </summary>
        /// <param name="context">Context providing information for the error message generation</param>
        public void Format(ErrorFormatterContext context)
        {
            if (context.Response.Body == null)
                context.Response.Body = new MemoryStream();

            context.Response.ContentType = "text/html";
            var writer = new StreamWriter(context.Response.Body);
            writer.WriteLine("<html><head><title>Errror 40</title></head><body>");
            writer.WriteLine("<h1>Opps. An error occurred.</h1>");
            writer.WriteLine("<p>" + context.Exception.Message + "</p>");
            writer.WriteLine("</body></html>");
        }
    }
}
