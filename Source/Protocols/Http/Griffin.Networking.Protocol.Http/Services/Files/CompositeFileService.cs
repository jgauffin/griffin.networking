namespace Griffin.Networking.Protocol.Http.Services.Files
{
    /// <summary>
    /// Can serve files from multiple services.
    /// </summary>
    public class CompositeFileService : IFileService
    {
        private readonly IFileService[] _fileServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeFileService"/> class.
        /// </summary>
        /// <param name="fileServices">One or more file services.</param>
        public CompositeFileService(params IFileService[] fileServices)
        {
            _fileServices = fileServices;
        }

        #region IFileService Members

        /// <summary>
        /// Loops through all services and returns the first matching file.
        /// </summary>
        /// <param name="context">Context used to locate and return files</param>
        public virtual bool GetFile(FileContext context)
        {
            foreach (var fileService in _fileServices)
            {
                fileService.GetFile(context);
                if (context.IsFound)
                    return true;
            }

            return false;
        }

        #endregion
    }
}