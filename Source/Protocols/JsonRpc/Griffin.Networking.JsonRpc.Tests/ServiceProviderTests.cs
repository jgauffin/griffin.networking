using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Griffin.Networking.JsonRpc.Remoting;
using Xunit;

namespace Griffin.Networking.JsonRpc.Tests
{
    public class SimpleServiceLocator : IServiceLocator
    {
        /// <summary>
        /// Resolve a service
        /// </summary>
        /// <param name="type">Type of service to locate.</param>
        // <param name="channel">Channel that the resolve is for. Can be used to control lifetime scope, as each handler should live as long as the channel.</param>
        /// <returns>The registered service</returns>
        /// <exception cref="InvalidOperationException">Failed to find service.</exception>
        public object Resolve(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
    public class ServiceProviderTests
    {
        [Fact]
        public void InvokeMethodWithArguments()
        {
            RpcServiceInvoker sp = new RpcServiceInvoker(new DotNetValueConverter(), new SimpleServiceLocator());
            sp.Map<MyService>();

            var result = sp.Invoke(new Request
                          {
                              Id = 10,
                              JsonRpc = "2.0",
                              Method = "Calculate",
                              Parameters = new object[] { "2", "3" }
                          });

            Assert.IsType<Response>(result);
            Response r = (Response)result;
            Assert.Equal(r.Result, "6");

        }

        [Fact]
        public void OperationContractWithoutName()
        {
            RpcServiceInvoker sp = new RpcServiceInvoker(new DotNetValueConverter(), new SimpleServiceLocator());
            sp.Map<ServiceWithoutName>();

            var result = sp.Invoke(new Request
            {
                Id = 10,
                JsonRpc = "2.0",
                Method = "None"
            });

            Assert.IsType<Response>(result);
            Response r = (Response)result;
            Assert.Equal(r.Result, "1");
        }

        public class ServiceWithoutName
        {
            [OperationContract]
            public string None()
            {
                return "1";
            }
        }

    }


    public class MyService
    {
        [OperationContract(Name = "Calculate")]
        public string Calculate(int x, int y)
        {
            return (x * y).ToString(CultureInfo.InvariantCulture);
        }
    }
}
