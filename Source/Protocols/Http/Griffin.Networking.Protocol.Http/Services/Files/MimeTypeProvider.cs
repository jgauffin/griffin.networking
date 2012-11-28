using System;
using System.Collections.Generic;
using System.IO;

namespace Griffin.Networking.Protocol.Http.Services.Files
{
    /// <summary>
    /// All available mime types
    /// </summary>
    /// <remarks>All mime types in here can be served by the file modules. All other files are ignored.</remarks>
    public class MimeTypeProvider
    {
        public static MimeTypeProvider _instance = new MimeTypeProvider();
        private readonly Dictionary<string, string> _items = new Dictionary<string, string>();

        /// <summary>
        /// Prevents a default instance of the <see cref="MimeTypeProvider" /> class from being created.
        /// </summary>
        internal MimeTypeProvider()
        {
            _items.Add("default", "application/octet-stream");
            _items.Add("txt", "text/plain");
            _items.Add("html", "text/html");
            _items.Add("htm", "text/html");
            _items.Add("jpg", "image/jpg");
            _items.Add("jpeg", "image/jpg");
            _items.Add("bmp", "image/bmp");
            _items.Add("gif", "image/gif");
            _items.Add("png", "image/png");

            _items.Add("ico", "image/vnd.microsoft.icon");
            _items.Add("css", "text/css");
            _items.Add("gzip", "application/x-gzip");
            _items.Add("zip", "multipart/x-zip");
            _items.Add("tar", "application/x-tar");
            _items.Add("pdf", "application/pdf");
            _items.Add("rtf", "application/rtf");
            _items.Add("xls", "application/vnd.ms-excel");
            _items.Add("ppt", "application/vnd.ms-powerpoint");
            _items.Add("doc", "application/application/msword");
            _items.Add("js", "application/javascript");
            _items.Add("au", "audio/basic");
            _items.Add("snd", "audio/basic");
            _items.Add("es", "audio/echospeech");
            _items.Add("mp3", "audio/mpeg");
            _items.Add("mp2", "audio/mpeg");
            _items.Add("mid", "audio/midi");
            _items.Add("wav", "audio/x-wav");
            _items.Add("swf", "application/x-shockwave-flash");
            _items.Add("avi", "video/avi");
            _items.Add("rm", "audio/x-pn-realaudio");
            _items.Add("ram", "audio/x-pn-realaudio");
            _items.Add("aif", "audio/x-aiff");
        }

        /// <summary>
        /// Gets singleton
        /// </summary>
        public static MimeTypeProvider Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Add a mimn type
        /// </summary>
        /// <param name="extension">Extension without dot</param>
        /// <param name="mimeType">The mime type</param>
        public void Add(string extension, string mimeType)
        {
            if (extension == null) throw new ArgumentNullException("extension");
            if (mimeType == null) throw new ArgumentNullException("mimeType");
            _items[extension] = mimeType;
        }

        /// <summary>
        /// Remove a mime type
        /// </summary>
        /// <param name="extension">extension without dot</param>
        public void Remove(string extension)
        {
            if (extension == null) throw new ArgumentNullException("extension");
            _items.Remove(extension);
        }

        /// <summary>
        /// Get mime type for the specified file
        /// </summary>
        /// <param name="filename">Full path to file</param>
        /// <returns>Mime type</returns>
        public string Get(string filename)
        {
            if (filename == null) throw new ArgumentNullException("filename");

            var extension = Path.GetExtension(filename);
            if (extension == null)
                return null;

            string contentType;
            return _items.TryGetValue(extension.TrimStart('.'), out contentType) ? contentType : _items["default"];
        }
    }
}