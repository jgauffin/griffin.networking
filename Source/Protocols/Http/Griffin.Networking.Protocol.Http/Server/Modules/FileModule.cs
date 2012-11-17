using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Griffin.Networking.Http.Services;
using Griffin.Networking.Http.Services.Files;

namespace Griffin.Networking.Http.Server.Modules
{
    /// <summary>
    /// Will serve static files
    /// </summary>
    /// <example>
    /// <code>
    /// // One of the available file services.
    /// var diskFiles = new DiskFileService("/public/", @"C:\www\public\");
    /// var module = new FileModule(diskFiles);
    /// 
    /// // the module manager is added to the HttpServer.
    /// var moduleManager = new ModuleManager();
    /// moduleManager.Add(module);
    /// </code>
    /// </example>
    public class FileModule : IWorkerModule
    {
        private readonly IFileService _fileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileModule" /> class.
        /// </summary>
        /// <param name="fileService">The file service.</param>
        public FileModule(IFileService fileService)
        {
            if (fileService == null) throw new ArgumentNullException("fileService");
            _fileService = fileService;
        }


        /// <summary>
        /// Invoked before anything else
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <remarks>
        /// <para>The first method that is exeucted in the pipeline.</para>
        /// Try to avoid throwing exceptions if you can. Let all modules have a chance to handle this method. You may break the processing in any other method than the Begin/EndRequest methods.
        /// <para>If you are going to handle the request, implement <see cref="IWorkerModule"/> and do it in the <see cref="IWorkerModule.HandleRequest"/> method.</para>
        /// </remarks>
        public void BeginRequest(IHttpContext context)
        {

        }

        /// <summary>
        /// End request is typically used for post processing. The response should already contain everything required.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <remarks>
        /// <para>The last method that is executed in the pipeline.</para>
        /// Try to avoid throwing exceptions if you can. Let all modules have a chance to handle this method. You may break the processing in any other method than the Begin/EndRequest methods.</remarks>
        public void EndRequest(IHttpContext context)
        {

        }

        /// <summary>
        /// Handle the request.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns><see cref="ModuleResult.Stop"/> will stop all processing except <see cref="IHttpModule.EndRequest"/>.</returns>
        /// <remarks>Invoked in turn for all modules unless you return <see cref="ModuleResult.Stop"/>.</remarks>
        public ModuleResult HandleRequest(IHttpContext context)
        {
            var header = context.Request.Headers["If-Modified-Since"];
            var time = header != null
                           ? DateTime.ParseExact(header.Value, "R", CultureInfo.InvariantCulture)
                           : DateTime.MinValue;


            var fileContext = new FileContext(context.Request, time);
            _fileService.GetFile(fileContext);
            if (!fileContext.IsFound)
                return ModuleResult.Continue;

            var mimeType = MimeTypeProvider.Instance.Get(fileContext.Filename);
            if (mimeType == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.UnsupportedMediaType;
                context.Response.StatusDescription = string.Format("File type '{0}' is not supported.", Path.GetExtension(fileContext.Filename));
                return ModuleResult.Stop;
            }

            context.Response.AddHeader("Last-Modified", fileContext.LastModifiedAtUtc.ToString("R"));
            context.Response.ContentType = mimeType;
            context.Response.Body = fileContext.FileStream;
            return ModuleResult.Stop;
        }
    }
}
