using System;
using System.IO;
using Griffin.Networking.Protocol.Http.Protocol;

namespace Griffin.Networking.Protocol.Http.Services.Files
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
        /// <param name="browserCacheDate">Usually the header "If-Modified-Since"</param>
        public FileContext(IRequest request, DateTime browserCacheDate)
        {
            if (request == null) throw new ArgumentNullException("request");
            Request = request;
            BrowserCacheDate = browserCacheDate;
        }


        /// <summary>
        /// Gets if file was found;
        /// </summary>
        /// <remarks>The stream is not set if the file was found but not modified.</remarks>
        public bool IsFound { get; private set; }

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
        /// Gets filename
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Set file that should be returned.
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="stream">File stream</param>
        /// <param name="lastModifiedAtUtc">When the file was modified (UTC time).</param>
        /// <remarks>
        /// <para>The stream will be disposed by the server after it's being sent</para>
        /// <para>Use <see cref="SetNotModified"/> if the file has not been modified</para>
        /// </remarks>
        public void SetFile(string fileName, Stream stream, DateTime lastModifiedAtUtc)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            Filename = fileName;
            LastModifiedAtUtc = lastModifiedAtUtc;
            FileStream = stream;
            IsFound = true;
        }

        /// <summary>
        /// File has not been modified.
        /// </summary>
        /// <param name="fileName">File name including extension.</param>
        public void SetNotModified(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");
            Filename = fileName;
            LastModifiedAtUtc = BrowserCacheDate;
            IsFound = true;
        }
    }
}