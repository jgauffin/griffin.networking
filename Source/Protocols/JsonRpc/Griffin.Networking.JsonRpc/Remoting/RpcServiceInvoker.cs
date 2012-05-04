using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.ServiceModel;

namespace Griffin.Networking.JsonRpc.Remoting
{
    /// <summary>
    /// Basic implementation of a service provider.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class RpcServiceInvoker : IRpcInvoker
    {
        private readonly IValueConverter _valueConverter;
        private readonly IServiceLocator _serviceLocator;
        private readonly Dictionary<string, Mapping> _mappings = new Dictionary<string, Mapping>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RpcServiceInvoker"/> class.
        /// </summary>
        /// <param name="valueConverter">Used to convert sent values into something that can be used by the service.</param>
        /// <param name="serviceLocator">The service locator used to find the services..</param>
        public RpcServiceInvoker(IValueConverter valueConverter, IServiceLocator serviceLocator)
        {
            _valueConverter = valueConverter;
            _serviceLocator = serviceLocator;
        }

        /// <summary>
        /// Map a service to a RPC method
        /// </summary>
        /// <typeparam name="T">Type which methods are decorated with the <see cref="OperationContractAttribute"/>.</typeparam>
        /// <remarks>
        /// You can use the <c>Name</c> property of <see cref="OperationContractAttribute"/> to use another name than the method name.
        /// </remarks>
        public void Map<T>()
        {
            foreach (var methodInfo in typeof (T).GetMethods())
            {
                var attr =
                    methodInfo.GetCustomAttributes(typeof (OperationContractAttribute), true).Cast
                        <OperationContractAttribute>().FirstOrDefault();
                if (attr == null)
                    continue;

                var name = string.IsNullOrEmpty(attr.Name) ? methodInfo.Name : attr.Name;
                _mappings.Add(name, new Mapping {ServiceType = typeof (T), Method = methodInfo});
            }
        }

        /// <summary>
        /// Create an error response
        /// </summary>
        /// <param name="id">Request id</param>
        /// <param name="errorCode">Error code</param>
        /// <param name="message">Message used to understand what when wrong and how it can be prevented in the future.</param>
        /// <returns>The error response</returns>
        protected virtual ErrorResponse CreateErrorResponse(object id, int errorCode, string message)
        {
            return new ErrorResponse(id, new RpcError
                                             {
                                                 Code = errorCode,
                                                 Message = message
                                             });
        }

        /// <summary>
        /// Invoke the request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ResponseBase Invoke(Request request)
        {
            Mapping mapping;
            if (!_mappings.TryGetValue(request.Method, out mapping))
            {
                return CreateErrorResponse(request.Id, RpcErrorCode.MethodNotFound,
                                           string.Format("Failed to find method named '{0}'.", request.Method));
            }

            var methodParameters = mapping.Method.GetParameters();
            var dictionary = request.Parameters as IDictionary<string, object>;
            if (dictionary != null)
            {
                ResponseBase errorResponse;
                if (ConvertToObjectArray(request, methodParameters, dictionary, out errorResponse))
                    return errorResponse;
            }

            object[] fixedParameters;
            if (request.Parameters == null)
                fixedParameters = null;
            else
            {
                var parameters = (object[]) request.Parameters;
                if (parameters.Length != methodParameters.Length)
                {
                    return CreateErrorResponse(request.Id, RpcErrorCode.InvalidParameters,
                                               "Expected '" + methodParameters.Length + "' number of parameters.");
                }

                ErrorResponse response;
                fixedParameters = ConvertParameters(request, methodParameters, out response);
                if (fixedParameters == null)
                    return response;
            }

            var instance = _serviceLocator.Resolve(mapping.ServiceType);
            var result = mapping.Method.Invoke(instance, fixedParameters);

            return new Response(request.Id, result);
        }

        private bool ConvertToObjectArray(Request request, IEnumerable<ParameterInfo> methodParameters,
                                          IDictionary<string, object> requestParameters,
                                          out ResponseBase errorResponse)
        {
            var parameters = new object[requestParameters.Count];
            var index = 0;
            foreach (var methodParameter in methodParameters)
            {
                object value;
                if (!requestParameters.TryGetValue(methodParameter.Name, out value))
                {
                    errorResponse = CreateErrorResponse(request.Id, RpcErrorCode.InvalidParameters,
                                                        "Failed to find a parameter named '" + methodParameter.Name +
                                                        "' in the RPC request (which the service requires).");
                    return true;
                }

                parameters[index++] = value;
            }

            errorResponse = null;
            return false;
        }

        private object[] ConvertParameters(Request request, ParameterInfo[] methodParameters, out ErrorResponse error)
        {
            if (methodParameters.Length == 0)
            {
                error = null;
                return new object[0];
            }

            var fixedParameters = new object[methodParameters.Length];

            var parameters = (object[]) request.Parameters;
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var methodParameter = methodParameters[i];

                // Convert the parameter if required.
                if (!methodParameter.ParameterType.IsAssignableFrom(parameter.GetType()))
                {
                    object converted;
                    if (!_valueConverter.TryConvert(parameter, methodParameter.ParameterType, out converted))
                    {
                        error = CreateErrorResponse(request.Id, RpcErrorCode.InvalidParameters,
                                                    string.Format(
                                                        "Parameter '{0}' is a '{1}' while the service expected a '{2}'",
                                                        i, parameter.GetType().Name,
                                                        methodParameter.ParameterType.Name));


                        return null;
                    }

                    fixedParameters[i] = converted;
                }
                else
                    fixedParameters[i] = parameters[i];
            }

            error = null;
            return fixedParameters;
        }



        #region Nested type: Mapping

        private class Mapping
        {
            public Type ServiceType { get; set; }
            public MethodInfo Method { get; set; }
        }

        #endregion
    }
}