using System;
using System.IO;
using Griffin.Networking.Protocol.Http.Protocol;

namespace Griffin.Networking.Protocol.Http.Implementation
{
    /// <summary>
    /// A HTTP file in a request.
    /// </summary>
    /// <remarks>The temporary file will be deleted when the request/response ends.</remarks>
    public class HttpFile : IHttpFile, IDisposable
    {
        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            File.Delete(TempFileName);
        }

        #endregion

        #region IHttpFile Members

        /// <summary>
        /// Gets or sets form element name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets client side file name
        /// </summary>
        public string OriginalFileName { get; set; }

        /// <summary>
        /// Gets or sets mime content type
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets full path to local file
        /// </summary>
        public string TempFileName { get; set; }

        #endregion
    }
}