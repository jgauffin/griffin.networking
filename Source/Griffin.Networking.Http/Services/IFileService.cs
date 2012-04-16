using Griffin.Networking.Http.Services.Files;

namespace Griffin.Networking.Http.Services
{
    /// <summary>
    /// Serves files
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Get a file
        /// </summary>
        /// <param name="context">Context used to locate and return files</param>
        void GetFile(FileContext context);
    }
}