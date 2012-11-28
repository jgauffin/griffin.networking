using System;
using System.IO;
using System.Net;
using Griffin.Networking.Protocol.Http.Server;
using Griffin.Networking.Protocol.Http.Server.Modules;
using Griffin.Networking.Protocol.Http.Services.Files;
using Griffin.Networking.Protocol.Http.Services.Routing;

namespace Griffin.Networking.Protocol.Http.DemoServer.HttpServerDemo
{
    internal class Program
    {
        public static void RunDemo()
        {
            var demoPath = Path.Combine(Environment.CurrentDirectory, "HttpServerDemo");

            var modules = new ModuleManager();

            var routerModule = new RouterModule();
            routerModule.Add(new DefaultDocumentRouter(demoPath, "index.html"));
            modules.Add(routerModule);

            var fileModule = new FileModule(new DiskFileService("/", demoPath));
            modules.Add(fileModule);

            var server = new HttpServer(modules);
            server.Start(IPAddress.Any, 8888);
        }
    }
}