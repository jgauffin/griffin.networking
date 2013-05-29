using System;
using System.Net;
using Griffin.Networking.Buffers;
using Griffin.Networking.Protocol.Http.Implementation;
using Griffin.Networking.Protocol.Http.Protocol;
using Griffin.Networking.Servers;

namespace Griffin.Networking.Protocol.Http
{
    /// <summary>
    /// Base class for handling HTTP requests in the server.
    /// </summary>
    public abstract class HttpService : INetworkService
    {
        private readonly IBufferSliceStack _stack;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpService" /> class.
        /// </summary>
        /// <param name="sliceStack">Used to retreive the buffers which are used during message serialization. Make sure that each buffer is large enough to serialize all headers.</param>
        /// <exception cref="System.ArgumentNullException">sliceStack</exception>
        /// <remarks>You typically want to pass a <c>static</c> stack to this constructor if you want performance.</remarks>
        protected HttpService(IBufferSliceStack sliceStack)
        {
            if (sliceStack == null) throw new ArgumentNullException("sliceStack");
            _stack = sliceStack;
        }

        /// <summary>
        /// Context  used to communicate with the client
        /// </summary>
        public IServerClientContext Context { get; private set; }

        #region INetworkService Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public abstract void Dispose();

        /// <summary>
        /// Assign the context which can be used to communicate with the client
        /// </summary>
        /// <param name="context">Context</param>
        public void Assign(IServerClientContext context)
        {
            Context = context;
        }

        /// <summary>
        /// A new message have been received from the remote end.
        /// </summary>
        /// <param name="message">You'll receive <see cref="IRequest"/> or <see cref="IResponse"/> depending on the type of application.</param>
        void INetworkService.HandleReceive(object message)
        {
            // Violates LSP, but the best solution that I could come up with.
            var ourRequest = message as HttpRequest;
            if (ourRequest != null)
                ourRequest.RemoteEndPoint = Context.RemoteEndPoint;

            var request = (IRequest) message;
            request.AddHeader("RemoteAddress", Context.RemoteEndPoint.ToString());

            OnRequest(request);
        }

        /// <summary>
        /// We've received a HTTP request.
        /// </summary>
        /// <param name="request">HTTP request</param>
        public abstract void OnRequest(IRequest request);
        

        /// <summary>
        /// An unhandled exception was caught when handling incoming bytes.
        /// </summary>
        /// <param name="context">Information about the exception that was caught</param>
        public virtual void OnUnhandledException(ServiceExceptionContext context)
        {
            
        }

        #endregion

        /// <summary>
        /// Send a HTTP message
        /// </summary>
        /// <param name="message">Message to send</param>
        public void Send(IMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var slice = _stack.Pop();
            var stream = new SliceStream(slice);
            var serializer = new HttpHeaderSerializer();
            serializer.SerializeResponse((IResponse) message, stream);
            Context.Send(slice, (int) stream.Length);
            if (message.ContentLength > 0 && message.Body == null)
                throw new InvalidOperationException("A content length is specified, but the Body stream is null.");

            if (message.Body != null)
                Context.Send(message.Body);

            if (message.ProtocolVersion == "HTTP/1.0")
                Context.Close();
        }
    }
}