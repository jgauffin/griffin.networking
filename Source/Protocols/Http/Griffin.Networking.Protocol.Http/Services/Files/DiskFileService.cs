using System;
using System.IO;

namespace Griffin.Networking.Http.Services.Files
{
    /// <summary>
    /// Serves files from the hard drive.
    /// </summary>
    public class DiskFileService : IFileService
    {
        private readonly string _basePath;
        private readonly string _rootUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeFileService"/> class.
        /// </summary>
        /// <param name="rootFilePath">Path to serve files from.</param>
        /// <param name="rootUri">Serve all files which are located under this URI</param>
        /// <example>
        /// <code>
        /// var diskFiles = new DiskFileService("/public/", @"C:\www\public\");
        /// var module = new FileModule(diskFiles);
        /// 
        /// var moduleManager = new ModuleManager();
        /// moduleManager.Add(module);
        /// </code>
        /// </example>
        public DiskFileService(string rootUri, string rootFilePath)
        {
            if (rootUri == null) throw new ArgumentNullException("rootUri");
            if (rootFilePath == null) throw new ArgumentNullException("rootFilePath");
            if (!Directory.Exists(rootFilePath))
                throw new ArgumentOutOfRangeException("rootFilePath", rootFilePath,
                                                      "Failed to find path " + rootFilePath);

            _rootUri = rootUri;
            _basePath = rootFilePath;
        }

        #region IFileService Members

        /// <summary>
        /// Get a file
        /// </summary>
        /// <param name="context">Context used to locate and return files</param>
        public virtual bool GetFile(FileContext context)
        {
            if (!context.Request.Uri.AbsolutePath.StartsWith(_rootUri))
                return false;


            var relativeUri = context.Request.Uri.AbsolutePath.Remove(0, _rootUri.Length);
            var fullPath = Path.Combine(_basePath, relativeUri.TrimStart('/').Replace('/', '\\'));
            if (!File.Exists(fullPath))
                return false;


            var date = File.GetLastWriteTimeUtc(fullPath);
            if (date <= context.BrowserCacheDate)
            {
                context.SetNotModified(fullPath);
                return true;
            }

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            context.SetFile(fullPath, stream, date);
            return true;
        }

        #endregion
    }
}