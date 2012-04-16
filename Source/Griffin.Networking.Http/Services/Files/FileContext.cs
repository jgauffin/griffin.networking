using System;
using System.IO;
using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Services.Files
{
    /// <summary>
    /// Context used by <see cref="IFileService"/> when locating files.
    /// </summary>
    /// <remarks>
    /// There are three scenarios for files:
    /// <list type="table">
    /// <item>
    /// <term>Not found</term>
    /// <description>Simply do not change the context, just return from the method.</description>
    /// </item>
    /// <item>
    /// <term>Found but not modified.</term>
    /// <description>The file UTC date/time is less or equal to <see cref="FileContext.BrowserCacheDate"/>. Use <see cref="FileContext.SetNotModified"/> and return</description>
    /// </item>
    /// <item>
    /// <term>Found and newer</term>
    /// <description>The file UTC date/time is newer than <see cref="FileContext.BrowserCacheDate"/>. Use <see cref="FileContext.SetFile"/> and return.</description>
    /// </item>
    /// </list>
    /// </remarks>
    public class FileContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileContext"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="browserCacheDate"> </param>
        public FileContext(IRequest request, DateTime browserCacheDate)
        {
            if (request == null) throw new ArgumentNullException("request");
            Request = request;
            BrowserCacheDate = browserCacheDate;
        }

        /// <summary>
        /// Gets the request (the Uri specifies the wanted file)
        /// </summary>
        public IRequest Request { get; private set; }

        /// <summary>
        /// Gets date when file was cached in the browser.
        /// </summary>
        public DateTime BrowserCacheDate { get; set; }

        /// <summary>
        /// Gets the date when the file was modified (UTC time)
        /// </summary>
        public DateTime LastModifiedAtUtc { get; private set; }

        /// <summary>
        /// Gets file stream
        /// </summary>
        /// <remarks>The server will own the stream</remarks>
        public Stream FileStream { get; private set; }

        /// <summary>
        /// Set file that should be returned.
        /// </summary>
        /// <param name="stream">File stream</param>
        /// <param name="lastModifiedAtUtc">When the file was modified (UTC time).</param>
        /// <remarks>
        /// <para>The stream will be disposed by the server after it's being sent</para>
        /// <para>Use <see cref="SetNotModified"/> if the file has not been modified</para>
        /// </remarks>
        public void SetFile(Stream stream, DateTime lastModifiedAtUtc)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            LastModifiedAtUtc = lastModifiedAtUtc;
            FileStream = stream;
        }

        /// <summary>
        /// File has not been modified.
        /// </summary>
        public void SetNotModified()
        {
            LastModifiedAtUtc = BrowserCacheDate;
        }
    }
}