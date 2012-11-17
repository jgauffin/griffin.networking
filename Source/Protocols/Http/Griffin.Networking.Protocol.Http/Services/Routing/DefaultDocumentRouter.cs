using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Griffin.Networking.Http.Server;

namespace Griffin.Networking.Http.Services.Routing
{
    /// <summary>
    /// Will add default document to URIs
    /// </summary>
    /// <remarks>works for all directories</remarks>
    public class DefaultDocumentRouter : IRequestRouter
    {
        private readonly string _homeDirectory;
        private readonly string _documentName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDocumentRouter" /> class.
        /// </summary>
        /// <param name="homeDirectory">Home directory on disk.</param>
        /// <param name="documentName">Name of the default document.</param>
        /// <example>
        /// <code>
        /// var documentRouter = new DefaultDocumentRouter(@"C:\www\mysite\", "index.html");
        /// </code>
        /// </example>
        public DefaultDocumentRouter(string homeDirectory, string documentName)
        {
            if (homeDirectory == null) throw new ArgumentNullException("homeDirectory");
            if (documentName == null) throw new ArgumentNullException("documentName");
            if (!Directory.Exists(homeDirectory))
                throw new ArgumentException("Homedirectory do not exist: " + homeDirectory, "homeDirectory");

            _homeDirectory = homeDirectory;
            _documentName = documentName;
        }

        /// <summary>
        /// Route the request.
        /// </summary>
        /// <param name="context">HTTP context used to identify the route</param>
        /// <returns><c>true</c> if we generated some routing; otherwise <c>false</c></returns>
        public bool Route(IHttpContext context)
        {
            var askedPath = context.Request.Uri.AbsolutePath.Replace("/", @"\").TrimStart('/');
            if (askedPath.Contains("."))
                return false;
            if (askedPath == "\\")
                askedPath = "";

            var diskPath = Path.Combine(_homeDirectory, askedPath) + "\\" + _documentName;
            if (!File.Exists(diskPath))
                return false;

            var url = context.Request.Uri.Scheme + "://" + context.Request.Uri.Host;
            if (context.Request.Uri.Port != 80)
                url += ":" + context.Request.Uri.Port;
            url += context.Request.Uri.AbsolutePath;
            url += _documentName;
            if (!string.IsNullOrEmpty(context.Request.Uri.Query))
                url += "?" + context.Request.Uri.Query;

            context.Request.Uri = new Uri(url);

            // never return true, since we just want to complete the path
            return false;
        }
    }
}
