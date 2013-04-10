using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Griffin.Networking.Buffers;
using Griffin.Networking.Protocol.Http.Protocol;
using Griffin.Networking.Messaging;
using Griffin.Networking.Protocol.Http.Services.Authentication;

namespace Griffin.Networking.Protocol.Http.DemoServer.Basic
{
    /// <summary>
    /// Will handle all incoming HTTP requests.
    /// </summary>
    public class MyHttpService : HttpService
    {
        private static readonly BufferSliceStack Stack = new BufferSliceStack(50, 32000);

        /// <summary>
        /// Initializes a new instance of the <see cref="MyHttpService" /> class.
        /// </summary>
        public MyHttpService()
            : base(Stack)
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
        }

        /// <summary>
        /// We've received a HTTP request.
        /// </summary>
        /// <param name="request">HTTP request</param>
        public override void OnRequest(IRequest request)
        {
            var response = request.CreateResponse(HttpStatusCode.OK, "Welcome");
            if (request.Uri.AbsolutePath.Contains("secret"))
            {
                SecretPage(request, response);
                return;
            }

            if (request.Uri.AbsolutePath.EndsWith("MrEinstein.png"))
            {
                response.Body =
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(GetType().Namespace + ".einstein.png");
                response.ContentType = "image/png";
            }
            else
            {
                response.Body = new MemoryStream();
                response.ContentType = "text/html";
                var buffer = Encoding.UTF8.GetBytes(@"<html><head><title>Totally awesome</title></head><body><h1>Hello world</h1><img src=""MrEinstein.png"" /></body></html>");
                response.Body.Write(buffer, 0, buffer.Length);
                response.Body.Position = 0;
            }

            Send(response);
        }

        private void SecretPage(IRequest request, IResponse response)
        {
            var repos = new SingleRealmRepository("MyRealm");
            var storage = new DummyUserStorage();
            var authenticator = new DigestAuthenticator(repos, storage);


            if (request.Headers["Authorization"] == null)
            {
                authenticator.CreateChallenge(request, response);
                Send(response);
                return;
            }
            var user = authenticator.Authenticate(request);
            if (user == null)
            {
                response.StatusCode = 403;
                Send(response);
                return;
            }

            response.Body = new MemoryStream();
            response.ContentType = "text/plain";
            var buffer = Encoding.UTF8.GetBytes(@"Welcome to my secret place");
            response.Body.Write(buffer, 0, buffer.Length);
            response.Body.Position = 0;
            Send(response);
        }
    }
}