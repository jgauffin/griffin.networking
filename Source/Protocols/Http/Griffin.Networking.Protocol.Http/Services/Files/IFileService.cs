namespace Griffin.Networking.Http.Services.Files
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
        /// <remarks><c>true</c> if the file was attached to the response; otherwise false;</remarks>
        bool GetFile(FileContext context);
    }
}