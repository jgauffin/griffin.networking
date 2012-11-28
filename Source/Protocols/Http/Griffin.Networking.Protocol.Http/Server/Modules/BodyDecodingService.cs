using System;
using System.Linq;
using System.Net;
using Griffin.Networking.Protocol.Http.Services;

namespace Griffin.Networking.Protocol.Http.Server.Modules
{
    /// <summary>
    /// Will decode the request body into Files/Form
    /// </summary>
    /// <remarks>
    /// <para>Uses the <c>HandleRequest</c> method for the decoding. So you probably want to add this module before any module doing real work.</para>
    /// <para>Do note that the module with return <see cref="HttpStatusCode.UnsupportedMediaType"/> if the content type is not supported. You can turn off this behaviour by setting
    /// <see cref="BeRude"/> to false.</para>
    /// </remarks>
    public class BodyDecodingModule : IWorkerModule
    {
        private readonly IBodyDecoder[] _decoders;

        /// <summary>
        /// Initializes a new instance of the <see cref="BodyDecodingModule" /> class.
        /// </summary>
        /// <param name="decoders">One or more decoders.</param>
        public BodyDecodingModule(params IBodyDecoder[] decoders)
        {
            if (decoders == null) throw new ArgumentNullException("decoders");
            if (decoders.Length == 0)
                throw new ArgumentOutOfRangeException("decoders", decoders, "Expected one or more decoders.");

            _decoders = decoders;
            BeRude = true;
        }

        /// <summary>
        /// Gets or sets if we should set 
        /// </summary>
        public bool BeRude { get; set; }

        #region IWorkerModule Members

        /// <summary>
        /// Invoked before anything else
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <remarks>
        /// <para>The first method that is exeucted in the pipeline.</para>
        /// Try to avoid throwing exceptions if you can. Let all modules have a chance to handle this method. You may break the processing in any other method than the Begin/EndRequest methods.</remarks>
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
            if (_decoders.Any(decoder => decoder.Decode(context.Request)))
            {
                return ModuleResult.Continue;
            }

            if (!BeRude)
                return ModuleResult.Continue;

            context.Response.StatusCode = (int) HttpStatusCode.UnsupportedMediaType;
            context.Response.StatusDescription = "We do not support content-type: " + context.Request.ContentType;
            return ModuleResult.Stop;
        }

        #endregion
    }
}