using System.Net;

namespace Griffin.Networking.Protocol.Http.Services.ViewEngines
{
    /// <summary>
    /// The requested view as not located.
    /// </summary>
    public class ViewNotFoundException : HttpException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewNotFoundException" /> class.
        /// </summary>
        /// <param name="viewPath">The view path.</param>
        public ViewNotFoundException(string viewPath)
            : base(HttpStatusCode.InternalServerError, string.Format("Failed to find view '{0}'.", viewPath))

        {
            ViewPath = viewPath;
        }

        /// <summary>
        /// Gets view that was not found;
        /// </summary>
        public string ViewPath { get; private set; }
    }
}