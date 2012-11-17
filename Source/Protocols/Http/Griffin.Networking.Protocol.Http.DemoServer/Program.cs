using System;

namespace Griffin.Networking.Http.DemoServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //LogManager.Assign(new SimpleLogManager<ConsoleLogger>());

            Console.WriteLine("Choose your poison:");
            Console.WriteLine(" 1. Simple server");
            Console.WriteLine(" 2. Pipeline server");
            Console.WriteLine(" 3. Complete HTTP Server");
            Console.Write("Choice: ");
            var key = Console.ReadKey();
            Console.WriteLine();
            Console.WriteLine();

            switch (key.Key)
            {
                case ConsoleKey.D1:
                    Basic.Program.RunDemo();
                    Console.WriteLine("Now running the simple server. Browse to http://localhost:8888");
                    break;
                case ConsoleKey.D2:
                    ThroughPipeline.Program.RunDemo();
                    Console.WriteLine("Now running the pipeline server. Browse to http://localhost:8888");
                    break;
                default:
                    HttpServerDemo.Program.RunDemo();
                    Console.WriteLine("Now running the HTTP server. Browse to http://localhost:8888");
                    break;
            }


            /*
            var cb = new ContainerBuilder();
            cb.RegisterType<HttpParser>().AsImplementedInterfaces().SingleInstance();
            cb.RegisterType<ResponseEncoder>().AsSelf().SingleInstance();
            cb.RegisterType<HeaderDecoder>().AsSelf().SingleInstance();
            cb.RegisterType<MessageHandler>().AsSelf().SingleInstance();
            cb.RegisterType<FileHandler>().AsSelf().SingleInstance();
            var serviceLocator = new AutofacServiceLocator(cb.Build());
            */


            Console.ReadLine();
        }
    }
}